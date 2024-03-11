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
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
