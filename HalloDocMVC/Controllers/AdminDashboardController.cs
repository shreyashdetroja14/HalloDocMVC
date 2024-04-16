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

            return RedirectToRoute("Dashboard");
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


        [Route("Dashboard/ViewNotes", Name = "ViewNotes")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> ViewNotes(int requestId)
        {
            ClaimsData claimsData = GetClaimsData();

            ViewNotesViewModel ViewNotes = new ViewNotesViewModel();
            ViewNotes = await _adminDashboardService.GetViewNotesViewModelData(requestId);
            
            ViewNotes.RequestId = requestId;
            ViewNotes.IsPhysician = claimsData.AspNetUserRole == "physician" ? true : false;

            return View(ViewNotes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Route("Dashboard/ViewNotes", Name = "EditViewNotes")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> ViewNotes(ViewNotesViewModel vnvm)
        {
            ClaimsData claimsData = GetClaimsData();

            vnvm.CreatedBy = claimsData.AspNetUserId;
            vnvm.IsPhysician = claimsData.AspNetUserRole == "physician" ? true : false;

            bool isNoteAdded = await _adminDashboardService.AddNote(vnvm);
            if (isNoteAdded)
            {
                TempData["SuccessMessage"] = "Note Added Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Add Note.";
            }

            return RedirectToRoute("ViewNotes", new { requestId = vnvm.RequestId });
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

            return RedirectToRoute("Dashboard");
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
                TempData["SuccessMessage"] = "Case Assigned Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Assign Case.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TransferRequest(AssignCaseViewModel TransferRequest)
        {
            bool isRequestTransferred = await _adminDashboardService.TransferRequest(TransferRequest);

            return RedirectToAction("Index");
        }

        [Route("Physician/TransferToAdmin", Name = "TransferToAdmin")]
        [CustomAuthorize("physician")]
        public IActionResult TransferToAdmin(int requestId)
        {
            TransferToAdminViewModel TransferData = new TransferToAdminViewModel();
            //TransferData = _adminDashboardService.GetTransferToAdminData(BlockRequest);
            TransferData.RequestId = requestId;

            return PartialView("_TransferToAdminModal", TransferData);
        }

        [HttpPost]
        [Route("Physician/TransferToAdmin", Name = "TransferToAdmin")]
        [CustomAuthorize("physician")]
        public async Task<IActionResult> TransferToAdmin(TransferToAdminViewModel TransferData)
        {
            ClaimsData claimsData = GetClaimsData();
            TransferData.PhysicianId = claimsData.Id;

            bool isRequestTransferred = await _adminDashboardService.TransferToAdmin(TransferData);
            if (isRequestTransferred)
            {
                TempData["SuccessMessage"] = "Request Transferred Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Transfer Request.";
            }

            return RedirectToRoute("Dashboard");
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
                TempData["SuccessMessage"] = "Request Blocked Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Block Request.";
            }

            return RedirectToAction("Index");
        }


        [Route("Dashboard/ViewUploads", Name = "ViewUploads")]

        [CustomAuthorize("admin", "physician")]
        public IActionResult ViewUploads(int requestId)
        {
            ViewDocumentsViewModel ViewUploads = new ViewDocumentsViewModel();
            ViewUploads.RequestId = requestId;
            ViewUploads = _adminDashboardService.GetViewUploadsViewModelData(ViewUploads);

            return View(ViewUploads);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Route("Dashboard/UploadRequestFile", Name = "UploadRequestFile")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> UploadRequestFile(IEnumerable<IFormFile>? MultipleFiles, int requestId)
        {

            if (MultipleFiles != null && MultipleFiles?.Count() != 0)
            {
                await _adminDashboardService.UploadFiles(MultipleFiles, requestId);
            }
            return RedirectToAction("ViewUploads", new { requestId });
        }


        [Route("Dashboard/DownloadFile", Name = "DownloadFile")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            var downloadedFile = await _adminDashboardService.DownloadFile(fileId);

            if (downloadedFile != null) 
            { 
                return File(downloadedFile.Data, "application/octet-stream", downloadedFile.Filename);
            }
            else
            {
                return RedirectToRoute("ViewUploads", new { requestId = downloadedFile?.RequestId});
            }
        }


        [HttpPost]
        [Route("Dashboard/DownloadAll", Name = "DownloadAll")]

        [CustomAuthorize("admin", "physician")]
        public IActionResult DownloadMultipleFiles([FromBody] DownloadRequest requestData)
        {

            List<int> selectedValues = requestData.SelectedValues;
            int requestId = requestData.RequestId;

            var zipdata = _adminDashboardService.GetFilesAsZip(selectedValues, requestId);

            if (zipdata != null)
            {
                return File(zipdata, "application/zip", "download.zip");
            }

            return RedirectToRoute("ViewUploads", new { requestId });
        }


        [Route("Dashboard/DeleteFile", Name = "DeleteFile")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            var deletedFile = await _adminDashboardService.DeleteFile(fileId);

            return RedirectToRoute("ViewUploads", new { requestId = deletedFile?.RequestId });
        }


        [HttpPost]
        [Route("Dashboard/DeleteAll", Name = "DeleteAll")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> DeleteMultipleFiles([FromBody] DownloadRequest requestData)
        {
            List<int> selectedValues = requestData.SelectedValues;
            int requestId = requestData.RequestId;

            bool isFileDeleted = await _adminDashboardService.DeleteSelectedFiles(selectedValues, requestId);
            if (isFileDeleted)
            {
                TempData["SuccessMessage"] = "Files Deleted Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Delete Files.";
            }

            string url = "/Dashboard/ViewUploads?requestId=" + requestData.RequestId.ToString();
            return Content(url);
            //return RedirectToRoute("ViewUploads", new { requestId });
        }


        [HttpPost]
        [Route("Dashboard/SendMail", Name = "SendMail")]

        [CustomAuthorize("admin", "physician")]
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

            string url = "/Dashboard/ViewUploads?requestId=" + requestData.RequestId.ToString();
            return Content(url);

            //return RedirectToAction("ViewUploads", new { requestId = requestData?.RequestId });
        }


        [Route("Dashboard/Orders", Name = "Orders")]

        [CustomAuthorize("admin", "physician")]
        public IActionResult Orders(int requestId)
        {
            OrdersViewModel OrderPageDetails = new OrdersViewModel();
            OrderPageDetails = _adminDashboardService.GetOrdersViewModel(requestId);

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
        [ValidateAntiForgeryToken]
        [Route("Dashboard/Orders", Name = "OrdersPost")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> SendOrder(OrdersViewModel Order)
        {
            bool isOrderSent = await _adminDashboardService.SendOrder(Order);
            if (isOrderSent)
            {
                TempData["SuccessMessage"] = "Order Sent Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Send Order.";
            }

            return RedirectToRoute("Orders", new {requestId = Order.RequestId});
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

        [Route("Dashboard/SendAgreement", Name = "SendAgreement")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> SendAgreement(int requestId)
        {
            SendAgreementViewModel SendAgreementInfo = new SendAgreementViewModel();
            SendAgreementInfo.RequestId = requestId;
            SendAgreementInfo = await _adminDashboardService.GetSendAgreementViewModelData(SendAgreementInfo);

            return PartialView("_SendAgreementModal", SendAgreementInfo);
        }

        [HttpPost]
        [Route("Dashboard/SendAgreement", Name = "SendAgreementPost")]

        [CustomAuthorize("admin", "physician")]
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
            return RedirectToRoute("Dashboard");
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


        [Route("Physician/CareType", Name = "CareType")]

        [CustomAuthorize("physician")]
        public IActionResult CareType(int requestId)
        {
            CareTypeViewModel CareTypeData = new CareTypeViewModel();
            CareTypeData.RequestId = requestId;

            return PartialView("_CareTypeModal", CareTypeData);
        }


        [HttpPost]
        [Route("Physician/CareType", Name = "CareTypePost")]

        [CustomAuthorize("physician")]
        public async Task<IActionResult> CareType(CareTypeViewModel CareTypeData)
        {
            ClaimsData claimsData = GetClaimsData();
            CareTypeData.PhysicianId = claimsData.Id;

            bool isCareTypeSelected = await _adminDashboardService.SelectCareType(CareTypeData);
            if (isCareTypeSelected)
            {
                TempData["SuccessMessage"] = "Care Type Selected Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Select Care Type.";
            }
            return RedirectToRoute("Dashboard");

        }


        [Route("Dashboard/EncounterForm", Name = "EncounterForm")]

        [CustomAuthorize("admin", "physician")]
        public IActionResult EncounterForm(int requestId)
        {
            ClaimsData claimsData = GetClaimsData();

            EncounterFormViewModel EncounterFormDetails = new EncounterFormViewModel();
            EncounterFormDetails.RequestId = requestId;
            EncounterFormDetails.UserRole = claimsData.AspNetUserRole;

            EncounterFormDetails = _adminDashboardService.GetEncounterFormViewModelData(EncounterFormDetails); 

            return View(EncounterFormDetails);
        }

        [HttpPost]
        [Route("Dashboard/EncounterForm", Name = "EncounterFormPost")]

        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> EncounterForm(EncounterFormViewModel EncounterFormDetails)
        {
            bool isEncounterFormUpdated = await _adminDashboardService.UpdateEncounterForm(EncounterFormDetails);
            if (isEncounterFormUpdated)
            {
                TempData["SuccessMessage"] = "Encounter Form Updated Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Update Encounter Form.";
            }

            return RedirectToRoute("EncounterForm", new { EncounterFormDetails.RequestId });
        }

        [HttpPost]
        [Route("Physician/Finalize", Name = "Finalize")]

        [CustomAuthorize("physician")]
        public async Task<IActionResult> Finalize(EncounterFormViewModel EncounterFormDetails)
        {
            bool isFinalized = await _adminDashboardService.Finalize(EncounterFormDetails);
            if (isFinalized)
            {
                TempData["SuccessMessage"] = "Encounter Form Finalized Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Finalize Encounter Form.";
            }
            return RedirectToRoute("Dashboard");
        }


        [Route("Physician/HouseCall", Name = "HouseCall")]

        [CustomAuthorize("physician")]
        public async Task<IActionResult> HouseCall(int requestId)
        {
            ClaimsData claimsData = GetClaimsData();
            int physicianId = claimsData.Id;

            bool isConcluded = await _adminDashboardService.HouseCall(requestId, physicianId);
            if (isConcluded)
            {
                TempData["SuccessMessage"] = "Request Concluded Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Conclude Request.";
            }
            return RedirectToRoute("Dashboard");
        }

        [Route("Physician/ConcludeCare", Name = "ConcludeCare")]

        [CustomAuthorize("physician")]
        public IActionResult ConcludeCare(int requestId)
        {
            ConcludeCareViewModel ConcludeCareData = new ConcludeCareViewModel();
            ConcludeCareData.RequestId = requestId;

            ConcludeCareData = _adminDashboardService.GetConcludeCareViewModel(ConcludeCareData);
            return View(ConcludeCareData);
        }

        [HttpPost]
        [Route("Physician/ConcludeCare", Name = "ConcludeCarePost")]

        [CustomAuthorize("physician")]
        public async Task<IActionResult> ConcludeCare(ConcludeCareViewModel ConcludeCareData)
        {
            bool isCareConcluded = true;
            //bool isCareConcluded = await _adminDashboardService.ConcludeCare(ConcludeCareData);
            if (isCareConcluded)
            {
                TempData["SuccessMessage"] = "Care Concluded Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Conclude Care.";
            }
            return RedirectToRoute("Dashboard");
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


        [Route("Dashboard/CreateRequest", Name = "CreateRequest")]

        [CustomAuthorize("admin", "physician")]
        public IActionResult CreateRequest()
        {
            PatientRequestViewModel PatientInfo = new PatientRequestViewModel();
            ViewBag.RegionList = _adminDashboardService.GetRegionList();
            PatientInfo.CreatorRole = GetClaimsData().AspNetUserRole;
            return View(PatientInfo);
        }

        [Route("Dashboard/CreateRequest", Name = "CreateRequestPost")]

        [CustomAuthorize("admin", "physician")]
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
                return RedirectToRoute("Dashboard");
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Create Request.";
                return RedirectToRoute("CreateRequest");
            }
            
        }
    }
}
