using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class VendorRowViewModel
    {
        public int VendorId { get; set; }

        public string VendorName { get; set;} = string.Empty;

        public int ProfessionId { get; set; }

        public string ProfessionName { get; set; } = string.Empty;

        public string? Email { get; set;}

        public string? FaxNumber { get; set;}

        public string? PhoneNumber { get; set;}

        public string? BusinessContact { get; set;}

    }
}
