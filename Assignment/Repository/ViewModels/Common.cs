using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class Common
    {
        public Dictionary<int, string> Diseases = new Dictionary<int, string>
        {
            {1, "Heart Disease" },
            {2, "Brain Disease" },
            {3, "Joint Pain" },
            {4, "Gynacology" },
            {5, "Surgery" },
            {6, "Fever/Flu" },
            {7, "Dental" },
        };
        
    }
}
