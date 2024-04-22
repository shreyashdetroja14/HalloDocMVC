using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class DownloadFormViewModel
    {
        public int EncounterFormId { get; set; }

        public int RequestId { get; set; }

        public int? PhysicianId { get; set; }
    }
}
