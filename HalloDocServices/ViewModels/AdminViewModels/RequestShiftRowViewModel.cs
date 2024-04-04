using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class RequestShiftRowViewModel
    {
        public int ShiftDetailId { get; set; }

        public int PhysicianId { get; set; }

        public string? Staff {  get; set; }

        public string? Day {  get; set; }
        
        public string? Time { get; set; }

        public string? RegionName { get; set; }

        public int RegionId { get; set; }
    }
}
