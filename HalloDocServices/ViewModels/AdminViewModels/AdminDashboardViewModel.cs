using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class AdminDashboardViewModel
    {
        public int? RequestStatus { get; set; }

        public int? NewRequestCount { get; set; }

        public int? PendingRequestCount { get; set;}

        public int? ActiveRequestCount { get; set;}

        public int? ConcludeRequestCount { get; set; }

        public int? ToCloseRequestCount { get; set; }

        public int? UnpaidRequestCount { get; set; }

        [Required(ErrorMessage = "Please Enter Email Address")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string? Email { get; set; }

        public List<RequestRowViewModel>? RequestRows { get; set; }
    }
}
