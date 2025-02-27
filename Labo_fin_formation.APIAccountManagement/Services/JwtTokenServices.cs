using JwtConfiguration;
using Labo_fin_formation.APIAccountManagement.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Labo_fin_formation.APIAccountManagement.Services
{
    public class JwtTokenServices: IJwtTokenServices
    {
        public string GenerateJwtToken(List<ApplicationRole> userRoles, ApplicationUser user)
        {
            List<Claim> authClaims =
                                [
                                new Claim(ClaimTypes.NameIdentifier, user.Id),
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.Email,user.Email)
                                ];

            // Ajouter les rôles de l'utilisateur
            //IList<string> userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, $"{role.Name} : {role.RoleLevel}"));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtExtensions.TheSecurityKey));

            var token = new JwtSecurityToken(
                issuer: JwtExtensions.TheIssueur,
                audience: JwtExtensions.TheAudience,
                expires: DateTime.UtcNow.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
