using Labo_fin_formation.APIAccountManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Labo_fin_formation.APIAccountManagement.Infrastructure.Identity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Map Identity tables to the 'Idtty' schema
            builder.Entity<ApplicationUser>().ToTable("AspNetUsers", "Idtty");
            builder.Entity<ApplicationRole>().ToTable("AspNetRoles", "Idtty");
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "Idtty");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "Idtty");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "Idtty");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "Idtty");
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "Idtty");
        }
    }
}
