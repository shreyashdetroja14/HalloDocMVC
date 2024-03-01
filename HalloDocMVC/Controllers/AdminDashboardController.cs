using Microsoft.AspNetCore.Mvc;
using HalloDocServices.ViewModels.AdminViewModels;
using HalloDocServices.Interface;

namespace HalloDocMVC.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;
        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        public async Task<IActionResult> Index(int? requestStatus)
        {
            AdminDashboardViewModel viewModel = new AdminDashboardViewModel();
            int reqStatus;
            reqStatus = requestStatus?? 1;
            
            viewModel = await _adminDashboardService.GetViewModelData(reqStatus);


            return View(viewModel);
        }

        public IActionResult FetchRequests(int requestStatus, int? requestType, string? searchPattern, int? searchRegion)
        {
            List<RequestRowViewModel> viewModels = new List<RequestRowViewModel>();
            viewModels = _adminDashboardService.GetViewModelData(requestStatus, requestType, searchPattern, searchRegion);

            return PartialView("_RequestTable", viewModels);
        }

        public IActionResult ViewCase(int requestId) 
        { 
            ViewCaseViewModel CaseInfo = new ViewCaseViewModel();

            CaseInfo =  _adminDashboardService.GetViewCaseViewModelData(requestId);


            return View(CaseInfo); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewCase (ViewCaseViewModel CaseInfo)
        {
            bool isInfoUpdated = await _adminDashboardService.UpdateViewCaseInfo(CaseInfo);

            if (isInfoUpdated) 
            {
                ViewBag.Success = "Case Updated";
            }
            else
            {
                ViewBag.Failure = "Unable to update details";
            }
            return View(CaseInfo);
        }

        public async Task<IActionResult> ViewNotes(int requestId)
        {
            ViewNotesViewModel ViewNotes = new ViewNotesViewModel();
            ViewNotes = await _adminDashboardService.GetViewNotesViewModelData(requestId);
            return View(ViewNotes);
        }

        [HttpPost]
        public async Task<IActionResult> ViewNotes(int requestId, string? AdminNotesInput)
        {
            bool isNoteAdded = false;
            if (AdminNotesInput != null)
            {
                isNoteAdded = await _adminDashboardService.AddAdminNote(requestId, AdminNotesInput);
            }
            if(isNoteAdded)
            {
                return RedirectToAction("ViewNotes", new { requestId });
            }
            else
            {
                return View(AdminNotesInput);
            }
        }
    }
}
