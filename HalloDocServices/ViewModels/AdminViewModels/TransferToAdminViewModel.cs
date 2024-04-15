using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class TransferToAdminViewModel
    {
        public int RequestId { get; set; }

        public int? AdminId { get; set; }

        public int? PhysicianId { get; set; }

        public string? PatientFullName { get; set; }

        public string? Description { get; set; }
    }
}
