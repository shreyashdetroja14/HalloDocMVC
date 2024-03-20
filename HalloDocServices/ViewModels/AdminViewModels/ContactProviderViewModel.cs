using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class ContactProviderViewModel
    {
        public int ProviderId { get; set; }

        public string? ProviderName { get; set; }

        public string? ProviderEmail { get; set; }

        public string? CommunicationType { get; set; }

        public string? Subject { get; set; }

        public string? Message { get; set; }
    }
}
