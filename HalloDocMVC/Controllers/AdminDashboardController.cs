﻿using Microsoft.AspNetCore.Mvc;
using HalloDocServices.ViewModels.AdminViewModels;
using HalloDocServices.Interface;
using HalloDocEntities.Models;
using HalloDocServices.Implementation;
using HalloDocServices.ViewModels;

namespace HalloDocMVC.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IPatientService _patientService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService, IPatientService patientService)
        {
            _adminDashboardService = adminDashboardService;
            _patientService = patientService;
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
        public async Task<IActionResult> ViewNotes(ViewNotesViewModel vnvm)
        {
            bool isNoteAdded = false;
            if (vnvm.AdminNotesInput != null)
            {
                isNoteAdded = await _adminDashboardService.AddAdminNote(vnvm.RequestId ?? 0, vnvm.AdminNotesInput);
            }
            if(isNoteAdded)
            {
                /*return RedirectToAction("ViewNotes", new { requestId });*/
                ViewNotesViewModel ViewNotes = new ViewNotesViewModel();
                ViewNotes = await _adminDashboardService.GetViewNotesViewModelData(vnvm.RequestId ?? 0);
                return View(ViewNotes);
            }
            else
            {
                return View(vnvm.AdminNotesInput);
            }
        }

        public IActionResult CancelCase(int requestId)
        {
            CancelCaseViewModel CancelCase = new CancelCaseViewModel();

            CancelCase.RequestId = requestId;
            CancelCase.AdminId = 1;

            CancelCase = _adminDashboardService.GetCancelCaseViewModelData(CancelCase);

            return PartialView("_CancelCaseModal", CancelCase);
        }

        [HttpPost]
        public async Task<IActionResult> CancelCase(CancelCaseViewModel CancelCase)
        {
            bool isCaseCancelled = await _adminDashboardService.CancelCase(CancelCase);
            if(isCaseCancelled)
            {
                
            }

            return RedirectToAction("Index");
        }

        public IActionResult AssignCase(int requestId, bool? isTransferRequest, int regionId)
        {
            AssignCaseViewModel AssignCase = new AssignCaseViewModel();
            AssignCase.RequestId = requestId;
            AssignCase.RegionId = regionId;
            AssignCase.IsTransferRequest = isTransferRequest;
            AssignCase = _adminDashboardService.GetAssignCaseViewModelData(AssignCase);

            return PartialView("_AssignCaseModal", AssignCase);
        }

        [HttpPost]
        public async Task<IActionResult> AssignCase(AssignCaseViewModel AssignCase)
        {
            bool isCaseAssigned = await _adminDashboardService.AssignCase(AssignCase);
            if (isCaseAssigned)
            {

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TransferRequest(AssignCaseViewModel TransferRequest)
        {
            bool isRequestTransferred = await _adminDashboardService.TransferRequest(TransferRequest);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> BlockRequest(int requestId) 
        {
            BlockRequestViewModel BlockRequest = new BlockRequestViewModel(); 
            BlockRequest.RequestId = requestId;
            BlockRequest = await _adminDashboardService.GetBlockRequestViewModelData(BlockRequest);

            return PartialView("_BlockRequestModal", BlockRequest);
        }

        [HttpPost]
        public async Task<IActionResult> BlockRequest(BlockRequestViewModel BlockRequest)
        {
            bool isReuqestBlocked = await _adminDashboardService.BlockRequest(BlockRequest);
            if (isReuqestBlocked)
            {

            }

            //return View("Index", "AdminDashboard");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ViewUploads(int requestId)
        {
            ViewDocumentsViewModel ViewUploads = new ViewDocumentsViewModel();
            ViewUploads.RequestId = requestId;
            ViewUploads = await _adminDashboardService.GetViewUploadsViewModelData(ViewUploads);

            return View(ViewUploads);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadRequestFile(IEnumerable<IFormFile>? MultipleFiles, int requestId)
        {

            if (MultipleFiles != null && MultipleFiles?.Count() != 0)
            {
                await _adminDashboardService.UploadFiles(MultipleFiles, requestId);
            }
            return RedirectToAction("ViewUploads", new { requestId });
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var downloadedFile = await _adminDashboardService.DownloadFile(id);

            if (downloadedFile != null) 
            { 
                
            }

            return File(downloadedFile.Data, "application/octet-stream", downloadedFile.Filename);

        }

        [HttpPost]
        public async Task<IActionResult> DownloadMultipleFiles([FromBody] List<string> selectedValues)
        {
            Console.WriteLine($"Selected Values: {selectedValues}");
            //Console.WriteLine($"Request ID: {requestId}");
            return View();
        }
    }
}
