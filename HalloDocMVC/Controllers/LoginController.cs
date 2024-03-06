
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Mvc;
using HalloDocServices.Interface;
using System.Text;
using HalloDocMVC.Auth;

namespace HalloDocMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IJwtService _jwtService;

        public LoginController(ILoginService loginService, IJwtService jwtService)
        {
            _loginService = loginService;
            _jwtService = jwtService;
        }

        public static string? decrypt(string emailToken)
        {
            if (emailToken == null)
            {
                return null;
            }
            else
            {
                byte[] encryptedEmail = Convert.FromBase64String(emailToken);
                string email = ASCIIEncoding.ASCII.GetString(encryptedEmail);
                return email;
            }
        }

        [CustomAuthorize("")]
        public IActionResult Index()
        {
            var cookie = Request.Cookies["jwt"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckLogin(LoginViewModel LoginInfo)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Invalid Email or Password";
                return View("Index");
            }

            var aspnetuser = await _loginService.CheckLogin(LoginInfo);

            if (aspnetuser == null)
            {
                ViewBag.Message = "Invalid Email or Password";
                return View("Index");
            }

            var jwtToken = _jwtService.GenerateJwtToken(aspnetuser);
            Response.Cookies.Append("jwt", jwtToken);

            return RedirectToAction("Dashboard", "Patient", new { id = aspnetuser.Id });

        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel Info)
        {
            bool isEmailValid = await _loginService.CheckUser(Info.Email);
            if (!isEmailValid)
            {
                ViewBag.Message = "There is no account with this email. Enter a valid email.";
                return View();
            }

            var receiver = Info.Email;

            var subject = "Reset Password from HalloDoc@Admin";
            var message = "Tap on link to reset password: http://localhost:5059/Login/ResetPassword";

            await _loginService.SendMail(receiver, subject, message);

            ViewBag.Message = "We have sent you a mail on your email address. Please reset your password from that link.";
            return View();
        }

        public IActionResult CreateAccount(string emailtoken)
        {
            
            CreateAccountViewModel Credentials = new CreateAccountViewModel();
            string? email = decrypt(emailtoken);
            if(email == null)
            {
                return NotFound();
            }
            Credentials.Email = email;
            return View(Credentials);
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

            string status = await _loginService.CreateAccount(Credentials);

            if (status.Equals("user exists"))
            {
                ViewBag.Message = "You already have an account with this email!";
                return View();
            }
            else if (status.Equals("not eligible"))
            {
                ViewBag.Message = "Looks like you are not eligible to create an account!";
                return View(Credentials);
            }
            else
            {
                return View("~/Views/Login/Index.cshtml");
            }
        }

        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            bool isTokenValid = await _loginService.ValidateToken(token);
            if (!isTokenValid)
            {
                return NotFound();
            }
            CreateAccountViewModel Credentials = new CreateAccountViewModel();
            Credentials.Email = email;
            return View(Credentials);
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

            bool isPasswordReset = await _loginService.ResetPassword(Credentials);

            if (!isPasswordReset)
            {
                ViewBag.Message = "Unfortunately there is no account with this email";
                return View(Credentials);
            }

            return View("~/Views/Login/Index.cshtml");
        }

        public IActionResult Logout ()
        {
            Response.Cookies.Delete("jwt");
            return View("Index");
        }
    }
}

// $2a$10$2M/4FLCkMI6T5m1yEXg5LOFKro/jJfmGjsihcIe7jpzrWQSYF4sEm
// $2a$10$Jr4XT9Izy0HfbvJ9Z7HXnOzAPC0OxZpiFL9vkQiDpARg6H7KIChiK