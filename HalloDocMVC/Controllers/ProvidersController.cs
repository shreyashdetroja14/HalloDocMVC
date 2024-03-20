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
    }
}
