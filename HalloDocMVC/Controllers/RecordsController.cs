using DocumentFormat.OpenXml.InkML;
using HalloDocEntities.Models;
using HalloDocMVC.Auth;
using HalloDocServices.Implementation;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace HalloDocMVC.Controllers
{
    [CustomAuthorize("admin")]
    public class RecordsController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly IRecordsService _recordsService;

        public RecordsController(IJwtService jwtService, IRecordsService recordsService)
        {
            _jwtService = jwtService;
            _recordsService = recordsService;
        }

        #region JWT TOKEN DATA

        public ClaimsData GetClaimsData()
        {
            ClaimsData claimsData = new ClaimsData();

            string token = Request.Cookies["jwt"] ?? "";

            if (_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                claimsData.AspNetUserId = jwtToken.Claims.FirstOrDefault(x => x.Type == "aspnetuserId")?.Value;
                claimsData.Email = jwtToken?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                claimsData.AspNetUserRole = jwtToken?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                claimsData.Username = jwtToken?.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
                claimsData.Id = int.Parse(jwtToken?.Claims.FirstOrDefault(x => x.Type == "id")?.Value ?? "");
            }

            return claimsData;
        }

        #endregion

        #region SEARCH RECORDS

        [RoleAuthorize("Records")]
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
            bool idRecordDeleted = await _recordsService.DeleteRecord(requestId, GetClaimsData().Id);
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
            if(excelFile.Length == 0)
            {
                 
                return StatusCode(400);
            }

            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "records.xlsx");
        }

        #endregion

        #region PATIENT HISTORY AND RECORD

        [RoleAuthorize("History")]
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

        #endregion

        #region BLOCK HISTORY

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


        public async Task<IActionResult> Unblock(int blockRequestId)
        {
            bool isRequestUnblocked = await _recordsService.UnblockRequest(blockRequestId, GetClaimsData().Id);
            if (isRequestUnblocked)
            {
                TempData["SuccessMessage"] = "Request Unblocked Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Unblock Request.";
            }

            return RedirectToAction("BlockedHistory");
        }

        #endregion

        #region EMAIL LOGS

        [RoleAuthorize("EmailLogs")]
        public IActionResult EmailLogs()
        {
            return View();
        }


        [HttpPost]
        public IActionResult GetEmailLogs(SearchRecordsViewModel SearchFilters)
        {
            if (SearchFilters.PageNumber <= 0)
            {
                SearchFilters.PageNumber = 1;
            }

            PaginatedListViewModel<LogRowViewModel> PaginatedList = new PaginatedListViewModel<LogRowViewModel>();
            PaginatedList = _recordsService.GetEmailLogs(SearchFilters);

            ViewBag.PagerData = PaginatedList.PagerData;
            return PartialView("_EmailSMSLogPartial", PaginatedList.DataRows);
        }

        #endregion

        #region SMS LOGS

        [RoleAuthorize("SmsLogs")]
        public IActionResult SMSLogs()
        {
            return View();
        }


        [HttpPost]
        public IActionResult GetSMSLogs(SearchRecordsViewModel SearchFilters)
        {
            if (SearchFilters.PageNumber <= 0)
            {
                SearchFilters.PageNumber = 1;
            }

            PaginatedListViewModel<LogRowViewModel> PaginatedList = new PaginatedListViewModel<LogRowViewModel>();
            PaginatedList = _recordsService.GetSMSLogs(SearchFilters);

            ViewBag.PagerData = PaginatedList.PagerData;
            return PartialView("_EmailSMSLogPartial", PaginatedList.DataRows);
        }

        #endregion
    }
}
