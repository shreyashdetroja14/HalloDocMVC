using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class SearchRecordsViewModel
    {
        public int? RequestStatus { get; set; }

        public string? PatientName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public int? RequestType { get; set; }

        public string? FromDateOfService { get; set; }

        public string? ToDateOfService { get; set; }

        public string? CreatedDate { get; set; }

        public string? ProviderName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set;}

        public int? AccountType { get; set; }

        public string? ReceiverName { get; set; }

        public string? SentDate { get; set; }

        public int PageNumber { get; set; }
    }
}
