using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class PagerViewModel
    {
        public int TotalItems { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public int StartPage { get; set; }

        public int EndPage { get; set; }

        public PagerViewModel() { }

        public PagerViewModel(int totalItems, int page, int pageSize = 5)
        {
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int currentPage = page;

            int startPage = currentPage - 2;
            int endPage = currentPage + 2;

            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 5)
                {
                    startPage = endPage - 4;
                }
            }
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            StartPage = startPage;
            EndPage = endPage;
            TotalPages = totalPages;
        }
    }
}
