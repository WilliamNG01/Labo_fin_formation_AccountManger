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

            Dictionary<int, string> roles = new()
            {
                { 0, "DefaultUser" },
                { 1, "Bénévole" },
                { 2, "Fournisseur" },
                { 3, "Patient" },
                { 4, "Employé" },
                { 5, "Coordinateur" },
                { 6, "Infirmier" },
                { 7, "Médecin" },
                { 8, "Admin" },
                { 9, "SuperAdmin" }
            };


            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Value))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = role.Value, Description = $"{role.Value} Role", RoleLevel = role.Key });
                }
            }
        }
    }
}
