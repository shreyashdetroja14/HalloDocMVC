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
            List<RecordRowViewModel> RecordsList = _recordsService.GetRecordsList(SearchFilters);
            return PartialView("_RecordsTablePartial", RecordsList);
        }
    }
}
