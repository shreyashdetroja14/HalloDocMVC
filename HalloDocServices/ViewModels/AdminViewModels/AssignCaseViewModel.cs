using Microsoft.AspNetCore.Mvc.Rendering;
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

        public List<SelectListItem> RegionList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> PhysicianList { get; set; } = new List<SelectListItem>();

        public int? RegionId { get; set; }

        public int? PhysicianId { get; set; }

        public string? Description { get; set; }
    }
}
