using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class CancelCaseViewModel
    {
        public string? PatientFullName { get; set; }

        public int? CaseTagId { get; set; }

        /*public List<SelectListItem>*/

        public string? AdminCancellationNote { get; set; }
    }
}
