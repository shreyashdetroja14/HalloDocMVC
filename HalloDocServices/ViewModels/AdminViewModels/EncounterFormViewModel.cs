using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class EncounterFormViewModel
    {
        public int EncounterFormId { get; set; }

        public int RequestId { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        [StringLength(100), MinLength(2, ErrorMessage = "Name can't be a single letter")]
        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string? Location { get; set; }

        public string? DOB { get; set; }

        public string? ServiceDate { get; set; }

        [RegularExpression(@"^(?:0)?[6789]\d{4}(?:\s?\d{5})?$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 098765 43210, 9876543210)")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string Email { get; set; } = null!;

        public string? PresentIllnessHistory { get; set; }

        public string? MedicalHistory { get; set; }

        public string? Medications { get; set; }

        public string? Allergies { get; set; }

        public string? Temperature { get; set; }

        public string? HeartRate { get; set; }

        public string? RespirationRate { get; set; }

        public string? BloodPressureSystolic { get; set; }

        public string? BloodPressureDiastolic { get; set; }

        public string? OxygenLevel { get; set; }

        public string? Pain { get; set; }

        public string? Heent { get; set; }

        public string? Cardiovascular { get; set; }

        public string? Chest { get; set; }

        public string? Abdomen { get; set; }

        public string? Extremities { get; set; }

        public string? Skin { get; set; }

        public string? Neuro { get; set; }

        public string? Other { get; set; }

        [Required(ErrorMessage = "Please enter a diagnosis")]
        public string? Diagnosis { get; set; }

        [Required(ErrorMessage = "Please enter a treatment plan")]
        public string? TreatmentPlan { get; set; }

        [Required(ErrorMessage = "Please enter medications dispensed")]
        public string? MedicationsDispensed { get; set; }

        [Required(ErrorMessage = "Please enter a procedure")]
        public string? Procedures { get; set; }

        [Required(ErrorMessage = "Please enter follow-up details")]
        public string? FollowUp { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool? IsFinalized { get; set; }

        public DateTime? FinalizedDate { get; set; }

        public bool IsPhysician { get; set; }

        public string? UserRole { get; set; }
    }
}
