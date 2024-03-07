using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels
{
    public class DownloadRequest
    {
        public List<int> SelectedValues { get; set; } = new List<int>();
        public int RequestId { get; set; }

        public string? EmailValue { get; set; }
    }

}
