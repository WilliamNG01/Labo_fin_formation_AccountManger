using Microsoft.AspNetCore.Identity;

namespace Labo_fin_formation.APIAccountManagement.Domain.Entities
{
    public class ApplicationRole: IdentityRole
    {
        public string? Description { get; set; }
    }
}