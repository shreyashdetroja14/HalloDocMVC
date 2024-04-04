using HalloDocMVC.Auth;
using HalloDocServices.Implementation;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDocMVC.Controllers
{
    [CustomAuthorize("admin")]
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

        #region PROVIDERS LIST 

        public IActionResult Index()
        {
            ProvidersViewModel Providers = new ProvidersViewModel();
            Providers = _providersService.GetProvidersViewModel(Providers);

            return View(Providers);
        }

        public IActionResult FetchProviders(int regionId)
        {
            List<ProviderRowViewModel> Providers = new List<ProviderRowViewModel>();
            Providers = _providersService.GetProvidersList(regionId);
            return PartialView("_ProvidersListPartial", Providers);
        }

        public IActionResult ContactProvider(int providerId)
        {
            ContactProviderViewModel ContactProvider = new ContactProviderViewModel();
            ContactProvider.ProviderId = providerId;
            ContactProvider = _providersService.GetContactProvider(ContactProvider);

            return PartialView("_ContactProviderModal", ContactProvider);
        }

        [HttpPost]
        public async Task<IActionResult> ContactProvider(ContactProviderViewModel ContactProvider)
        {
            if (ContactProvider.CommunicationType == "email")
            {
                List<string> receivers = new List<string>
                {
                    ContactProvider.ProviderEmail ?? ""
                };
                string subject = ContactProvider.Subject ?? "";
                string body = ContactProvider.Message ?? "";

                bool isMailSent = await _mailService.SendMail(receivers, subject, body, false, new List<string>());

                if(isMailSent)
                {
                    TempData["SuccessMessage"] = "Mail Sent Successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed To Send Mail. Try Again.";
                }
            }
            else
            {
                TempData["SuccessMessage"] = "Communication Successfull";

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
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

        public IActionResult EditProvider(int providerId)
        {
            EditProviderViewModel ProviderInfo = new EditProviderViewModel();
            ProviderInfo.ProviderId = providerId;
            ProviderInfo.IsAccessProvider = false;

            ProviderInfo = _providersService.GetEditProviderViewModel(ProviderInfo);

            return View(ProviderInfo);
        }

        [HttpPost]
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

        public IActionResult CreateProvider()
        {
            EditProviderViewModel ProviderInfo = new EditProviderViewModel();

            ProviderInfo = _providersService.GetEditProviderViewModel(ProviderInfo);

            return View(ProviderInfo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProvider(EditProviderViewModel ProviderInfo)
         {
            ProviderInfo.CreatedBy = GetClaimsData().AspNetUserId;

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

        public IActionResult Scheduling()
        {
            SchedulingViewModel SchedulingData = new SchedulingViewModel();
            SchedulingData = _providersService.GetSchedulingViewModel(SchedulingData);

            return View(SchedulingData);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShift(CreateShiftViewModel CreateShiftData)
        {
            CreateShiftData.CreatedBy = GetClaimsData().AspNetUserId;
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

        #endregion

        #region REQUESTED SHIFTS

        public IActionResult RequestedShifts()
        {
            RequestedShiftViewModel RequestedShiftData = new RequestedShiftViewModel();
            RequestedShiftData = _providersService.GetRequestedShiftViewModel(RequestedShiftData);
            return View(RequestedShiftData);
        }

        public IActionResult GetShiftsList(int regionId)
        {
            List<RequestShiftRowViewModel> shiftsList = _providersService.GetShiftsList(regionId);

            return PartialView("_RequestShiftTablePartial", shiftsList);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveShifts(List<int> shiftDetailIds)
        {
            bool isShiftApproved = await _providersService.ApproveShifts(shiftDetailIds, GetClaimsData().AspNetUserId);
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
        public async Task<IActionResult> DeleteShifts(List<int> shiftDetailIds)
        {
            bool isShiftDeleted = await _providersService.DeleteShifts(shiftDetailIds, GetClaimsData().AspNetUserId);
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

        public IActionResult GetEventResources(int regionId)
        {
            CalendarViewModel calendarData = _providersService.GetCalendarViewModel(regionId);

            return Json(calendarData);
        }

        #endregion

    }
}
