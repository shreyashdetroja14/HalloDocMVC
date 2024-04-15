using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class ViewNotesViewModel
    {
        public int? AdminId { get; set; }

        public int RequestId {  get; set; }

        public List<string>? TransferNotes { get; set; }

        public string? AdminNotes { get; set; }

        [Required(ErrorMessage = "Note cannot be empty")]
        public string? AdminNotesInput { get; set; }

        public string? PhysicianNotes { get; set;}

        [Required(ErrorMessage = "Note cannot be empty")]
        public string? PhysicianNotesInput { get; set;}

        public string? AdminCancellationNotes { get;set; }

        public string? PhysicianCancellationNotes { get; set; }

        public string? PatientCancellationNotes { get; set; }

        public bool IsPhysician { get; set; }

        public string? CreatedBy { get; set; }
    }
}
