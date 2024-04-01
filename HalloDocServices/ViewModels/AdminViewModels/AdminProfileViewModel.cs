using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class AdminProfileViewModel
    {
        public int AdminId { get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public bool? IsEditAdmin { get; set; }

        [Required(ErrorMessage = "Please enter Username")]
        public string Username { get; set; } = null!;

        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contain 1 number, 1 special charecter, 1 uppercase and 1 lowercase charecter")]
        public string? Password { get; set; }

        public int? Status { get; set; }

        [Required(ErrorMessage = "Please Set a Role")]
        public int? RoleId { get; set; }

        public List<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();

        public string? RoleName { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        [StringLength(100), MinLength(2, ErrorMessage = "Name can't be a single letter")]
        /*[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Firstname can only contain letters and spaces.")]*/
        public string FirstName { get; set; } = null!;

        /*[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Lastname can only contain letters and spaces.")]*/
        public string? LastName { get; set;}

        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,4}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Confirm Email")]
        [Compare(nameof(Email), ErrorMessage = "Emails do not match.")]
        public string? ConfirmEmail { get; set; }

        /*[RegularExpression(@"^0?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 09876543210 or 9876543210)")]*/
        [RegularExpression(@"^(?:0)?[6789]\d{4}(?:\s?\d{5})?$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 098765 43210, 9876543210)")]
        public string? PhoneNumber { get; set; }

        public List<int> AdminRegions { get; set; } = new List<int>();

        public string? Address1 { get; set; }

        public string? Address2 { get; set;}

        [Required(ErrorMessage = "Please Enter City")]
        [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "City name can only contain letters, spaces, hyphens, and apostrophes.")]
        public string? City { get; set;}

        [Required(ErrorMessage = "Please Select a State")]
        public int? RegionId { get; set; }

        public List<SelectListItem> StateList { get; set; } = new List<SelectListItem>();

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Please enter a valid 6-digit zip code.")]
        public string? ZipCode { get; set;}

        /*[RegularExpression(@"^0?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 01234567890 or 9876543210)")]*/
        [RegularExpression(@"^(?:0)?[6789]\d{4}(?:\s?\d{5})?$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 098765 43210, 9876543210)")]
        public string? SecondPhoneNumber { get; set; }
    }
}
