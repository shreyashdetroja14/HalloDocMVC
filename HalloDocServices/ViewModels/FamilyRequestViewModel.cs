
namespace HalloDocServices.ViewModels
{
    public class FamilyRequestViewModel
    {
        public string? FamilyFirstName { get; set; }

        public string? FamilyLastName { get; set; }

        public string? FamilyPhoneNumber { get; set; }

        public string? FamilyEmail { get; set; } 

        public string? FamilyRelation { get; set; }

        public PatientRequestViewModel PatientInfo { get; set; } = null!;

    }
}
