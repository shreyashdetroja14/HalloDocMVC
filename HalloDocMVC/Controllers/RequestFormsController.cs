using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers
{
    public class RequestFormsController : Controller
    {
        public IActionResult SubmitRequest()
        {
            return View();
        }
        public IActionResult PatientRequest()
        {
            return View();
        }
        public IActionResult FamilyRequest()
        {
            return View();
        }
        public IActionResult ConciergeRequest()
        {
            return View();
        }
        public IActionResult BusinessRequest()
        {
            return View();
        }
    }
}
