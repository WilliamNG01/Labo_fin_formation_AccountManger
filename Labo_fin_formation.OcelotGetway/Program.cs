using JwtConfiguration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
});

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddJwtAutnetication(builder.Configuration);
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = !string.IsNullOrEmpty(builder.Configuration["Jwt:Issuer"]),
//            ValidateAudience = !string.IsNullOrEmpty(builder.Configuration["Jwt:Audience"]),
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };
//    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

try
{
    await app.UseOcelot();
}
catch (Exception ex)
{
    // Gérer l'exception ici (par exemple, journalisation, message d'erreur)
    Console.WriteLine($"Erreur lors du démarrage d'Ocelot : {ex.Message}");
}

app.Run();
