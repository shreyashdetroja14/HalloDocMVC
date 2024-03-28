using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class EditProviderViewModel
    {
        public bool IsCreateProvider { get; set; }

        public int ProviderId { get; set; }

        public int AdminId { get; set; }

        [Required(ErrorMessage = "Please enter Username")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage ="Please enter password.")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "Password must contain 1 number, 1 special charecter, 1 uppercase and 1 lowercase charecter")]
        public string? Password { get; set; }

        public int? Status { get; set; }

        public List<SelectListItem> StatusList { get; set; } = new List<SelectListItem>();

        public int? RoleId { get; set; }

        public List<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();

        public string? RoleName { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        [StringLength(100), MinLength(2, ErrorMessage = "Name can't be a single letter")]
        /*[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Firstname can only contain letters and spaces.")]*/
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Last Name")]
        /*[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Lastname can only contain letters and spaces.")]*/
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string Email { get; set; } = null!;

        /*[RegularExpression(@"^0?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 01234567890 or 9876543210)")]*/
        [RegularExpression(@"^(?:0)?[6789]\d{4}(?:\s?\d{5})?$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 098765 43210, 9876543210)")]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^MCI-[A-Z][0-9]{7}$", ErrorMessage = "Please enter a valid MCI registration number.")]
        public string? MedicalLicense { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit NPI number.")]
        public string? NPINumber { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string? SyncEmail { get; set; }

        public List<int> ProviderRegions { get; set; } = new List<int>();

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        [Required(ErrorMessage = "Please Enter City")]
        [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "City name can only contain letters, spaces, hyphens, and apostrophes.")]
        public string? City { get; set; }

        public int? RegionId { get; set; }

        public List<SelectListItem> StateList { get; set; } = new List<SelectListItem>();

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Please enter a valid 6-digit zip code.")]
        public string? ZipCode { get; set; }

        /*[RegularExpression(@"^0?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 01234567890 or 9876543210)")]*/
        [RegularExpression(@"^(?:0)?[6789]\d{4}(?:\s?\d{5})?$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 098765 43210, 9876543210)")]
        public string? SecondPhoneNumber { get; set; }

        [Required(ErrorMessage ="Please Enter Business Name")]
        public string BusinessName { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Business Website")]
        [RegularExpression(@"^(http|https)://([\w\-_]+\.)+[^\s]{2,}$", ErrorMessage = "Please enter a valid website URL with a protocol (http or https).")]
        public string BusinessWebsite { get; set; } = null!;

        public IFormFile? Photo { get; set; }

        public IFormFile? Signature { get; set; }

        public string? SignaturePath { get; set; }

        public string? AdminNotes { get; set; }

        public bool? IsContractorDoc { get; set; }

        public bool? IsBackgroundDoc { get; set; }

        public bool? IsHippaDoc{ get; set;}

        public bool? IsNonDisclosureDoc{ get; set;}

        public bool? IsLicenseDoc{ get; set;}

        public IFormFile? ContractorDoc { get; set; }

        public IFormFile? BackgroundDoc { get; set; }

        public IFormFile? HippaDoc { get; set; }

        public IFormFile? NonDisclosureDoc { get; set; }

        public IFormFile? LicenseDoc { get; set; }

        public string? CreatedBy { get; set; }
    }
}
