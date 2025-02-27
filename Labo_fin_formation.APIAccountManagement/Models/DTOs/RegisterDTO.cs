using System.ComponentModel.DataAnnotations;

namespace Labo_fin_formation.APIAccountManagement.Models.DTOs;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }= string.Empty;

    [Required]
    [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public string? ConfirmPassword { get; set; }

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    public string? UserName { get; set; }

    public string? Role { get; set; }

    public bool TwoFactorEnabled { get; set; }= false;

    [Required]
    public bool GdprAccepted { get; set; }

    public string? PhoneNumber { get; set; }
}
