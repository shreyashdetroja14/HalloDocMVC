using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class CreateVendorViewModel
    {
        public bool IsEditVendor { get; set; }

        public int VendorId { get; set; }

        [Required(ErrorMessage = "Please Enter Vendor Name")]
        [StringLength(100), MinLength(2, ErrorMessage = "Vendor name can't be a single letter")]
        public string VendorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please Select Profession")]
        public int ProfessionId { get; set; }

        public List<SelectListItem> ProfessionList { get; set; } = new List<SelectListItem>();

        [RegularExpression(@"^[\d ]*$", ErrorMessage = "Fax number can only contain numbers")]
        public string? FaxNumber { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string? Email { get; set; }

        [RegularExpression(@"^(?:0)?[6789]\d{4}(?:\s?\d{5})?$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 098765 43210, 9876543210)")]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^[\d ]*$", ErrorMessage = "Business Contact can only contain numbers")]
        public string? BusinessContact { get; set; }

        public string? Street { get; set; }

        [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "City name can only contain letters, spaces, hyphens, and apostrophes.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Please Select a State")]
        public int RegionId { get; set; }

        public List<SelectListItem> RegionList { get; set; } = new List<SelectListItem>();

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Please enter a valid 6-digit zip code.")]
        public string? ZipCode { get; set; }


    }
}
