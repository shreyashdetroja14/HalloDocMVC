using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class ViewNotesViewModel
    {
        public List<string>? TransferNotes { get; set; }

        public string? AdminNotes { get; set; }

        public string? PhysicianNotes { get; set;}
    }
}
