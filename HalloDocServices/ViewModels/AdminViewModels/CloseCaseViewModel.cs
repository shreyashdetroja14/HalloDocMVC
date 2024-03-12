using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class CloseCaseViewModel
    {
        public ViewDocumentsViewModel ViewUploads { get; set; } = new ViewDocumentsViewModel();

        public ViewCaseViewModel ViewCase { get; set; } = new ViewCaseViewModel();
    }
}
