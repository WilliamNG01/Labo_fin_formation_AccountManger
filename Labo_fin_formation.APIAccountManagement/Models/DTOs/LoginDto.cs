﻿using System.ComponentModel.DataAnnotations;

namespace Labo_fin_formation.APIAccountManagement.Models.DTOs
{
    public class LoginDto
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
