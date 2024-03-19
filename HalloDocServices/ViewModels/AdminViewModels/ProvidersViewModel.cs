using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class ProvidersViewModel
    {
        public List<SelectListItem> RegionList { get; set; } = new List<SelectListItem>();

        public int? RegionId { get; set; }

        public List<ProviderRowViewModel> ProvidersList { get; set; } = new List<ProviderRowViewModel>();
    }
}
