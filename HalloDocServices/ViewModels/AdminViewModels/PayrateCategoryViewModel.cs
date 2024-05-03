using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class PayrateCategoryViewModel
    {
        public int PayrateId { get; set; }

        public int PhysicianId { get; set; }

        public int PayrateCategoryId { get; set; }

        public int Payrate { get; set; }

        public string? CategoryName { get; set; }
    }
}
