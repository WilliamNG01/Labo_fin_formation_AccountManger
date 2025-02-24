using Microsoft.AspNetCore.Identity;

namespace Labo_fin_formation.APIAccountManagement.Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool GDPRConsent { get; set; }
    }
}
