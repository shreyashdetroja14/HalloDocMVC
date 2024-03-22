using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels
{
    public class ConciergeRequestViewModel
    {
        [StringLength(100), MinLength(2, ErrorMessage = "Name can't be a single letter")]
        public string? ConciergeFirstName { get; set; }

        public string? ConciergeLastName { get; set; }

        [RegularExpression(@"^0?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 01234567890 or 9876543210)")]
        public string? ConciergePhoneNumber { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string ConciergeEmail { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Property Name")]
        public string ConciergePropertyName { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Street Name")]
        public string ConciergeStreet { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter City")]
        [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "City name can only contain letters, spaces, hyphens, and apostrophes.")]
        public string ConciergeCity { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter State")]
        [AllowedStates(ErrorMessage = "Invalid state.")]
        public string ConciergeState { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Zipcode")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Please enter a valid 6-digit zip code.")]
        public string ConciergeZipCode { get; set; } = null!;

        public PatientRequestViewModel PatientInfo { get; set; } = null!;
    }

}
