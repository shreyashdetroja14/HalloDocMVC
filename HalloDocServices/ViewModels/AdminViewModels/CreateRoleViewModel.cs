using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class CreateRoleViewModel
    {
        public string? CreatedBy { get; set; }

        [Required(ErrorMessage ="Please Enter Role Name")]
        public string? RoleName { get; set; }

        [Required(ErrorMessage ="Please Select Account Type")]
        public int? AccountType { get; set;}

        public List<SelectListItem> AccountTypeList { get; set; } = new List<SelectListItem>();

        public List<int> MenuIds { get; set; } = new List<int>();

        public List<MenuItemViewModel> MenuItems { get; set; } = new List<MenuItemViewModel>();
    }
}
