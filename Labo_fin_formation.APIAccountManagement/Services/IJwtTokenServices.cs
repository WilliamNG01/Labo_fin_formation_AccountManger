using Labo_fin_formation.APIAccountManagement.Domain.Entities;

namespace Labo_fin_formation.APIAccountManagement.Services
{
    public interface IJwtTokenServices
    {
        string GenerateJwtToken(List<ApplicationRole> userRoles, ApplicationUser user);
    }
}