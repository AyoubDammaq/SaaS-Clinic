﻿using AuthentificationService.Entities;
using System.ComponentModel.DataAnnotations;

namespace AuthentificationService.Models
{
    public class UserDto
    {
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Patient;
    }
}
