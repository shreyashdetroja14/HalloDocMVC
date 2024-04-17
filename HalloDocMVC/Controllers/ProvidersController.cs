using DocumentFormat.OpenXml.Wordprocessing;
using HalloDocMVC.Auth;
using HalloDocServices.Implementation;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDocMVC.Controllers
{
    
    public class ProvidersController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly IProvidersService _providersService;
        private readonly IMailService _mailService;

        public ProvidersController(IJwtService jwtService, IProvidersService providersService, IMailService mailService)
        {
            _jwtService = jwtService;
            _providersService = providersService;
            _mailService = mailService;
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

        #region PROVIDERS LIST 

        [CustomAuthorize("admin")]
        public IActionResult Index()
        {
            ProvidersViewModel Providers = new ProvidersViewModel();
            Providers = _providersService.GetProvidersViewModel(Providers);

            return View(Providers);
        }

        [CustomAuthorize("admin")]
        public IActionResult FetchProviders(int regionId)
        {
            List<ProviderRowViewModel> Providers = new List<ProviderRowViewModel>();
            Providers = _providersService.GetProvidersList(regionId);
            return PartialView("_ProvidersListPartial", Providers);
        }

        [CustomAuthorize("admin")]
        public IActionResult ContactProvider(int providerId)
        {
            ContactProviderViewModel ContactProvider = new ContactProviderViewModel();
            ContactProvider.ProviderId = providerId;
            ContactProvider = _providersService.GetContactProvider(ContactProvider);

            return PartialView("_ContactProviderModal", ContactProvider);
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> ContactProvider(ContactProviderViewModel ContactProvider)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            ContactProvider.AdminId = claimsData.Id;

            bool isMailSent = await _providersService.ContactProvider(ContactProvider);

            if (isMailSent)
            {
                TempData["SuccessMessage"] = "Provider Contacted Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Contact Provider. Try Again.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> StopNotifications(List<int> StopNotificationIds)
        {
            bool isNotiStatusUpdated = await _providersService.UpdateNotiStatus(StopNotificationIds);
            if (isNotiStatusUpdated)
            {
                TempData["SuccessMessage"] = "Notification Status Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Update Notification Status.";
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region EDIT PROVIDER

        [CustomAuthorize("admin")]
        public IActionResult EditProvider(int providerId)
        {
            EditProviderViewModel ProviderInfo = new EditProviderViewModel();
            ProviderInfo.ProviderId = providerId;
            ProviderInfo.IsAccessProvider = false;

            ProviderInfo = _providersService.GetEditProviderViewModel(ProviderInfo);

            return View(ProviderInfo);
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> ResetPassword(EditProviderViewModel AccountInfo)
        {

            if (AccountInfo.Password == null)
            {
                TempData["ErrorMessage"] = "Unable to reset password";
                return RedirectToAction("EditProvider", new { providerId = AccountInfo.ProviderId });
            }

            bool isPasswordReset = await _providersService.ResetPassword(AccountInfo);
            if (isPasswordReset)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to reset password";

            }

            return RedirectToAction("EditProvider", new { providerId = AccountInfo.ProviderId });
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> EditAccountInfo(EditProviderViewModel AccountInfo)
        {
            bool isInfoUpdated= await _providersService.EditAccountInfo(AccountInfo);
            if (isInfoUpdated)
            {
                TempData["SuccessMessage"] = "Account Info Updated Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Update Account Info.";
            }
            return RedirectToAction("EditProvider", new {providerId = AccountInfo.ProviderId});
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> EditPhysicianInfo(EditProviderViewModel PhysicianInfo)
        {
            bool isInfoUpdated = await _providersService.EditPhysicianInfo(PhysicianInfo);
            if (isInfoUpdated)
            {
                TempData["SuccessMessage"] = "Physician Info Updated Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Update Physician Info.";
            }
            return RedirectToAction("EditProvider", new { providerId = PhysicianInfo.ProviderId });
        }
        
        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> EditBillingInfo(EditProviderViewModel BillingInfo)
        {
            bool isInfoUpdated = await _providersService.EditBillingInfo(BillingInfo);
            if (isInfoUpdated)
            {
                TempData["SuccessMessage"] = "Billing Info Updated Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Update Billing Info.";
            }
            return RedirectToAction("EditProvider", new { providerId = BillingInfo.ProviderId });
        }
        
        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> EditProfileInfo(EditProviderViewModel ProfileInfo)
        {
            bool isInfoUpdated = await _providersService.EditProfileInfo(ProfileInfo);
            if (isInfoUpdated)
            {
                TempData["SuccessMessage"] = "Profile Info Updated Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Update Profile Info.";
            }
            return RedirectToAction("EditProvider", new { providerId = ProfileInfo.ProviderId });
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> Onboarding(IFormFile UploadDoc, int docId, int providerId)
        {

            bool isDocUploaded = await _providersService.Onboarding(UploadDoc, docId, providerId);
            if (isDocUploaded)
            {
                TempData["SuccessMessage"] = "Document Uploaded Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Upload Document.";
            }

            return RedirectToAction("EditProvider", new { providerId });
        }

        [CustomAuthorize("admin")]
        public async Task<IActionResult> DeleteProvider(int providerId)
        {
            bool isProvDeleted = await _providersService.DeleteProvider(providerId);
            if (isProvDeleted)
            {
                TempData["SuccessMessage"] = "Provider Deleted Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Delete Provider.";
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region CREATE PROVIDER

        [CustomAuthorize("admin")]
        public IActionResult CreateProvider()
        {
            EditProviderViewModel ProviderInfo = new EditProviderViewModel();

            ProviderInfo = _providersService.GetEditProviderViewModel(ProviderInfo);

            return View(ProviderInfo);
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> CreateProvider(EditProviderViewModel ProviderInfo)
         {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            ProviderInfo.CreatedBy = claimsData.AspNetUserId;

            bool isProvCreated= await _providersService.CreateProvider(ProviderInfo);
            if (isProvCreated)
            {
                TempData["SuccessMessage"] = "Provider Created Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Provider.";
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region SCHEDULING PAGE
        [Route("Scheduling", Name = "Scheduling")]
        [CustomAuthorize("admin", "physician")]
        public IActionResult Scheduling()
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            SchedulingViewModel SchedulingData = new SchedulingViewModel();
            if(claimsData.AspNetUserRole == "physician")
            {
                SchedulingData.IsPhysician = true;
                SchedulingData.PhysicianId = claimsData.Id;
                SchedulingData.CreateShiftData.PhysicianId = claimsData.Id;
            }
            else
            {
                SchedulingData.IsPhysician = false;
            }
            SchedulingData = _providersService.GetSchedulingViewModel(SchedulingData);

            return View(SchedulingData);
        }

        [CustomAuthorize("admin", "physician")]
        public IActionResult GetEventResources(int regionId)
        {
            CalendarViewModel calendarData = new CalendarViewModel();
            ClaimsData claimsData = _jwtService.GetClaimValues();
            if(claimsData.AspNetUserRole == "physician")
            {
                calendarData = _providersService.GetCalendarViewModel(regionId, claimsData.Id);
            }
            else
            {
                calendarData = _providersService.GetCalendarViewModel(regionId);
            }

            return Json(calendarData);
        }

        [HttpPost]
        [CustomAuthorize("admin", "physician")]
        public IActionResult CheckAvailableShift(CreateShiftViewModel CreateShiftData)
        {
            bool isShiftAvailable = _providersService.CheckAvailableShift(CreateShiftData);

            return Json(new { status = isShiftAvailable });
        }

        [HttpPost]
        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> CreateShift(CreateShiftViewModel CreateShiftData)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            CreateShiftData.CreatedBy = claimsData.AspNetUserId ?? "";
            CreateShiftData.CreatorRole = claimsData.AspNetUserRole;
            bool isShiftCreated = await _providersService.CreateShift(CreateShiftData);
            if (isShiftCreated)
            {
                TempData["SuccessMessage"] = "Shift Created Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Shift.";
            }

            return RedirectToAction("Scheduling");
        }

        [CustomAuthorize("admin")]
        public IActionResult GetPhysicianSelectList(int regionId)
        {
            List<SelectListItem> physicianList = _providersService.GetPhysiciansByRegion(regionId);

            return Json(physicianList);
        }

        [CustomAuthorize("admin", "physician")]
        public IActionResult ViewShift(int shiftDetailId)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            CreateShiftViewModel ViewShiftData = new CreateShiftViewModel();
            ViewShiftData.ShiftDetailId = shiftDetailId;
            ViewShiftData = _providersService.GetViewShiftViewModel(ViewShiftData);

            if(claimsData.AspNetUserRole == "physician")
            {
                ViewShiftData.IsPhysician = true;
            }
            else
            {
                ViewShiftData.IsPhysician = false;
            }

            return PartialView("_ViewShiftModal", ViewShiftData);
        }

        [HttpPost]
        [CustomAuthorize("admin", "physician")]
        public async Task<IActionResult> EditShift(CreateShiftViewModel EditShiftData)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            EditShiftData.CreatorRole = claimsData.AspNetUserRole;

            bool isShiftEdited= await _providersService.EditShift(EditShiftData);
            if (isShiftEdited)
            {
                TempData["SuccessMessage"] = "Shift Edited Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Edit Shift.";
            }
            return RedirectToAction("Scheduling");
        }
        
        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> ReturnShift(CreateShiftViewModel ReturnShiftData)
        {
            bool isShiftReturned= await _providersService.ReturnShift(ReturnShiftData);
            if (isShiftReturned)
            {
                TempData["SuccessMessage"] = "Shift Returned Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Return Shift.";
            }
            return RedirectToAction("Scheduling");
        }
        
        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> DeleteShift(CreateShiftViewModel DeleteShiftData)
        {
            bool isShiftDeleted= await _providersService.DeleteShift(DeleteShiftData);
            if (isShiftDeleted)
            {
                TempData["SuccessMessage"] = "Shift Deleted Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Delete Shift.";
            }
            return RedirectToAction("Scheduling");
        }

        #endregion

        #region REQUESTED SHIFTS

        [CustomAuthorize("admin")]
        public IActionResult RequestedShifts()
        {
            RequestedShiftViewModel RequestedShiftData = new RequestedShiftViewModel();
            RequestedShiftData = _providersService.GetRequestedShiftViewModel(RequestedShiftData);
            return View(RequestedShiftData);
        }

        [CustomAuthorize("admin")]
        public IActionResult GetShiftsList(int regionId)
        {
            List<RequestShiftRowViewModel> shiftsList = _providersService.GetShiftsList(regionId);

            return PartialView("_RequestShiftTablePartial", shiftsList);
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> ApproveShifts(List<int> shiftDetailIds)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            bool isShiftApproved = await _providersService.ApproveShifts(shiftDetailIds, claimsData.AspNetUserId ?? "");
            if (isShiftApproved)
            {
                TempData["SuccessMessage"] = "Shifts Approved Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Approve Shifts.";
            }
            return RedirectToAction("RequestedShifts");
        }
        
        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> DeleteShifts(List<int> shiftDetailIds)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            bool isShiftDeleted = await _providersService.DeleteShifts(shiftDetailIds, claimsData.AspNetUserId ?? "");
            if (isShiftDeleted)
            {
                TempData["SuccessMessage"] = "Shifts Deleted Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Delete Shifts.";
            }
            return RedirectToAction("RequestedShifts");
        }

        #endregion

        #region MDS ON CALL

        [CustomAuthorize("admin")]
        public IActionResult MDsOnCall()
        {
            MDsOnCallViewModel MDsOnCallData = new MDsOnCallViewModel();
            MDsOnCallData = _providersService.GetMDsOnCallViewModel(MDsOnCallData);

            return View(MDsOnCallData);
        }

        [CustomAuthorize("admin")]
        public IActionResult GetMDsList(int regionId)
        {
            MDsListViewModel MDsList = _providersService.GetMDsList(regionId);
            return PartialView("_MDsOnCallListPartial", MDsList);
        }

        #endregion

        #region PROVIDER LOCATION

        [CustomAuthorize("admin")]
        public IActionResult ProviderLocation()
        {
            List<ProviderLocationViewModel> ProviderLocations = _providersService.GetProviderLocations();
            return View(ProviderLocations);
        }

        #endregion

    }
}
