using HalloDocEntities.Models;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace HalloDocMVC.Controllers
{
    public class InvoicingController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IJwtService _jwtService;

        public InvoicingController(IInvoiceService invoiceService, IJwtService jwtService)
        {
            _invoiceService = invoiceService;
            _jwtService = jwtService;
        }

        public IActionResult Index()
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            if(claimsData.AspNetUserRole == "admin")
            {
                ViewBag.PhysicianList = _invoiceService.GetPhysicianList();
            }

            return View();
        }

        public IActionResult GetTimesheet(string selectedDatePeriod, int? physicianId)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            // Regular expression to extract start and end dates
            var datePattern = @"^(\d{4}-\d{2}-\d{2})-(\d{4}-\d{2}-\d{2})$";
            var match = Regex.Match(selectedDatePeriod, datePattern);

            if (!match.Success)
            {
                return BadRequest("Invalid date format in selected value.");
            }

            var startDate = DateTime.Parse(match.Groups[1].Value);
            var endDate = DateTime.Parse(match.Groups[2].Value);

            TimesheetViewModel TimesheetData = new TimesheetViewModel();
            TimesheetData.PhysicianId = claimsData.AspNetUserRole == "physician" ? (claimsData.Id) : (physicianId ?? 0);
            TimesheetData.TimesheetStartDate = DateOnly.FromDateTime(startDate).ToString("yyyy-MM-dd");
            TimesheetData.TimesheetEndDate = DateOnly.FromDateTime(endDate).ToString("yyyy-MM-dd");
            TimesheetData.SelectedDatePeriod = selectedDatePeriod;
            TimesheetData.AspNetUserRole = claimsData.AspNetUserRole;

            TimesheetData = _invoiceService.GetTimesheetViewModelData(TimesheetData);

            if (claimsData.AspNetUserRole == "admin")
            {
                return PartialView("_UnapprovedTimesheetPartial", TimesheetData);
            }

            return PartialView("_TimesheetPartial", TimesheetData); 
        }

        public IActionResult GetReceipts(string selectedDatePeriod, int? physicianId)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            // Regular expression to extract start and end dates
            var datePattern = @"^(\d{4}-\d{2}-\d{2})-(\d{4}-\d{2}-\d{2})$";
            var match = Regex.Match(selectedDatePeriod, datePattern);

            if (!match.Success)
            {
                return BadRequest("Invalid date format in selected value.");
            }

            var startDate = DateTime.Parse(match.Groups[1].Value);
            var endDate = DateTime.Parse(match.Groups[2].Value);

            TimesheetViewModel TimesheetData = new TimesheetViewModel();
            TimesheetData.PhysicianId = claimsData.AspNetUserRole == "physician" ? (claimsData.Id) : (physicianId ?? 0);
            TimesheetData.TimesheetStartDate = DateOnly.FromDateTime(startDate).ToString("yyyy-MM-dd");
            TimesheetData.TimesheetEndDate = DateOnly.FromDateTime(endDate).ToString("yyyy-MM-dd");
            TimesheetData.SelectedDatePeriod = selectedDatePeriod;
            TimesheetData.AspNetUserRole = claimsData.AspNetUserRole;

            TimesheetData = _invoiceService.GetTimesheetViewModelData(TimesheetData);

            return PartialView("_ReceiptsPartial", TimesheetData);
        }

        public IActionResult AddOrEditTimesheet(string selectedDatePeriod, int? physicianId)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            // Regular expression to extract start and end dates
            var datePattern = @"^(\d{4}-\d{2}-\d{2})-(\d{4}-\d{2}-\d{2})$";
            var match = Regex.Match(selectedDatePeriod, datePattern);

            if (!match.Success)
            {
                return BadRequest("Invalid date format in selected value.");
            }

            var startDate = DateTime.Parse(match.Groups[1].Value);
            var endDate = DateTime.Parse(match.Groups[2].Value);

            TimesheetViewModel TimesheetData = new TimesheetViewModel();
            TimesheetData.PhysicianId = claimsData.AspNetUserRole == "physician" ? (claimsData.Id) : (physicianId ?? 0);
            TimesheetData.TimesheetStartDate = DateOnly.FromDateTime(startDate).ToString("yyyy-MM-dd");
            TimesheetData.TimesheetEndDate = DateOnly.FromDateTime(endDate).ToString("yyyy-MM-dd");
            TimesheetData.SelectedDatePeriod = selectedDatePeriod;
            TimesheetData.AspNetUserRole = claimsData.AspNetUserRole;

            if (claimsData.AspNetUserRole == "physician")
            {
                bool isSheetFinalized = _invoiceService.CheckFinalizeStatus(TimesheetData);
                if(isSheetFinalized)
                {
                    ViewBag.Message = "The timesheet has already been finalized";
                    return View();
                }
            }

            if (claimsData.AspNetUserRole == "admin")
            {
                bool isSheetApproved = _invoiceService.CheckApprovedStatus(TimesheetData);
                if (isSheetApproved)
                {
                    ViewBag.Message = "The timesheet has already been approved";
                    return View();
                }
            }

            TimesheetData = _invoiceService.GetTimesheetViewModelData(TimesheetData);

            return View(TimesheetData); 
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditTimesheet(TimesheetViewModel TimesheetData)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            if(claimsData.AspNetUserRole == "physician")
            {
                TimesheetData.PhysicianId = claimsData.Id;
            }
            TimesheetData.CreatedBy = claimsData.AspNetUserId;

            bool isAddedOrEdited = await _invoiceService.AddOrEditTimesheet(TimesheetData);
            if (isAddedOrEdited)
            {
                TempData["SuccessMessage"] = "Timesheet Updated Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Update Timesheet.";
            }

            return RedirectToAction("AddOrEditTimesheet", new { selectedDatePeriod = TimesheetData.SelectedDatePeriod, physicianId = TimesheetData.PhysicianId });
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditReceipt(TimesheetReceiptViewModel TimesheetReceipt)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            TimesheetReceipt.AdminId = claimsData.AspNetUserRole == "admin" ? claimsData.Id : null;
            TimesheetReceipt.PhysicianId = claimsData.AspNetUserRole == "physician" ? claimsData.Id : null;

            int receiptId = await _invoiceService.AddOrEditReceipt(TimesheetReceipt);
            //bool isAddedOrEdited = false;
            if (receiptId != 0)
            {
                return Json(new { 
                    result = "success", 
                    itemName = TimesheetReceipt.ItemName,
                    amount = TimesheetReceipt.Amount,
                    fileName = TimesheetReceipt.FileToUpload?.FileName,
                    receiptId = receiptId
                });
            }
            else
            {
                return Json(new { result = "failure" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReceipt(int receiptId)
        {
            bool isDeleted = await _invoiceService.DeleteReceipt(receiptId);
            if (isDeleted)
            {
                return Json(new { result = "success" });
            }
            else
            {
                return Json(new { result = "failure" });
            }
        }

        public async Task<IActionResult> FinalizeTimesheet(int timesheetId)
        {
            bool isFinalized = await _invoiceService.FinalizeTimesheet(timesheetId);
            if (isFinalized)
            {
                TempData["SuccessMessage"] = "Timesheet Finalized Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Finalize Timesheet.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveTimesheet(TimesheetViewModel TimesheetData)
        {
            bool isFinalized = await _invoiceService.ApproveTimesheet(TimesheetData);
            if (isFinalized)
            {
                TempData["SuccessMessage"] = "Timesheet Approved Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Approve Timesheet.";
            }

            return RedirectToAction("Index");
        }
    }
}
