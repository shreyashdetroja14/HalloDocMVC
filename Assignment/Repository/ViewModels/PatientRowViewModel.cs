using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class PatientRowViewModel
    {
        public int PatientId { get; set; }

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string Email { get; set; } = null!;
        
        public int? Age { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        public string? Disease { get; set; }

        public string? Doctor { get; set; }

    }
}
