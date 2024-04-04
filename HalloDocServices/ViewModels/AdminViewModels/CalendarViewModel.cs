using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class CalendarViewModel
    {
        public List<ResourceViewModel> Resources { get; set; } = new List<ResourceViewModel>();

        public List<EventViewModel> Events { get; set; } = new List<EventViewModel>();
    }
}
