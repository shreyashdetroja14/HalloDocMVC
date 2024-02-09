using HalloDocMVC.Models;
using HalloDocMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Controllers
{
    public class RequestFormsController : Controller
    {
        private readonly HallodocContext _context;

        public RequestFormsController(HallodocContext context)
        {
            _context = context;
        }
        public IActionResult SubmitRequest()
        {
            return View();
        }
        public IActionResult PatientRequest()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientRequest(PatientRequestViewModel prvm)
        {
            if(ModelState.IsValid)
            {
                var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == prvm.Email);
                if (aspnetuserFetched == null)
                {
                    var aspnetuserNew = new AspNetUser();

                    aspnetuserNew.Id = Guid.NewGuid().ToString();

                    string? email = prvm.Email;
                    aspnetuserNew.UserName = email.Substring(0, email.IndexOf("@"));

                    aspnetuserNew.PasswordHash = "passworddefault";
                    aspnetuserNew.Email = email;
                    aspnetuserNew.PhoneNumber = prvm.PhoneNumber;
                    aspnetuserNew.CreatedDate = DateTime.Now;

                    _context.Add(aspnetuserNew);
                    await _context.SaveChangesAsync();

                    var userNew = new User();

                    userNew.AspNetUserId = aspnetuserNew.Id;
                    userNew.FirstName = prvm.FirstName;
                    userNew.LastName = prvm.LastName;
                    userNew.Email = prvm.Email;
                    userNew.Mobile = prvm.PhoneNumber;
                    userNew.Street = prvm.Street;
                    userNew.City = prvm.City;
                    userNew.State = prvm.State;
                    userNew.ZipCode = prvm.ZipCode;

                    userNew.CreatedBy = "admin";
                    userNew.CreatedDate = DateTime.Now;

                    _context.Add(userNew);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("SubmitRequest");
            }
            return View("~/Views/Home/Index.cshtml");
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
