using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers
{
    public class RecordsController : Controller
    {
        private readonly IRecordsService _recordsService;

        public RecordsController(IRecordsService recordsService)
        {
            _recordsService = recordsService;
        }
        public IActionResult SearchRecords()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetRecordsList(SearchRecordsViewModel SearchFilters)
        {
            if(SearchFilters.PageNumber <= 0)
            {
                SearchFilters.PageNumber = 1;
            }

            PaginatedListViewModel<RecordRowViewModel> PaginatedList = new PaginatedListViewModel<RecordRowViewModel>();
            PaginatedList = _recordsService.GetRecordsList(SearchFilters);

            ViewBag.PagerData = PaginatedList.PagerData;
            return PartialView("_RecordsTablePartial", PaginatedList.DataRows);
        }
    }
}
