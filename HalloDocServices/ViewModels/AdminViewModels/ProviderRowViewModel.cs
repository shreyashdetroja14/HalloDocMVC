using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class ProviderRowViewModel
    {
        public int ProviderId { get; set; }

        public string? ProviderName { get; set;}

        public bool? IsNotificationStopped { get; set; }

        public string? Role { get ; set; }

        public string? OnCallStatus { get; set;}

        public string? Status { get; set;}
    }
}
