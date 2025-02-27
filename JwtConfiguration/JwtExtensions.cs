using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JwtConfiguration;

public static class JwtExtensions
{
    public static string? TheSecurityKey;
    public static string? TheIssueur;
    public static string? TheAudience;
    public static void AddJwtAutnetication(this IServiceCollection services, IConfiguration _configuration)
    {
        TheSecurityKey = _configuration["Jwt:Key"];
        TheIssueur = _configuration["Jwt:Issuer"];
        TheAudience = _configuration["Jwt:Audience"];
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = !string.IsNullOrEmpty(_configuration["Jwt:Issuer"]),
                    ValidateAudience = !string.IsNullOrEmpty(_configuration["Jwt:Audience"]),
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
                };
            });
    }

}
