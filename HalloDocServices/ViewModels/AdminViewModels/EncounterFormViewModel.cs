using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class EncounterFormViewModel
    {
        public int EncounterFormId { get; set; }

        public int RequestId { get; set; }

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string? Location { get; set; }

        public string? DOB { get; set; }

        public string? ServiceDate { get; set; }

        public string? PhoneNumber { get; set; }

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

        public string? Diagnosis { get; set; }

        public string? TreatmentPlan { get; set; }

        public string? MedicationsDispensed { get; set; }

        public string? Procedures { get; set; }

        public string? FollowUp { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool? IsFinalized { get; set; }

        public DateTime? FinalizedDate { get; set; }

        public bool IsPhysician { get; set; }

        public string? UserRole { get; set; }
    }
}
