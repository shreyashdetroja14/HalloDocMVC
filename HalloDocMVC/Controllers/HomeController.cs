using HalloDocEntities.Data;

using HalloDocEntities.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace HalloDocMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HalloDocContext _context;

        public HomeController(HalloDocContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("UserName,PasswordHash,Email,PhoneNumber,Ip")] AspNetUser aspnetuser)
        {
            Console.WriteLine(aspnetuser);

            var id = Guid.NewGuid().ToString();
            var createddate = DateTime.Now;
            Console.WriteLine(aspnetuser.Id);
            aspnetuser.Id = id;
            Console.WriteLine(aspnetuser.Id);
            aspnetuser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(aspnetuser.PasswordHash);
            aspnetuser.CreatedDate = createddate;
            aspnetuser.ModifiedDate = createddate;

            ModelState.Remove("Id");

            if (ModelState.IsValid)
            {

                _context.Add(aspnetuser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Login");
            }

            return View("AddUser");


        }

        
    }
}