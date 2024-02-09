using HalloDocMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HalloDocMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HallodocContext _context;

        public HomeController(HallodocContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("UserName,PasswordHash,Email,PhoneNumber,Ip, Id")] AspNetUser aspnetuser)
        {
            Console.WriteLine(aspnetuser);

            var id = Guid.NewGuid().ToString();
            var createddate = DateTime.Now;
            Console.WriteLine(aspnetuser.Id);
            aspnetuser.Id = id;
            Console.WriteLine(aspnetuser.Id);

            aspnetuser.CreatedDate = createddate;
            aspnetuser.ModifiedDate = createddate;

            if (ModelState.IsValid)
            {

                _context.Add(aspnetuser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Login");
            }

            return View("AddUser");


        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}