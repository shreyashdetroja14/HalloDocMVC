


namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class CancelCaseViewModel
    {
        public int? AdminId { get; set; }

        public int RequestId { get; set; }

        public string? PatientFullName { get; set; }

        public int? CaseTagId { get; set; }

        public List<int>? CaseTagIds { get; set; }

        public List<string>? CaseTags { get; set; }

        public string? AdminCancellationNote { get; set; }

    }
}
