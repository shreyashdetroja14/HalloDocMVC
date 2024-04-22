﻿using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Password")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contain 1 number, 1 special charecter, 1 uppercase and 1 lowercase charecter")]
        public string Password { get; set; } = null!;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
    }
}
