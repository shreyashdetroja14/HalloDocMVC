using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
