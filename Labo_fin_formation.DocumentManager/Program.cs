using JwtConfiguration;
using Labo_fin_formation.DocumentManager.Repositories;
using Labo_fin_formation.DocumentManager.Services;
using Labo_fin_formation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<SqlConnection>(c => new SqlConnection(builder.Configuration.GetConnectionString("LaboDocAdminConnectionString")));

builder.Services.AddJwtAutnetication(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<PolicyService>();

// 🔹 Ajouter Authorization avec des policies dynamiques
builder.Services.AddAuthorization(options =>
{
    using var scope = builder.Services.BuildServiceProvider().CreateScope();
    var policyService = scope.ServiceProvider.GetRequiredService<PolicyService>();
    var policies = policyService.GetRolePoliciesAsync().Result; // ⚠️ Attention au .Result pour éviter async ici

    if (policies.Any())
    {
        var policiesdistinct = policies.GroupBy(grp => grp.PolicyName)
            .Select(grp => new
            {
                police = grp.Key,
                roles = grp.Select(s => s.RoleName).ToList()
            });

        foreach (var ps in policiesdistinct)
        {
            options.AddPolicy(ps.police, policy => policy.RequireRole(ps.roles.ToArray()));

            Console.WriteLine($"✅ Policy ajoutée : {ps.police} avec rôles {string.Join(", ", ps.roles)}");
        }
    }
    else
    {
        options.AddPolicy("DefaultRestrictedPolicy", policy => policy.RequireRole("DefaultUser"));
    }
});

//CORS
string[] corsOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins(corsOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

//Logging
builder.Services.AddLogging();

//Swagger (API documentation)
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<SwaggerFilters.EmptyStringSchemaFilter>();
    options.SchemaFilter<SwaggerFilters.DefaultBooleanSchemaFilter>();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter the token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{ }
        }
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
