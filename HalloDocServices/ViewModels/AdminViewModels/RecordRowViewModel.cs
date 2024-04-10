using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class RecordRowViewModel
    {
        public int RequestId { get; set; }

        public string? PatientName { get; set; }

        public string? Requestor { get; set; }

        public string? DateOfService { get; set; }

        public string? CloseCaseDate { get;set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set;}

        public string? ZipCode { get; set;}

        public string? RequestStatus { get; set;}

        public string? PhysicianName { get; set;}

        public string? PhysicianNote { get;set; }

        public string? CancelledByProviderNote { get;set; }

        public string? AdminNote { get;set; }

        public string? PatientNote { get;set; }

        public string? CancellationReason { get;set; }
    }
}
