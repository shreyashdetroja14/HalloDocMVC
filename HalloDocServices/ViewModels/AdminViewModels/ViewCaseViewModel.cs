using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class ViewCaseViewModel
    {
        public int RequestId { get; set; }

        public int? Status { get; set; }

        public int? RequestType { get; set; }

        public string? ConfirmationNumber { get; set; }

        public string? Symptoms { get; set; }

        public string? FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string? DOB { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string? Email { get; set; } = null!;

        [RegularExpression(@"^(?:0)?[6789]\d{4}(?:\s?\d{5})?$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 098765 43210, 9876543210)")]
        public string? PhoneNumber { get; set; }

        public string? Region { get; set; }

        public string? BusinessNameOrAddress { get; set; }

        public string? Room { get; set; }

        public bool IsPhysician { get; set; }
    }
}
