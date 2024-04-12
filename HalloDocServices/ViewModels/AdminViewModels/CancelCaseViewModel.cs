


using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class CancelCaseViewModel
    {
        public int? AdminId { get; set; }

        public int RequestId { get; set; }

        public string? PatientFullName { get; set; }

        [Required(ErrorMessage ="Please enter id")]
        public int CaseTagId { get; set; }

        public List<int>? CaseTagIds { get; set; }

        public List<string>? CaseTags { get; set; }

        [Required(ErrorMessage = "Please provide a reason")]
        public string AdminCancellationNote { get; set; } = string.Empty;

    }
}
