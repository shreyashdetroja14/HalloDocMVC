using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult PatientLogin()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
