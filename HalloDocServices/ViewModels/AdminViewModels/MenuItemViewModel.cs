    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class MenuItemViewModel
    {
        public int MenuId { get; set; }

        public string? Name { get; set; }

        public int? AccountType { get; set; }

        public bool IsChecked { get; set; }
    }
}
