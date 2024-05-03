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
            return View();
        }

        public IActionResult GetTimesheet(string selectedDatePeriod)
        {
            // Regular expression to extract start and end dates
            var datePattern = @"^(\d{4}-\d{2}-\d{2})-(\d{4}-\d{2}-\d{2})$";
            var match = Regex.Match(selectedDatePeriod, datePattern);

            if (!match.Success)
            {
                return BadRequest("Invalid date format in selected value.");
            }

            var startDate = DateTime.Parse(match.Groups[1].Value);
            var endDate = DateTime.Parse(match.Groups[2].Value);

            

            return PartialView("_TimesheetPartial"); 
        }

        public IActionResult AddOrEditTimesheet(string selectedDatePeriod)
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
            TimesheetData.PhysicianId = claimsData.Id;
            TimesheetData.TimesheetStartDate = DateOnly.FromDateTime(startDate).ToString("yyyy-MM-dd");
            TimesheetData.TimesheetEndDate = DateOnly.FromDateTime(endDate).ToString("yyyy-MM-dd");

            TimesheetData = _invoiceService.GetTimesheetViewModelData(TimesheetData);

            return View(TimesheetData); 
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditTimesheet(TimesheetViewModel TimesheetData)
        {

            ClaimsData claimsData = _jwtService.GetClaimValues();
            TimesheetData.PhysicianId = claimsData.Id;
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

            return View(TimesheetData);
        }
    }
}
