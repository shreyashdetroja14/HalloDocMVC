using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class UserAccessRow
    {
        public string AspNetUserId { get; set; } = null!;

        public int AdminId { get; set; }

        public int PhysicianId { get; set; }

        public int? AccountType { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Status { get; set; }

        public int? OpenRequests { get; set; }
    }
}
