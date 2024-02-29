using Microsoft.AspNetCore.Mvc;
using HalloDocServices.ViewModels.AdminViewModels;
using HalloDocServices.Interface;
using HalloDocEntities.Models;

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
            return View();
        }
    }
}
