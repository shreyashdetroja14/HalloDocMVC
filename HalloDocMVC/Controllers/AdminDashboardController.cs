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
using System.Dynamic;
using System.Security.Claims;

namespace HalloDocMVC.Controllers
{
    
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


        
        [Route("Dashboard", Name = "Dashboard")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> Index(int? requestStatus)
        {
            ClaimsData claimsData = GetClaimsData();

            AdminDashboardViewModel viewModel = new AdminDashboardViewModel();
            int reqStatus;
            reqStatus = requestStatus?? 1;
            
            if(claimsData.AspNetUserRole == "physician")
            {
                viewModel = await _adminDashboardService.GetViewModelData(reqStatus, claimsData.Id);
                return View("~/Views/AdminDashboard/PhysicianDashboard.cshtml", viewModel);
            }

            viewModel = await _adminDashboardService.GetViewModelData(reqStatus);
            return View(viewModel);
            

        }


        [Route("FetchRequests", Name = "FetchRequests")]

        [CustomAuthorize("admin", "physician")]
        public IActionResult FetchRequests(int requestStatus, int? requestType, string? searchPattern, int? searchRegion, int pageNumber = 1)
        {
            ClaimsData claimsData = GetClaimsData();

            PaginatedListViewModel<RequestRowViewModel> PaginatedList = new PaginatedListViewModel<RequestRowViewModel>();

            if(claimsData.AspNetUserRole == "physician")
            {
                PaginatedList = _adminDashboardService.GetViewModelData(requestStatus, requestType, searchPattern, searchRegion, pageNumber, physicianId: claimsData.Id);
                ViewBag.PagerData = PaginatedList.PagerData;
                return PartialView("_PhysicianRequestTable", PaginatedList.DataRows);
            }
            else
            {
                PaginatedList = _adminDashboardService.GetViewModelData(requestStatus, requestType, searchPattern, searchRegion, pageNumber);
                ViewBag.PagerData = PaginatedList.PagerData;
                return PartialView("_RequestTable", PaginatedList.DataRows);
            }


            
        }


        [Route("Physician/AcceptCase", Name = "AcceptCase")]

        [CustomAuthorize("physician")]
        public async Task<IActionResult> AcceptCase(int requestId)
        {
            ClaimsData claimsData = GetClaimsData();
            int physicianId = claimsData.Id;
            string createdBy = claimsData.AspNetUserId ?? "";

            bool isCaseAccepted = await _adminDashboardService.AcceptCase(requestId);
            if (isCaseAccepted)
            {
                TempData["SuccessMessage"] = "Case Accepted Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Accept Case.";
            }

            return RedirectToRoute("PhysicianDashboard");
        }


        [Route("Dashboard/ViewCase", Name = "ViewCase")]

        [CustomAuthorize("admin", "physician")]
        public IActionResult ViewCase(int requestId) 
        {
            ClaimsData claimsData = GetClaimsData();

            ViewCaseViewModel CaseInfo = new ViewCaseViewModel();
            CaseInfo.RequestId = requestId;
            CaseInfo.IsPhysician = claimsData.AspNetUserRole == "physician" ? true : false;
            
            CaseInfo =  _adminDashboardService.GetViewCaseViewModelData(CaseInfo);

            return View(CaseInfo); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Dashboard/ViewCase", Name = "EditViewCase")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> ViewCase (ViewCaseViewModel CaseInfo)
        {
            bool isInfoUpdated = await _adminDashboardService.UpdateViewCaseInfo(CaseInfo);

            if (isInfoUpdated)
            {
                TempData["SuccessMessage"] = "Case Info Updated Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Update Case Case.";
            }
            return RedirectToRoute("ViewCase", new { requestId = CaseInfo.RequestId });
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
                isNoteAdded = await _adminDashboardService.AddAdminNote(vnvm.RequestId ?? 0, vnvm.AdminNotesInput, GetClaimsData().AspNetUserId ?? "");
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
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Model State Not Valid.";
                return RedirectToAction("Index");
            }

            bool isCaseCancelled = await _adminDashboardService.CancelCase(CancelCase);
            if(isCaseCancelled)
            {
                TempData["SuccessMessage"] = "Case Cancelled Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Cancel Case.";
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

        public IActionResult ViewUploads(int requestId)
        {
            ViewDocumentsViewModel ViewUploads = new ViewDocumentsViewModel();
            ViewUploads.RequestId = requestId;
            ViewUploads = _adminDashboardService.GetViewUploadsViewModelData(ViewUploads);

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
                TempData["SuccessMessage"] = "Mail Sent Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Send Mail. Try Again.";
            }

            string url = "/AdminDashboard/ViewUploads?requestId=" + requestData.RequestId.ToString();
            return Content("");

            //return RedirectToAction("ViewUploads", new { requestId = requestData?.RequestId });
        }

        public IActionResult Orders(int requestId)
        {
            OrdersViewModel OrderPageDetails = new OrdersViewModel();
            OrderPageDetails.RequestId = requestId;
            return View(OrderPageDetails);
        }

        public IActionResult GetProfessionList(int requestId)
        {
            string professionlist = _adminDashboardService.GetProfessionListOptions();

            return Content(professionlist, "text/html");
        }

        public IActionResult GetVendorList(int professionId)
        {
            string vendorlist = _adminDashboardService.GetVendorListOptions(professionId);

            return Content(vendorlist, "text/html");
        }

        public IActionResult GetVendorDetails(int vendorId)
        {
            OrdersViewModel VendorDetails = _adminDashboardService.GetVendorDetails(vendorId);
            return Json(VendorDetails);
        }

        [HttpPost]
        public async Task<IActionResult> SendOrder(OrdersViewModel Order)
        {
            bool isOrderSent = await _adminDashboardService.SendOrder(Order);
            if (isOrderSent)
            {

            }

            return RedirectToAction("Orders", new {requestId = Order.RequestId});
        }

        public IActionResult ClearCase(int requestId)
        {
            ClearCaseViewModel ClearCase = new ClearCaseViewModel();
            ClearCase.RequestId = requestId;
            return PartialView("_ClearCaseModal", ClearCase);
        }

        [HttpPost]
        public async Task<IActionResult> ClearCase(ClearCaseViewModel ClearCase)
        {
            bool isCaseCleared = await _adminDashboardService.ClearCase(ClearCase);
            if(isCaseCleared)
            {

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SendAgreement(int requestId)
        {
            SendAgreementViewModel SendAgreementInfo = new SendAgreementViewModel();
            SendAgreementInfo.RequestId = requestId;
            SendAgreementInfo = await _adminDashboardService.GetSendAgreementViewModelData(SendAgreementInfo);

            return PartialView("_SendAgreementModal", SendAgreementInfo);
        }

        [HttpPost]
        public async Task<IActionResult> SendAgreement(SendAgreementViewModel SendAgreementInfo)
        {
            bool isAgreementSent = await _adminDashboardService.SendAgreementViaMail(SendAgreementInfo);
            if (isAgreementSent)
            {
                TempData["SuccessMessage"] = "Agreement Sent Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Send Agreement.";
            }
            return RedirectToAction("Index");
        }

        public IActionResult CloseCase(int requestId)
        {
            CloseCaseViewModel CloseCaseInfo = new CloseCaseViewModel();

            CloseCaseInfo.ViewUploads.RequestId = requestId;
            CloseCaseInfo.ViewUploads = _adminDashboardService.GetViewUploadsViewModelData(CloseCaseInfo.ViewUploads);

            CloseCaseInfo.ViewCase.RequestId = requestId;
            CloseCaseInfo.ViewCase = _adminDashboardService.GetViewCaseViewModelData(CloseCaseInfo.ViewCase);

            return View(CloseCaseInfo);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCloseCase(ViewCaseViewModel ViewCase)
        {
            bool isInfoUpdated = await _adminDashboardService.UpdateViewCaseInfo(ViewCase);

            if (isInfoUpdated)
            {
                ViewBag.Success = "Case Updated";
            }
            else
            {
                ViewBag.Failure = "Unable to update details";
            }
            return RedirectToAction("CloseCase", new {requestId =  ViewCase.RequestId});
        }

        public async Task<IActionResult> Close(int requestId)
        {
            int adminId = 1;
            bool isCaseClosed = await _adminDashboardService.CloseCase(requestId, adminId);
            if (isCaseClosed)
            {

            }

            return RedirectToAction("Index");
        }

        public IActionResult EncounterForm(int requestId)
        {
            EncounterFormViewModel EncounterFormDetails = new EncounterFormViewModel();
            EncounterFormDetails.RequestId = requestId;

            EncounterFormDetails = _adminDashboardService.GetEncounterFormViewModelData(EncounterFormDetails); 

            return View(EncounterFormDetails);
        }

        [HttpPost]
        public async Task<IActionResult> EncounterForm(EncounterFormViewModel EncounterFormDetails)
        {
            bool isEncounterFormUpdated = await _adminDashboardService.UpdateEncounterForm(EncounterFormDetails);
            if (isEncounterFormUpdated) 
            {

            }

            return View(EncounterFormDetails);
        }

        [HttpPost]
        public async Task<IActionResult> SendLink(AdminDashboardViewModel EmailData)
        {
            bool isLinkSent = await _adminDashboardService.SendLink(EmailData.Email ?? "");
            if (isLinkSent)
            {
                TempData["SuccessMessage"] = "Link Sent Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Send The Link.";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Export(int requestStatus, int? requestType, string? searchPattern, int? searchRegion, int? pageNumber)
        {
            var excelFile = _adminDashboardService.ExportToExcel(requestStatus, requestType, searchPattern, searchRegion, pageNumber);
            
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "requests.xlsx");
        }

        public IActionResult CreateRequest()
        {
            PatientRequestViewModel PatientInfo = new PatientRequestViewModel();
            ViewBag.RegionList = _adminDashboardService.GetRegionList();
            PatientInfo.CreatorRole = GetClaimsData().AspNetUserRole;
            return View(PatientInfo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(PatientRequestViewModel PatientInfo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RegionList = _adminDashboardService.GetRegionList();
                return View(PatientInfo);
            }

            var claimsData = GetClaimsData();

            PatientInfo.CreatedBy = claimsData.Id;
            PatientInfo.CreatorRole = claimsData.AspNetUserRole;
            PatientInfo.CreatorAspId = claimsData.AspNetUserId;
            

            PatientInfo.EmailToken = _jwtService.GenerateEmailToken(PatientInfo.Email, isExpireable: false);

            bool isRequestCreated = await _adminDashboardService.CreateRequest(PatientInfo);
            if (isRequestCreated)
            {
                TempData["SuccessMessage"] = "Request Created Successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Create Request.";
                return RedirectToAction("CreateRequest");
            }
            
        }
    }
}
