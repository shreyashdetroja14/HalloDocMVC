using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class AccountAccessViewModel
    {
        public int RoleId { get; set; }

        public string? RoleName { get; set; }

        public string? AccountType { get; set; }
    }
}
