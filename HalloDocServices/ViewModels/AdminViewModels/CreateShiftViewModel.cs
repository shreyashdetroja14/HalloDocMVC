using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class CreateShiftViewModel
    {
        public int RegionId { get; set; }

        public List<SelectListItem> RegionList { get; set; } = new List<SelectListItem>();

        public int PhysicianId { get; set; }

        public List<SelectListItem> PhysicianList { get; set;} = new List<SelectListItem>();

        public string ShiftDate { get; set; } = string.Empty;

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public bool IsRepeat { get; set; }

        public List<int> RepeatDays { get; set; } = new List<int>();

        public int RepeatUpto {  get; set; }

        public string CreatedBy { get; set; } = string.Empty;
    }
}
