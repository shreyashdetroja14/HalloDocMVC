using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels
{
    public class BusinessRequestViewModel
    {
        [StringLength(100), MinLength(2, ErrorMessage = "Name can't be a single letter")]
        public string? BusinessFirstName { get; set; }

        public string? BusinessLastName { get; set;}

        [RegularExpression(@"^0?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 01234567890 or 9876543210)")]
        public string? BusinessPhoneNumber { get; set;}

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string? BusinessEmail { get; set;}

        [Required(ErrorMessage = "Please Enter Business Name")]
        public string BusinessName { get; set; } = null!;

        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter a valid case number.")]
        public int? BusinessCaseNumber { get; set;}

        public PatientRequestViewModel PatientInfo { get; set; } = null!;
    }
}
