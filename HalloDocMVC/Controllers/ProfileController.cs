using HalloDocMVC.Auth;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDocMVC.Controllers
{
    [CustomAuthorize("admin")]
    public class ProfileController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IPatientService _patientService;
        private readonly IJwtService _jwtService;
        private readonly IProfileService _profileService;

        public ProfileController(IAdminDashboardService adminDashboardService, IPatientService patientService, IJwtService jwtService, IProfileService profileService)
        {
            _adminDashboardService = adminDashboardService;
            _patientService = patientService;
            _jwtService = jwtService;
            _profileService = profileService;
        }

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
            }

            return claimsData;
        }

        public IActionResult Index()
        {
            ClaimsData claimsData = GetClaimsData();

            if(claimsData.AspNetUserRole == "admin")
            {
                AdminProfileViewModel AdminProfileDetails = _profileService.GetAdminProfileViewModelData(claimsData.AspNetUserId??"");
                return View(AdminProfileDetails);
            }
            else
            {
                /*PhysicianProfileViewModel PhysicianProfileDetails = _profileService.GetPhysicianProfileViewModelData(aspnetuserId);
                return View(PhysicianProfileDetails);*/

                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(AdminProfileViewModel AdminProfileDetails)
        {
            if(AdminProfileDetails.Password == null)
            {
                TempData["ErrorMessage"] = "Unable to reset password";
                return RedirectToAction("Index");
            }

            bool isPasswordReset = await _profileService.ResetPassword(AdminProfileDetails);
            if(isPasswordReset)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to reset password";

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditAdminInfo(AdminProfileViewModel AdminProfileDetails)
        {
            if(!AdminProfileDetails.Email.Equals(AdminProfileDetails.ConfirmEmail))
            {
                TempData["ErrorMessage"] = "Email and Confirm Email must be equal";
                return RedirectToAction("Index");

            }

            bool isAdminInfoUpdated = await _profileService.UpdateAdminInfo(AdminProfileDetails);
            if (isAdminInfoUpdated)
            {
                TempData["SuccessMessage"] = "Admin Info Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable To Update Info";

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditBilling(AdminProfileViewModel AdminProfileDetails)
        {
            bool isBillingInfoUpdated = await _profileService.UpdateBillingInfo(AdminProfileDetails);
            if (isBillingInfoUpdated)
            {
                TempData["SuccessMessage"] = "Mailing & Billing Info Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable To Update Info";

            }

            return RedirectToAction("Index");
        }
    }
}
