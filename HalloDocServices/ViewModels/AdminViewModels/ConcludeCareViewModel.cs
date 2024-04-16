using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class ConcludeCareViewModel
    {
        public int RequestId { get; set; }

        public int PhysicianId { get; set; }

        public int? RequestClientId { get; set; }

        public string? PatientFullName { get; set; }

        public string? ConfirmationNumber { get; set; }

        public IEnumerable<IFormFile>? MultipleFiles { get; set; }

        public string? ProviderNotes { get; set; }

        public List<RequestFileViewModel> FileInfo { get; set; } = null!;


    }
}
