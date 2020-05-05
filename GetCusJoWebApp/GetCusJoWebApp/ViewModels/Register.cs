﻿using System.ComponentModel.DataAnnotations;

namespace GetCusJoWebApp.ViewModels
{
    public class Register
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match")]

        public string ConfirmPassword { get; set; }
    }
}
