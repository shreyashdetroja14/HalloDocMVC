using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class AdminProfileViewModel
    {
        public int AdminId { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public int? Status { get; set; }

        public int? RoleId { get; set; }

        public string? RoleName { get; set; }

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set;}

        public string Email { get; set; } = null!;
        
        public string? ConfirmEmail { get; set; }

        public string? PhoneNumber { get; set; }

        public List<int> AdminRegions { get; set; } = new List<int>();

        public string? Address1 { get; set; }

        public string? Address2 { get; set;}

        public string? City { get; set;}

        public int? RegionId { get; set; }

        public List<SelectListItem> StateList { get; set; } = new List<SelectListItem>();

        public string? ZipCode { get; set;}

        public string? SecondPhoneNumber { get; set; }
    }
}
