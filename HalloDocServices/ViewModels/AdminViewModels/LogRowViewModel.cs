using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class LogRowViewModel
    {
        public int EmailLogId { get; set; }

        public int SMSLogId { get; set; }

        public string? RecipientName { get; set; }

        public string? Action { get; set; }

        public string? RoleName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CreatedDate { get; set; }
        
        public string? SentDate { get; set; }

        public bool IsSent { get; set; }

        public int? SentTries { get; set; }

        public string? ConfirmationNumber { get; set; }

        public bool IsSMSLog { get; set; }

        public bool IsEmailLog { get; set; }
    }
}
