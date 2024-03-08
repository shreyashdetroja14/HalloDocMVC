using Microsoft.AspNetCore.Mvc;
using HalloDocServices.ViewModels.AdminViewModels;
using HalloDocServices.Interface;
using HalloDocEntities.Models;
using HalloDocServices.Implementation;
using HalloDocServices.ViewModels;
using System.IO.Compression;
using HalloDocMVC.Auth;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace HalloDocMVC.Controllers
{
    [CustomAuthorize("admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IPatientService _patientService;
        private readonly IJwtService _jwtService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService, IPatientService patientService, IJwtService jwtService)
        {
            _adminDashboardService = adminDashboardService;
            _patientService = patientService;
            _jwtService = jwtService;
        }

        public async Task<IActionResult> Index(int? requestStatus)
        {
            AdminDashboardViewModel viewModel = new AdminDashboardViewModel();
            int reqStatus;
            reqStatus = requestStatus?? 1;
            
            viewModel = await _adminDashboardService.GetViewModelData(reqStatus);


            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult FetchRequests(int requestStatus, int? requestType, string? searchPattern, int? searchRegion)
        {
            string token = Request.Cookies["jwt"]??"";
            if(!_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return StatusCode(401, "Unauthorized");
            }

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

        public async Task<IActionResult> DownloadFile(int fileId)
        {
            var downloadedFile = await _adminDashboardService.DownloadFile(fileId);

            if (downloadedFile != null) 
            { 
                return File(downloadedFile.Data, "application/octet-stream", downloadedFile.Filename);
            }
            else
            {
                return RedirectToAction("ViewUploads", new { requestId = downloadedFile?.RequestId});
            }
        }

        [HttpPost]
        public IActionResult DownloadMultipleFiles([FromBody] DownloadRequest requestData)
        {

            List<int> selectedValues = requestData.SelectedValues;
            int requestId = requestData.RequestId;

            var zipdata = _adminDashboardService.GetFilesAsZip(selectedValues, requestId);

            if (zipdata != null)
            {
                return File(zipdata, "application/zip", "download.zip");
            }

            return RedirectToAction("ViewUploads", new { requestId });
        }

        public async Task<IActionResult> DeleteFile(int fileId)
        {
            var deletedFile = await _adminDashboardService.DeleteFile(fileId);

            return RedirectToAction("ViewUploads", new { requestId = deletedFile?.RequestId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMultipleFiles([FromBody] DownloadRequest requestData)
        {
            List<int> selectedValues = requestData.SelectedValues;
            int requestId = requestData.RequestId;

            await _adminDashboardService.DeleteSelectedFiles(selectedValues, requestId);

            return RedirectToAction("ViewUploads", new { requestId });
        }

        public async Task<IActionResult> SendFilesViaEmail([FromBody] DownloadRequest requestData)
        {
            //var deletedFile = await _adminDashboardService.DeleteFile(fileId);
            bool isMailSent = await _adminDashboardService.SendMailWithAttachments(requestData);
            if (isMailSent)
            {

            }

            return RedirectToAction("ViewUploads", new { requestId = requestData?.RequestId });


        }
    }
}
