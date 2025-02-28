using JwtConfiguration;
using Labo_fin_formation.APIAccountManagement.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Labo_fin_formation.APIAccountManagement.Services
{
    public class JwtTokenServices: IJwtTokenServices
    {
        private readonly IConfiguration _configuration;
        public JwtTokenServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateJwtToken(List<ApplicationRole> userRoles, ApplicationUser user)
        {
            //List<Claim> authClaims =
            //                    [
            //                    new Claim(ClaimTypes.Sub, user.Id),
            //                    new Claim(ClaimTypes.Name, user.UserName),
            //                    new Claim(ClaimTypes.Email,user.Email)
            //                    //new Claim(ClaimTypes.Role,userRoles.Where(x => x.RoleLevel == userRoles.Max(m => m.RoleLevel)).First().Name)
            //                    ];
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // Ajouter les rôles de l'utilisateur
            // Ajouter les rôles comme claims
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

            //string TheSecurityKey = _configuration["Jwt:Key"];
            //string TheIssueur = _configuration["Jwt:Issuer"];
            //string TheAudience = _configuration["Jwt:Audience"];
            //var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TheSecurityKey));

            //var token = new JwtSecurityToken(
            //    issuer: TheIssueur,
            //    audience: TheAudience,
            //    expires: DateTime.UtcNow.AddHours(1),
            //    claims: authClaims,
            //    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            //);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
