using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class CreateShiftViewModel
    {

        public int ShiftDetailId { get; set; }

        [Required(ErrorMessage = "Please Select Region")]
        public int RegionId { get; set; }

        public List<SelectListItem> RegionList { get; set; } = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Select Physician")]
        public int PhysicianId { get; set; }

        public List<SelectListItem> PhysicianList { get; set;} = new List<SelectListItem>();

        [Required(ErrorMessage = "Please Select Shift Date")]
        public string ShiftDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please Select Start Time")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "Please Select End Time")]
        public TimeOnly EndTime { get; set; }

        public bool IsRepeat { get; set; }

        public List<int> RepeatDays { get; set; } = new List<int>();

        [Required(ErrorMessage = "Please Select Repeat Times")]
        public int RepeatUpto {  get; set; }

        public string CreatedBy { get; set; } = string.Empty;

        public string ModifiedBy { get; set; } = string.Empty;
    }
}
