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

        public async Task<IActionResult> FetchRequests(int requestStatus, int? requestType)
        {
            List<RequestRowViewModel> viewModels = new List<RequestRowViewModel>();
            viewModels = await _adminDashboardService.GetViewModelData(requestStatus, requestType);

            return PartialView("_RequestTable", viewModels);
        }
    }
}
