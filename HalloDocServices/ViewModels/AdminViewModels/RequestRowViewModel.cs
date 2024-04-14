using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class RequestRowViewModel
    {
        public int? DashboardRequestStatus { get; set; }

        public int? RequestStatus { get; set; }

        public int RequestId { get; set; }

        public int RequestType { get; set; }

        public string? PatientFullName { get; set; }

        public string? PatientEmail{ get; set; }

        public string? DateOfBirth { get; set; }

        public string? RequestorName { get; set; }

        public string? PhysicianName { get; set; }

        public string? DateOfService { get; set; }

        public string? RequestedDate { get; set; }

        public string? PatientPhoneNumber { get; set; }

        public string? SecondPhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Zipcode { get; set; }

        public int? Region { get; set; }

        public List<string>? Notes { get; set; }

        public string? CallType { get; set; }
    }
}
