using HalloDocServices.Implementation;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

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

        public async Task<IActionResult> DeleteRecord(int requestId)
        {
            bool idRecordDeleted = await _recordsService.DeleteRecord(requestId);
            if (idRecordDeleted)
            {
                TempData["SuccessMessage"] = "Record Deleted Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Delete Record.";
            }

            return RedirectToAction("SearchRecords");
        }

        [HttpPost]
        public IActionResult ExportRecords(SearchRecordsViewModel SearchFilters)
        {
            var excelFile = _recordsService.ExportToExcel(SearchFilters);

            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "records.xlsx");
        }

        public IActionResult PatientHistory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetPatientList(SearchRecordsViewModel SearchFilters)
        {
            if (SearchFilters.PageNumber <= 0)
            {
                SearchFilters.PageNumber = 1;
            }

            PaginatedListViewModel<PatientRowViewModel> PaginatedList = new PaginatedListViewModel<PatientRowViewModel>();
            PaginatedList = _recordsService.GetPatientList(SearchFilters);

            ViewBag.PagerData = PaginatedList.PagerData;
            return PartialView("_PatientHistoryPartial", PaginatedList.DataRows);
        }

        public IActionResult PatientRecord(int userId)
        {   PatientRowViewModel UserIdModel = new PatientRowViewModel();
            UserIdModel.UserId = userId;
            return View(UserIdModel);
        }

        public IActionResult GetPatientRecordList(int userId, int pageNumber)
        {
            if(pageNumber <= 0)
            {
                pageNumber = 1;
            }

            PaginatedListViewModel<PatientRowViewModel> PaginatedList = new PaginatedListViewModel<PatientRowViewModel>();
            PaginatedList = _recordsService.GetPatientRecordList(userId, pageNumber);

            ViewBag.PagerData = PaginatedList.PagerData;
            return PartialView("_PatientRecordPartial", PaginatedList.DataRows);
        }

        public IActionResult BlockedHistory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetBlockedList(SearchRecordsViewModel SearchFilters)
        {
            if (SearchFilters.PageNumber <= 0)
            {
                SearchFilters.PageNumber = 1;
            }

            PaginatedListViewModel<PatientRowViewModel> PaginatedList = new PaginatedListViewModel<PatientRowViewModel>();
            PaginatedList = _recordsService.GetBlockedList(SearchFilters);

            ViewBag.PagerData = PaginatedList.PagerData;
            return PartialView("_BlockedHistoryPartial", PaginatedList.DataRows);
        }
    }
}
