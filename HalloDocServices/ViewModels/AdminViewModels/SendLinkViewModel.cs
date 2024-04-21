using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class SendLinkViewModel
    {
        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        [StringLength(100), MinLength(2, ErrorMessage = "Name can't be a single letter")]
        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        [RegularExpression(@"^(?:0)?[6789]\d{4}(?:\s?\d{5})?$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 098765 43210, 9876543210)")]
        public string? PhoneNumber { get; set; }

        public int? AdminId { get; set; }

        public int? PhysicianId { get; set; }
    }
}
