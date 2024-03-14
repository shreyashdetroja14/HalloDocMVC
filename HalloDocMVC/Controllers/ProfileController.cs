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
    }
}
