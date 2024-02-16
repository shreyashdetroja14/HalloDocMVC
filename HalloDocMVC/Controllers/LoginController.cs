using HalloDocMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly HallodocContext _context;

        public LoginController(HallodocContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckLogin(AspNetUser aspnetuser)
        {
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == aspnetuser.Email && u.PasswordHash == aspnetuser.PasswordHash);

            if (user == null)
            {
                ViewBag.Email = "INVALID EMAIL OR PASSWORD";
                return View("Index");
            }
            else
            {
                return RedirectToAction("Dashboard", "Patient", new {user.Id});
            }
        }
    }
}
