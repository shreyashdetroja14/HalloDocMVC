using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class AdminDashboardViewModel : BaseViewModel
    {
        public int? RequestStatus { get; set; }

        public int? NewRequestCount { get; set; }

        public int? PendingRequestCount { get; set;}

        public int? ActiveRequestCount { get; set;}

        public int? ConcludeRequestCount { get; set; }

        public int? ToCloseRequestCount { get; set; }

        public int? UnpaidRequestCount { get; set; }

        public List<SelectListItem> RegionList { get; set; } = new List<SelectListItem>();

        public List<RequestRowViewModel>? RequestRows { get; set; }

        public SendLinkViewModel SendLinkData { get; set; } = new SendLinkViewModel();

        public string? SupportMessage { get; set; }
    }
}
