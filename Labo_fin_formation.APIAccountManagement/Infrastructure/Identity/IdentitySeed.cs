using Labo_fin_formation.APIAccountManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Labo_fin_formation.APIAccountManagement.Infrastructure.Identity
{
    public class IdentitySeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "SuperAdmin", "Admin", "Médecin", "Infirmier", "Coordinateur", "Employé", "Bénévole", "Fournisseur", "Patient", "DefaultUser" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = roleName, Description = $"{roleName} Role" });
                }
            }
        }
    }
}
