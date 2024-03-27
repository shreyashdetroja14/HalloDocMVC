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
    public class AccessController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly IAccessService _accessService;
        private readonly IMailService _mailService;

        public AccessController(IJwtService jwtService, IAccessService accessService, IMailService mailService)
        {
            _jwtService = jwtService;
            _accessService = accessService;
            _mailService = mailService;
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
            if(!ModelState.IsValid)
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
    }
}
