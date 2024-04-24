using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class SendAgreementViewModel
    {
        public int RequestId { get; set; }

        public int RequestType{ get; set; }

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool? IsAgreementSent { get; set; }

        public int? AdminId { get; set; }

        public int? PhysicianId { get; set; }
    }
}
