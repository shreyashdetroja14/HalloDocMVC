using HalloDocEntities.Models;
using HalloDocMVC.Auth;
using HalloDocServices.Implementation;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDocMVC.Controllers
{
    [CustomAuthorize("admin")]
    public class AccessController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly IAccessService _accessService;
        private readonly IMailService _mailService;
        private readonly IProvidersService _providersService;
        private readonly IProfileService _profileService;

        public AccessController(IJwtService jwtService, IAccessService accessService, IMailService mailService, IProvidersService providersService, IProfileService profileService)
        {
            _jwtService = jwtService;
            _accessService = accessService;
            _mailService = mailService;
            _providersService = providersService;
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
                claimsData.RoleId = int.Parse(jwtToken?.Claims.FirstOrDefault(x => x.Type == "roleId")?.Value ?? "0");
                claimsData.Id = int.Parse(jwtToken?.Claims.FirstOrDefault(x => x.Type == "id")?.Value ?? "0");
            }

            return claimsData;
        }


        [RoleAuthorize("Accounts")]
        public IActionResult Accounts()
        {
            AccessViewModel AccessData = new AccessViewModel();
            AccessData.AdminId = GetClaimsData().Id;

            AccessData = _accessService.GetAccessViewModel(AccessData);

            return View(AccessData);
        }


        [RoleAuthorize("Role")]
        public IActionResult CreateRole()
        {
            CreateRoleViewModel CreateRoleData = new CreateRoleViewModel();
            CreateRoleData = _accessService.GetCreateRoleViewModel(CreateRoleData);

            return View(CreateRoleData);
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel CreateRoleData)
        {
            if (!ModelState.IsValid)
            {
                CreateRoleData = _accessService.GetCreateRoleViewModel(CreateRoleData);
                return View(CreateRoleData);
            }

            CreateRoleData.CreatedBy = GetClaimsData().AspNetUserId;

            bool isRoleCreated = await _accessService.CreateRole(CreateRoleData);
            if (isRoleCreated)
            {
                TempData["SuccessMessage"] = "Role Created Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Role.";
            }
            return RedirectToAction("Accounts");
        }


        [RoleAuthorize("Role")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            string modifiedBy = GetClaimsData().AspNetUserId ?? "";
            bool isRoleDeleted = await _accessService.DeleteRole(roleId, modifiedBy);

            if (isRoleDeleted)
            {
                TempData["SuccessMessage"] = "Role Deleted Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Delete Role.";
            }
            return RedirectToAction("Accounts");

        }


        [RoleAuthorize("Role")]
        public IActionResult EditRole(int roleId)
        {
            CreateRoleViewModel EditRoleData = new CreateRoleViewModel();
            EditRoleData.RoleId = roleId;

            EditRoleData = _accessService.GetCreateRoleViewModel(EditRoleData);

            return View("CreateRole", EditRoleData);
        }


        [HttpPost]
        public async Task<IActionResult> EditRole(CreateRoleViewModel EditRoleData)
        {
            EditRoleData.ModifiedBy = GetClaimsData().AspNetUserId;

            bool isRoleEdited = await _accessService.EditRole(EditRoleData);
            if (isRoleEdited)
            {
                TempData["SuccessMessage"] = "Role Edited Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Edit Role.";
            }
            return RedirectToAction("Accounts");
        }


        [RoleAuthorize("Users")]
        public async Task<IActionResult> UserAccess()
        {
            List<UserAccessRow> userAccessList = await _accessService.GetUserAccessList();
            return View(userAccessList);
        }


        public IActionResult CreatePhysician()
        {
            EditProviderViewModel ProviderInfo = new EditProviderViewModel();

            ProviderInfo = _providersService.GetEditProviderViewModel(ProviderInfo);

            return View("~/Views/Providers/CreateProvider.cshtml", ProviderInfo);
        }


        [HttpPost]
        public async Task<IActionResult> CreateProvider(EditProviderViewModel ProviderInfo)
        {
            ProviderInfo.CreatedBy = GetClaimsData().AspNetUserId;

            bool isProvCreated = await _providersService.CreateProvider(ProviderInfo);
            if (isProvCreated)
            {
                TempData["SuccessMessage"] = "Provider Created Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Provider.";
            }

            return RedirectToAction("UserAccess");
        }


        public IActionResult CreateAdmin()
        {
            AdminProfileViewModel AdminDetails = new AdminProfileViewModel();
            AdminDetails = _accessService.GetCreateAdminViewModel(AdminDetails);

            return View(AdminDetails);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAdmin(AdminProfileViewModel AdminDetails)
        {
            AdminDetails.CreatedBy = GetClaimsData().AspNetUserId ?? "";
            bool isAdminCreated = await _accessService.CreateAdmin(AdminDetails);
            if (isAdminCreated)
            {
                TempData["SuccessMessage"] = "Admin Created Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Admin.";
            }

            return RedirectToAction("UserAccess");
        }


        public IActionResult EditAdmin(string aspnetuserId)
        {

            AdminProfileViewModel AdminProfileDetails = _profileService.GetAdminProfileViewModelData(aspnetuserId);
            AdminProfileDetails.AspNetUserId = aspnetuserId;
            AdminProfileDetails.IsEditAdmin = true;

            return View("~/Views/Profile/Index.cshtml", AdminProfileDetails);
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(AdminProfileViewModel AdminProfileDetails)
        {

            if (AdminProfileDetails.Password == null)
            {
                TempData["ErrorMessage"] = "Unable to reset password";
                return RedirectToAction("EditAdmin", new {aspnetuserId = AdminProfileDetails.AspNetUserId});
            }

            bool isPasswordReset = await _profileService.ResetPassword(AdminProfileDetails);
            if (isPasswordReset)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to reset password";

            }

            return RedirectToAction("EditAdmin", new { aspnetuserId = AdminProfileDetails.AspNetUserId });
        }


        [HttpPost]
        public async Task<IActionResult> EditAccountInfo(AdminProfileViewModel AdminProfileDetails)
        {

            bool isAdminInfoUpdated = await _profileService.UpdateAccountInfo(AdminProfileDetails);
            if (isAdminInfoUpdated)
            {
                TempData["SuccessMessage"] = "Account Info Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable To Update Account Info";

            }

            return RedirectToAction("EditAdmin", new { aspnetuserId = AdminProfileDetails.AspNetUserId });
        }


        [HttpPost]
        public async Task<IActionResult> EditAdminInfo(AdminProfileViewModel AdminProfileDetails)
        {
            bool isAdminInfoUpdated = await _profileService.UpdateAdminInfo(AdminProfileDetails);
            if (isAdminInfoUpdated)
            {
                TempData["SuccessMessage"] = "Admin Info Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable To Update Info";

            }

            return RedirectToAction("EditAdmin", new { aspnetuserId = AdminProfileDetails.AspNetUserId });
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

            return RedirectToAction("EditAdmin", new { aspnetuserId = AdminProfileDetails.AspNetUserId });
        }


        public IActionResult EditPhysician(int physicianId)
        {
            EditProviderViewModel ProviderInfo = new EditProviderViewModel();
            ProviderInfo.ProviderId = physicianId;
            ProviderInfo.IsAccessProvider = true;

            ProviderInfo = _providersService.GetEditProviderViewModel(ProviderInfo);

            return View("~/Views/Providers/EditProvider.cshtml", ProviderInfo);
        }


        [HttpPost]
        public async Task<IActionResult> ResetProviderPassword(EditProviderViewModel AccountInfo)
        {

            if (AccountInfo.Password == null)
            {
                TempData["ErrorMessage"] = "Unable to reset password";
                return RedirectToAction("EditPhysician", new { physicianId = AccountInfo.ProviderId });
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

            return RedirectToAction("EditPhysician", new { providerId = AccountInfo.ProviderId });
        }


        [HttpPost]
        public async Task<IActionResult> EditProviderAccountInfo(EditProviderViewModel AccountInfo)
        {
            bool isInfoUpdated = await _providersService.EditAccountInfo(AccountInfo);
            if (isInfoUpdated)
            {
                TempData["SuccessMessage"] = "Account Info Updated Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Update Account Info.";
            }
            return RedirectToAction("EditPhysician", new { physicianId = AccountInfo.ProviderId });
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
            return RedirectToAction("EditPhysician", new { physicianId = PhysicianInfo.ProviderId });
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
            return RedirectToAction("EditPhysician", new { physicianId = BillingInfo.ProviderId });
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
            return RedirectToAction("EditPhysician", new { physicianId = ProfileInfo.ProviderId });
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

            return RedirectToAction("EditPhysician", new { physicianId = providerId });
        }
    }
}
