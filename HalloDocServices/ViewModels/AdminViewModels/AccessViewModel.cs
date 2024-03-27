using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class AccessViewModel
    {
        public int AdminId { get; set; }

        public List<AccountAccessViewModel> AccountList { get; set; } = new List<AccountAccessViewModel>();
    }
}
