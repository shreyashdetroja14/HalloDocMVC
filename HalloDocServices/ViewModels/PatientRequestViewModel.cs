﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels
{
    public class PatientRequestViewModel
    {
        [Required(ErrorMessage = "Please enter symptoms")]
        public string? Symptoms { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        [StringLength(100), MinLength(2, ErrorMessage = "Name can't be a single letter")]
        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string? DOB { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string Email { get; set; } = null!;

        public string? EmailToken { get; set; }


        /*[RegularExpression(@"^0?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 01234567890 or 9876543210)")]*/
        [RegularExpression(@"^(?:0)?[6789]\d{4}(?:\s?\d{5})?$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 098765 43210, 9876543210)")]
        public string? PhoneNumber { get; set; }

        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        /*[RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contain 1 number, 1 special charecter, 1 uppercase and 1 lowercase charecter")]*/
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must contain atleast one number, special character, uppercase-lowercase character.")]

        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        public string? Street { get; set; }

        /*[Required(ErrorMessage = "Please Enter City")]*/
        [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "City name can only contain letters, spaces, hyphens, and apostrophes.")]
        public string? City { get; set; }

        /*[Required(ErrorMessage = "Region/State is required")]
        [AllowedStates(ErrorMessage = "Invalid state.")]*/
        public string? State { get; set; }

        [Required(ErrorMessage = "Please Select a Region")]
        public int RegionId { get; set; }

        public List<SelectListItem> RegionList { get; set; } = new List<SelectListItem>();

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Please enter a valid 6-digit zip code.")]
        public string? ZipCode { get; set; }

        public string? Room { get; set; }

        public string? File { get; set; }

        public IEnumerable<IFormFile>? MultipleFiles { get; set; }

        public string? Notes { get; set; }

        public int CreatedBy { get; set; }

        public string? CreatorAspId { get; set; }

        public string? CreatorRole { get; set; }
    }

    /*public class AllowedStatesAttribute : ValidationAttribute
    {
        private readonly string[] _allowedStates = { "Gujarat", "Maharashtra", "Madhya Pradesh", "Uttar Pradesh", "Rajasthan" };

        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value is string state)
            {
                string lowercaseState = state.ToLowerInvariant();
                if (!_allowedStates.Any(allowedState => allowedState.ToLowerInvariant() == lowercaseState))
                {
                    return new ValidationResult("Invalid state. Please select a valid option from the list.");
                }
            }

            return ValidationResult.Success;
        }
    }*/

}
