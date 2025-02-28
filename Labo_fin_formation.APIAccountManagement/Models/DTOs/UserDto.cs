using Labo_fin_formation.APIAccountManagement.Domain.Entities;

namespace Labo_fin_formation.APIAccountManagement.Models.DTOs
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public IList<string?> Roles { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool GDPRConsent { get; set; }
        
        public UserDto(){}
        public UserDto(ApplicationUser user, IList<string?> roles)
        {
            this.Id = user.Id;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.UserName = user.UserName;
            this.Email = user.Email;
            this.EmailConfirmed = user.EmailConfirmed;
            this.PhoneNumber = user.PhoneNumber;
            this.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            this.TwoFactorEnabled = user.TwoFactorEnabled;
            this.GDPRConsent = user.GDPRConsent;
            this.Roles = roles;
        }
    }
}
