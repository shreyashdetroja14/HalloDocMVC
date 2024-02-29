using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class ViewNotesViewModel
    {

        public int? RequestId {  get; set; }

        public List<string>? TransferNotes { get; set; }

        public string? AdminNotes { get; set; }

        public string? AdminNotesInput { get; set; }

        public string? PhysicianNotes { get; set;}

        public string? AdminCancellationNotes { get;set; }

        public string? PhysicianCancellationNotes { get; set; }

        public string? PatientCancellationNotes { get; set; }


    }
}
