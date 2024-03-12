using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class ViewCaseViewModel
    {
        public int RequestId { get; set; }

        public int? Status { get; set; }

        public int? RequestType { get; set; }

        public string? ConfirmationNumber { get; set; }

        public string? Symptoms { get; set; }

        public string? FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string? DOB { get; set; }

        public string? Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Region { get; set; }

        public string? BusinessNameOrAddress { get; set; }

        public string? Room { get; set; }


    }
}
