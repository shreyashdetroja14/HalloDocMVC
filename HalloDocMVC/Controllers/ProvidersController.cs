using HalloDocServices.Interface;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index()
        {
            return View();
        }
    }
}
