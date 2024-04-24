using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class PaginatedList<T>
    {
        public PagerViewModel PagerData { get; set; } = new PagerViewModel();

        public List<T> DataRows { get; set; } = new List<T>();
    }
}
