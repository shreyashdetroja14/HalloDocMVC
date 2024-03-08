using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class OrdersViewModel
    {
        public int RequestId { get; set; }

        public int? ProfessionId { get; set; }

        /*public Dictionary<int, string> ProfessionList { get; set;} = new Dictionary<int, string>();*/

        public int? VendorId { get; set; }

        /*public Dictionary<int, string> VendorList { get; set;} = new Dictionary<int, string>();*/

        public string? BusinessContact { get; set; }

        public string? Email { get; set; }

        public string? FaxNumber { get; set; }

        public string? OrderDetails { get; set; }

        public int? NumberOfRefills { get; set; }
    }
}
