using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class AssignCaseViewModel
    {
        public int? AdminId { get; set; }

        public int RequestId { get; set; }

        public bool? IsTransferRequest { get; set; }

        public Dictionary<int, string>? RegionList { get; set; }

        public Dictionary<int, string>? PhysicianList { get; set; }

        public int? RegionId { get; set; }

        public int? PhysicianId { get; set; }

        public string? Description { get; set; }
    }
}
