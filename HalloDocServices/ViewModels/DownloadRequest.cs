using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels
{
    public class DownloadRequest
    {
        public List<string> SelectedValues { get; set; } = new List<string>();
        public int RequestId { get; set; }
    }

}
