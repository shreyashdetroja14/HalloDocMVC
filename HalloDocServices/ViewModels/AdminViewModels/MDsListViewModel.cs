using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class MDsListViewModel
    {
        public List<MDCardViewModel> UnavailableMDs { get; set; } = new List<MDCardViewModel>();

        public List<MDCardViewModel> AvailableMDs { get; set; } = new List<MDCardViewModel>();
    }
}
