using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class MDsOnCallViewModel
    {
        public int RegionId { get; set; }

        public List<SelectListItem> RegionList = new List<SelectListItem>();
    }
}
