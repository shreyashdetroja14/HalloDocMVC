using HalloDocEntities.Data;

using BCrypt.Net;
using HalloDocEntities.Models;
using HalloDocEntities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HalloDocRepository.Repository.Interface;

namespace HalloDocMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly HalloDocContext _context;
        private readonly ILoginRepository _loginRepository;

        public LoginController(HalloDocContext context, ILoginRepository loginRepository)
        {
            _context = context;
            _loginRepository = loginRepository;
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
        public async Task<IActionResult> CheckLogin(LoginViewModel LoginInfo)
        {
            string aspnetuserId = await _loginRepository.CheckLogin(LoginInfo);

            if (aspnetuserId.Equals(""))
            {
                return View("Index");
            }

            return RedirectToAction("Dashboard", "Patient", new { id = aspnetuserId });

            /*var userFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == logininfo.Email);
            if (userFetched != null)
            {
                if (BCrypt.Net.BCrypt.Verify(logininfo.Password, userFetched.PasswordHash))
                {
                    return RedirectToAction("Dashboard", "Patient", new { id = userFetched.Id });
                }
            }
            return View("Index");*/
        }

        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel Credentials)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Please Try Again";
                return View(Credentials);
            }

            if (Credentials.Password != Credentials.ConfirmPassword)
            {
                ViewBag.Message = "Confirmed Password should match the Password";
                return View(Credentials);
            }

            var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == Credentials.Email);
            if (aspnetuserFetched != null)
            {
                ViewBag.Message = "You already have an account with this email!";
                return View();
            }
            var aspnetuserNew = new AspNetUser();
            aspnetuserNew.Id = Guid.NewGuid().ToString();
            aspnetuserNew.UserName = Credentials.Email;
            aspnetuserNew.Email = Credentials.Email;
            aspnetuserNew.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Credentials.Password);
            aspnetuserNew.CreatedDate = DateTime.Now;

            _context.AspNetUsers.Add(aspnetuserNew);
            await _context.SaveChangesAsync();

            var requestClientFetched = await _context.RequestClients.OrderBy(x => x.RequestClientId).LastOrDefaultAsync(m => m.Email == Credentials.Email);

            var userNew = new User();
            userNew.AspNetUserId = aspnetuserNew.Id;
            userNew.Email = aspnetuserNew.Email;
            
            if (requestClientFetched != null)
            {
                var requests = _context.Requests.Where(x => x.RequestId == requestClientFetched.RequestId).ToList();
                userNew.FirstName = requestClientFetched.FirstName;
                userNew.LastName = requestClientFetched?.LastName;
                userNew.Mobile = requestClientFetched?.PhoneNumber;
                userNew.Street = requestClientFetched?.Street;
                userNew.City = requestClientFetched?.City;
                userNew.State = requestClientFetched?.State;
                userNew.ZipCode = requestClientFetched?.ZipCode;
                userNew.StrMonth = requestClientFetched?.StrMonth;
                userNew.IntDate = requestClientFetched?.IntDate;
                userNew.IntYear = requestClientFetched?.IntYear;
                userNew.CreatedBy = "admin";
                userNew.CreatedDate = DateTime.Now;

                _context.Users.Add(userNew);
                await _context.SaveChangesAsync();

                foreach (var request in requests)
                {
                    request.UserId = userNew.UserId;
                    _context.Update(request);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                ViewBag.Message = "Looks like you are not eligible to create an account!";
                return View(Credentials);
            }

            return View("~/Views/Login/Index.cshtml");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(CreateAccountViewModel Credentials)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Please Try Again";
                return View(Credentials);
            }

            if (Credentials.Password != Credentials.ConfirmPassword)
            {
                ViewBag.Message = "Confirmed Password should match the Password";
                return View(Credentials);
            }

            var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == Credentials.Email);
            if (aspnetuserFetched == null)
            {
                ViewBag.Message = "Please enter correct Email";
                return View(Credentials);
            }
            aspnetuserFetched.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Credentials.Password);

            _context.AspNetUsers.Update(aspnetuserFetched);
            await _context.SaveChangesAsync();

            return View("~/Views/Login/Index.cshtml");
        }
    }
}

// $2a$10$2M/4FLCkMI6T5m1yEXg5LOFKro/jJfmGjsihcIe7jpzrWQSYF4sEm
// $2a$10$Jr4XT9Izy0HfbvJ9Z7HXnOzAPC0OxZpiFL9vkQiDpARg6H7KIChiK