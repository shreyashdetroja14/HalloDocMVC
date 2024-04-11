using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class PatientRowViewModel
    {
        public int UserId { get; set; }

        public int RequestId { get; set; }

        public int BlockedRequestId { get; set; }

        public int? EncounterFormId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? CreatedDate { get; set; }

        public string? ConfirmationNumber { get; set; }

        public string? ProviderName { get; set;}

        public string? ConcludedDate { get; set;}

        public string? Status { get; set;}

        public bool? IsActive { get; set; }

        public string? BlockedReason { get; set; }

    }
}
