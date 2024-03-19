using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDocMVC.Controllers
{
    public class ProvidersController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly IProvidersService _providersService;

        public ProvidersController(IJwtService jwtService, IProvidersService providersService)
        {
            _jwtService = jwtService;
            _providersService = providersService;
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
            return PartialView("_ContactProviderModal", ContactProvider);
        }

        [HttpPost]
        public IActionResult ContactProvider(ContactProviderViewModel ContactProvider)
        {
            return View();
        }
    }
}
