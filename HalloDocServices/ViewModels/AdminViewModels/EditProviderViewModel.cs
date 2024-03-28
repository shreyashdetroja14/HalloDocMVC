using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class EditProviderViewModel
    {
        public bool IsCreateProvider { get; set; }

        public int ProviderId { get; set; }

        public int AdminId { get; set; }

        public string Username { get; set; } = null!;

        public string? Password { get; set; }

        public int? Status { get; set; }

        public List<SelectListItem> StatusList { get; set; } = new List<SelectListItem>();

        public int? RoleId { get; set; }

        public List<SelectListItem> RoleList { get; set; } = new List<SelectListItem>();

        public string? RoleName { get; set; }

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? MedicalLicense { get; set; }

        public string? NPINumber { get; set; }

        public string? SyncEmail { get; set; }

        public List<int> ProviderRegions { get; set; } = new List<int>();

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? City { get; set; }

        public int? RegionId { get; set; }

        public List<SelectListItem> StateList { get; set; } = new List<SelectListItem>();

        public string? ZipCode { get; set; }

        public string? SecondPhoneNumber { get; set; }

        public string BusinessName { get; set; } = null!;

        public string BusinessWebsite { get; set; } = null!;

        public IFormFile? Photo { get; set; }

        public IFormFile? Signature { get; set; }

        public string? SignaturePath { get; set; }

        public string? AdminNotes { get; set; }

        public bool? IsContractorDoc { get; set; }

        public bool? IsBackgroundDoc { get; set; }

        public bool? IsHippaDoc{ get; set;}

        public bool? IsNonDisclosureDoc{ get; set;}

        public bool? IsLicenseDoc{ get; set;}

        /*public IFormFile? UploadDoc { get; set; }*/
    }
}
