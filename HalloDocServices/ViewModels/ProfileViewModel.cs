using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels
{
    public class ProfileViewModel
    {
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        [StringLength(100), MinLength(2, ErrorMessage = "Name can't be a single letter")]
        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string DOB { get; set; } = null!;

        [RegularExpression(@"^0?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 01234567890 or 9876543210)")]
        public string? PhoneNumber { get; set; }

        public int? PhoneNumberType { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string Email { get; set; } = null!;

        public string? Street { get; set; }

        public string? City { get; set; }

        [Required(ErrorMessage = "Region/State is required")]
        [AllowedStates(ErrorMessage = "Invalid state.")]
        public string? State { get; set; }

        [RegularExpression(@"^\d{6}$", ErrorMessage = "Please enter a valid 6-digit zip code.")]
        public string? ZipCode { get; set; }
    }
}
