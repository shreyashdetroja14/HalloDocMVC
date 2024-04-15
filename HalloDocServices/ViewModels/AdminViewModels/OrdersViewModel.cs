using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class OrdersViewModel
    {
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Please select a profession")]
        public int? ProfessionId { get; set; }

        public List<SelectListItem> ProfessionList { get; set; } = new List<SelectListItem>();

        /*public Dictionary<int, string> ProfessionList { get; set;} = new Dictionary<int, string>();*/

        [Required(ErrorMessage = "Please select a profession")]
        public int? VendorId { get; set; }

        /*public Dictionary<int, string> VendorList { get; set;} = new Dictionary<int, string>();*/

        [RegularExpression(@"^0?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 01234567890 or 9876543210)")]
        public string? BusinessContact { get; set; }

        
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string? Email { get; set; }

        [RegularExpression(@"^+?[0-9]+$", ErrorMessage = "Please enter a valid fax number")]
        public string? FaxNumber { get; set; }

        public string? OrderDetails { get; set; }

        [Required(ErrorMessage = "Please select refill amount")]
        public int? NumberOfRefills { get; set; }
    }
}
