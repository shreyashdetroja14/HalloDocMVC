using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Repository.ViewModels
{
    public class PatientFormViewModel
    {
        public bool IsEditPatient { get; set; }

        public int PatientId { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [StringLength(100)]
        public string? LastName { get; set; }

        public int? DoctorId { get; set; }

        public int? Age { get; set; }

        [StringLength(100)]
        public string Email { get; set; } = null!;

        [StringLength(100)]
        public string? PhoneNumber { get; set; }

        public int? Gender { get; set; }

        public int? DiseaseKey { get; set; }

        public List<SelectListItem> DiseaseList = new List<SelectListItem>();

        public Dictionary<int, string> DiseaseDictionary { get; set;} = new Dictionary<int, string>();

        [StringLength(100)]
        public string? Specialist { get; set; }

        public List<SelectListItem> SpecialistList = new List<SelectListItem>();
    }
}
