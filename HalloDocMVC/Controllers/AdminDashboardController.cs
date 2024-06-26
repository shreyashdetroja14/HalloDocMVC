﻿using Microsoft.AspNetCore.Mvc;
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
using HalloDocServices.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        /*public ClaimsData GetClaimsData()
        {
            ClaimsData claimsData = new ClaimsData();

            string token = Request.Cookies["jwt"] ?? "";

            if (_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                claimsData.AspNetUserId = jwtToken.Claims.FirstOrDefault(x => x.Type == "aspnetuserId")?.Value;
                claimsData.Email = jwtToken?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                claimsData.AspNetUserRole = jwtToken?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                claimsData.Username = jwtToken?.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
                claimsData.RoleId = int.Parse(jwtToken?.Claims.FirstOrDefault(x => x.Type == "roleId")?.Value ?? "0");
                claimsData.Id = int.Parse(jwtToken?.Claims.FirstOrDefault(x => x.Type == "id")?.Value ?? "");
            }

            return claimsData;
        }*/

        #endregion



        [Route("Dashboard", Name = "Dashboard")]
        [CustomAuthorize("admin", "physician")]
        [RoleAuthorize("Dashboard")]
        public async Task<IActionResult> Index(int? requestStatus)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            AdminDashboardViewModel viewModel = new AdminDashboardViewModel();
            int reqStatus;
            reqStatus = requestStatus?? 1;
            
            if(claimsData.AspNetUserRole == "physician")
            {
                viewModel = await _adminDashboardService.GetViewModelData(reqStatus, claimsData.Id);
                viewModel.AspNetUserRole = claimsData.AspNetUserRole;
                viewModel.AccountType = (int)(AccountType.Physician);
                viewModel.Username = claimsData.Username;

                return View("~/Views/AdminDashboard/PhysicianDashboard.cshtml", viewModel);
            }

            viewModel = await _adminDashboardService.GetViewModelData(reqStatus);
            viewModel.AspNetUserRole = claimsData.AspNetUserRole;
            viewModel.AccountType = (int)(AccountType.Physician);
            viewModel.Username = claimsData.Username;

            return View(viewModel);
            

        }


        [Route("FetchRequests", Name = "FetchRequests")]
        [CustomAuthorize("admin", "physician")]
        public IActionResult FetchRequests(int requestStatus, int? requestType, string? searchPattern, int? searchRegion, int pageNumber = 1)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

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
            ClaimsData claimsData = _jwtService.GetClaimValues();
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
            ClaimsData claimsData = _jwtService.GetClaimValues();

            if(claimsData.AspNetUserRole == "physician")
            {
                bool isValidRequest = _adminDashboardService.CheckValidRequest(requestId, claimsData.Id);
                if (!isValidRequest)
                {
                    return View("~/Views/Home/AccessDenied.cshtml");
                }
            }

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
            ClaimsData claimsData = _jwtService.GetClaimValues();

            if (claimsData.AspNetUserRole == "physician")
            {
                bool isValidRequest = _adminDashboardService.CheckValidRequest(requestId, claimsData.Id);
                if (!isValidRequest)
                {
                    return View("~/Views/Home/AccessDenied.cshtml");
                }
            }

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
            ClaimsData claimsData = _jwtService.GetClaimValues();

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

            //TempData["back"] = 1;
            return RedirectToRoute("ViewNotes", new { requestId = vnvm.RequestId });
        }


        [CustomAuthorize("admin")]
        public IActionResult CancelCase(int requestId)
        {
            CancelCaseViewModel CancelCase = new CancelCaseViewModel();

            CancelCase.RequestId = requestId;
            CancelCase.AdminId = 1;

            CancelCase = _adminDashboardService.GetCancelCaseViewModelData(CancelCase);

            return PartialView("_CancelCaseModal", CancelCase);
        }


        [HttpPost]
        [CustomAuthorize("admin")]
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


        [CustomAuthorize("admin")]
        public IActionResult AssignCase(int requestId, bool? isTransferRequest)
        {
            AssignCaseViewModel AssignCase = new AssignCaseViewModel();
            AssignCase.RequestId = requestId;
            AssignCase.IsTransferRequest = isTransferRequest;
            AssignCase = _adminDashboardService.GetAssignCaseViewModelData(AssignCase);

            return PartialView("_AssignCaseModal", AssignCase);
        }

        [CustomAuthorize("admin")]
        public IActionResult GetPhysicianSelectList(int regionId)
        {
            List<SelectListItem> physicianList = _adminDashboardService.GetPhysiciansByRegion(regionId);

            return Json(physicianList);
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> AssignCase(AssignCaseViewModel AssignCase)
        {
            ClaimsData claims = _jwtService.GetClaimValues();
            AssignCase.AdminId = claims.Id;

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
        [CustomAuthorize("admin")]
        public async Task<IActionResult> TransferRequest(AssignCaseViewModel TransferRequest)
        {
            ClaimsData claims = _jwtService.GetClaimValues();
            TransferRequest.AdminId = claims.Id;
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
            ClaimsData claimsData = _jwtService.GetClaimValues();
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


        [CustomAuthorize("admin")]
        public async Task<IActionResult> BlockRequest(int requestId) 
        {
            BlockRequestViewModel BlockRequest = new BlockRequestViewModel(); 
            BlockRequest.RequestId = requestId;
            BlockRequest = await _adminDashboardService.GetBlockRequestViewModelData(BlockRequest);

            return PartialView("_BlockRequestModal", BlockRequest);
        }


        [HttpPost]
        [CustomAuthorize("admin")]
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
            ClaimsData claimsData = _jwtService.GetClaimValues();
            if (claimsData.AspNetUserRole == "physician")
            {
                bool isValidRequest = _adminDashboardService.CheckValidRequest(requestId, claimsData.Id);
                if (!isValidRequest)
                {
                    return View("~/Views/Home/AccessDenied.cshtml");
                }
            }

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
                TempData["SuccessMessage"] = "Files uploaded successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to upload files.";
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
                TempData["SuccessMessage"] = "File downloaded successfully";
                return File(downloadedFile.Data, "application/octet-stream", downloadedFile.Filename);
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Download File.";
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

            TempData["SuccessMessage"] = "File deleted successfully";
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
                TempData["SuccessMessage"] = "Files deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete files.";
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
            ClaimsData claimsData = _jwtService.GetClaimValues();
            requestData.SenderRole = claimsData.AspNetUserRole;
            if(claimsData.AspNetUserRole == "physician")
            {
                requestData.PhysicianId = claimsData.Id;
            }
            else
            {
                requestData.AdminId = claimsData.Id;
            }

            bool isMailSent = await _adminDashboardService.SendMailWithAttachments(requestData);
            if (isMailSent)
            {
                TempData["SuccessMessage"] = "Mail sent successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to send mail. Try again.";
            }

            string url = "/Dashboard/ViewUploads?requestId=" + requestData.RequestId.ToString();
            return Content(url);

            //return RedirectToAction("ViewUploads", new { requestId = requestData?.RequestId });
        }


        [Route("Dashboard/Orders", Name = "Orders")]
        [CustomAuthorize("admin", "physician")]
        [RoleAuthorize("SendOrder")]
        public IActionResult Orders(int requestId)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            if (claimsData.AspNetUserRole == "physician")
            {
                bool isValidRequest = _adminDashboardService.CheckValidRequest(requestId, claimsData.Id);
                if (!isValidRequest)
                {
                    return View("~/Views/Home/AccessDenied.cshtml");
                }
            }

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


        [CustomAuthorize("admin")]
        public IActionResult ClearCase(int requestId)
        {
            ClearCaseViewModel ClearCase = new ClearCaseViewModel();
            ClearCase.RequestId = requestId;
            return PartialView("_ClearCaseModal", ClearCase);
        }


        [HttpPost]
        [CustomAuthorize("admin")]
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
            ClaimsData claimsData = _jwtService.GetClaimValues();
            if(claimsData.AspNetUserRole == "physician")
            {
                SendAgreementInfo.PhysicianId = claimsData.Id;
            }
            else
            {
                SendAgreementInfo.AdminId = claimsData.Id;
            }

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


        [Route("/Admin/CloseCase", Name = "CloseCase")]
        [CustomAuthorize("admin")]
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
        [CustomAuthorize("admin")]
        public async Task<IActionResult> UpdateCloseCase(ViewCaseViewModel ViewCase)
        {
            bool isInfoUpdated = await _adminDashboardService.UpdateViewCaseInfo(ViewCase);

            if (isInfoUpdated)
            {
                TempData["SuccessMessage"] = "Case Info Updated Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Update Case Info.";
            }
            return RedirectToAction("CloseCase", new {requestId =  ViewCase.RequestId});
        }


        [CustomAuthorize("admin")]
        public async Task<IActionResult> Close(int requestId)
        {
            int adminId = 1;
            bool isCaseClosed = await _adminDashboardService.CloseCase(requestId, adminId);
            if (isCaseClosed)
            {
                TempData["SuccessMessage"] = "Case Closed Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Close Case.";
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
            ClaimsData claimsData = _jwtService.GetClaimValues();
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
            ClaimsData claimsData = _jwtService.GetClaimValues();
            if (claimsData.AspNetUserRole == "physician")
            {
                bool isValidRequest = _adminDashboardService.CheckValidRequest(requestId, claimsData.Id);
                if (!isValidRequest)
                {
                    return View("~/Views/Home/AccessDenied.cshtml");
                }
            }

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
            ClaimsData claimsData = _jwtService.GetClaimValues();
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
            ClaimsData claimsData = _jwtService.GetClaimValues();
            if (claimsData.AspNetUserRole == "physician")
            {
                bool isValidRequest = _adminDashboardService.CheckValidRequest(requestId, claimsData.Id);
                if (!isValidRequest)
                {
                    return View("~/Views/Home/AccessDenied.cshtml");
                }
            }
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
            ClaimsData claimsData = _jwtService.GetClaimValues();
            ConcludeCareData.CreatedBy = claimsData.AspNetUserId;
            bool isCareConcluded = await _adminDashboardService.ConcludeCare(ConcludeCareData);
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
        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> SendLink(SendLinkViewModel SendLinkData)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            if(claimsData.AspNetUserRole == "physician")
            {
                SendLinkData.PhysicianId = claimsData.Id;
            }
            else
            {
                SendLinkData.AdminId = claimsData.Id;
            }

            bool isLinkSent = await _adminDashboardService.SendLink(SendLinkData);
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


        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> RequestSupport(AdminDashboardViewModel SupportMessageData)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            bool isMailSent = await _adminDashboardService.RequestSupport(SupportMessageData);
            if (isMailSent)
            {
                TempData["SuccessMessage"] = "Support Request sent Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Send The Request.";
            }
            return RedirectToAction("Index");
        }


        [CustomAuthorize("admin")]
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
            PatientInfo.CreatorRole = _jwtService.GetClaimValues().AspNetUserRole;
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

            var claimsData = _jwtService.GetClaimValues();

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


        [Route("Physician/DownloadEncounterForm", Name = "DownloadEncounterForm")]
        [CustomAuthorize("physician")]
        public IActionResult DownloadEncounterForm(int requestId)
        {
            DownloadFormViewModel DownloadFormData = new DownloadFormViewModel();
            DownloadFormData.RequestId = requestId;

            return PartialView("_DownloadFormModal", DownloadFormData);
        }

        [HttpPost]
        [Route("Physician/DownloadEncounterForm", Name = "DownloadEncounterFormPost")]
        [CustomAuthorize("physician")]
        public async Task<IActionResult> DownloadEncounterForm(DownloadFormViewModel DownloadFormData)
        {
            var encounterData = await _adminDashboardService.GenerateEncounterPdf(DownloadFormData);

            if(encounterData == null)
            {
                TempData["ErrorMessage"] = "Unable to download encounter form.";
                return RedirectToRoute("Dashboard");
            }

            return File(encounterData, "application/pdf", "EncounterForm.pdf");
        }
    }
}
