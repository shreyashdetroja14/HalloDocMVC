using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class EventViewModel
    {
        public int ShiftDetailId { get; set; }

        public int PhysicianId { get; set; }

        public string PhysicianName { get; set; } = string.Empty;

        public string ShiftDate { get; set; } = string.Empty;

        public string StartTime { get; set; } = string.Empty;

        public string EndTime { get; set; } = string.Empty;

        public string ShiftRegion {  get; set; } = string.Empty;

        public bool IsApproved { get; set; }
    }
}
