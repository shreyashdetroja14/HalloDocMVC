using System;
using System.Collections.Generic;
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

        public List<RequestRowViewModel>? RequestRows { get; set; }
    }
}
