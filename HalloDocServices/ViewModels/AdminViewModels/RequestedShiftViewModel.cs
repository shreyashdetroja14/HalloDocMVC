using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class RequestedShiftViewModel
    {
        public List<SelectListItem> RegionList { get; set; } = new List<SelectListItem>();
    }
}
