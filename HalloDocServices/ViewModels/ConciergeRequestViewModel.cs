namespace HalloDocServices.ViewModels
{
    public class ConciergeRequestViewModel
    {
        public string? ConciergeFirstName { get; set; }

        public string? ConciergeLastName { get; set; }

        public string? ConciergePhoneNumber { get; set; }

        public string ConciergeEmail { get; set; } = null!;

        public string ConciergePropertyName { get; set; } = null!;

        public string ConciergeStreet { get; set; } = null!;

        public string ConciergeCity { get; set; } = null!;

        public string ConciergeState { get; set; } = null!;

        public string ConciergeZipCode { get; set; } = null!;

        public PatientRequestViewModel PatientInfo { get; set; } = null!;
    } 
}
