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
                claimsData.Id = int.Parse(jwtToken?.Claims.FirstOrDefault(x => x.Type == "id")?.Value ?? "");
            }

            return claimsData;
        }

        public IActionResult Accounts()
        {
            AccessViewModel AccessData = new AccessViewModel();
            AccessData.AdminId = GetClaimsData().Id;

            AccessData = _accessService.GetAccessViewModel(AccessData);

            return View(AccessData);
        }

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

        public async Task<IActionResult> UserAccess()
        {
            List<UserAccessRow> userAccessList = await _accessService.GetUserAccessList();
            return View(userAccessList);
        }

        public IActionResult CreatePhysician()
        {
            EditProviderViewModel ProviderInfo = new EditProviderViewModel();
            ProviderInfo.IsCreateProvider = true;

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
            ClaimsData claimsData = GetClaimsData();

            AdminProfileViewModel AdminProfileDetails = _profileService.GetAdminProfileViewModelData(claimsData.AspNetUserId ?? "");
            AdminProfileDetails.IsEditAdmin = true;

            return View("~/Views/Profile/Index.cshtml", AdminProfileDetails);

        }
    }
}
