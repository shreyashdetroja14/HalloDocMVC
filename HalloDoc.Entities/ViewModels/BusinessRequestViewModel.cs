namespace HalloDocMVC.ViewModels
{
    public class BusinessRequestViewModel
    {
        public string? BusinessFirstName { get; set; }

        public string? BusinessLastName { get; set;}

        public string? BusinessPhoneNumber { get; set;}

        public string? BusinessEmail { get; set;}

        public string BusinessName { get; set; } = null!;

        public int? BusinessCaseNumber { get; set;}

        public PatientRequestViewModel PatientInfo { get; set; } = null!;
    }
}
