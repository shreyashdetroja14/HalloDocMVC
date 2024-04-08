using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class MDCardViewModel
    {
        public int PhysicianId { get; set; }

        public string? FullName { get; set; }

        public string? ProfilePhotoPath { get; set; }

        public string? OnCallStatus { get; set; }
    }
}
