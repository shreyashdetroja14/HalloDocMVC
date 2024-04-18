
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Mvc;
using HalloDocServices.Interface;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using HalloDocServices.Implementation;

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

        #region LOGIN

        public IActionResult Index()
        {
            string token = Request.Cookies["jwt"] ?? "";
            if (_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                var roleclaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
                
                if (roleclaim?.Value == "admin")
                {
                    return RedirectToRoute("Dashboard");
                }
                else if (roleclaim?.Value == "physician")
                {
                    return RedirectToRoute("Dashboard");
                }
                else if (roleclaim?.Value == "patient")
                {
                    return RedirectToAction("Dashboard", "Patient");
                }
            }

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

            if (aspnetuser.Id == null)
            {
                ViewBag.Message = "Invalid Email or Password";
                return View("Index");
            }

            var jwtToken = _jwtService.GenerateJwtToken(aspnetuser);
            Response.Cookies.Append("jwt", jwtToken);

            string role = aspnetuser.AspNetUserRoles.FirstOrDefault()?.Role.Name??"";

            if (role == "admin")
            {
                //return RedirectToAction("Index", "AdminDashboard");
                return RedirectToRoute("Dashboard");
            }
            else if(role == "physician")
            {
                return RedirectToRoute("Dashboard");
            }
            else if(role == "patient")
            {
                return RedirectToAction("Dashboard", "Patient");
            }
            else
            {
                return View("Index");
            }

        }

        #endregion

        #region FORGOT PASSWORD

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

            Info.EmailToken = _jwtService.GenerateEmailToken(Info.Email, isExpireable: true);
            await _loginService.SendMail(Info);

            ViewBag.Message = "We have sent you a mail on your email address. Please reset your password from that link.";
            return View();
        }

        #endregion

        #region CREATE ACCOUNT

        public IActionResult CreateAccount(string emailtoken)
        {
            if (_jwtService.ValidateToken(emailtoken, out JwtSecurityToken jwtToken))
            {
                Response.Cookies.Append("emailToken", emailtoken);
                string email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value.ToString() ?? "";

                CreateAccountViewModel Credentials = new CreateAccountViewModel();
                Credentials.Email = email;
                return View(Credentials);
            }

            return NotFound();

            
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

        #endregion

        #region RESET PASSWORD

        public IActionResult ResetPassword(string emailtoken)
        {
            if (_jwtService.ValidateToken(emailtoken, out JwtSecurityToken jwtToken))
            {
                Response.Cookies.Append("emailToken", emailtoken);
                string email = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value.ToString() ?? "";

                CreateAccountViewModel Credentials = new CreateAccountViewModel();
                Credentials.Email = email;
                return View(Credentials);
            }

            return NotFound();
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

        #endregion

        #region LOGOUT 

        public IActionResult Logout ()
        {
            Response.Cookies.Delete("jwt");
            //return View("Index");
            return RedirectToAction("Index");
        }

        #endregion

        #region VALIDATE COOKIE (JWT TOKEN)

        public IActionResult ValidateCookie() 
        {
            string token = Request.Cookies["jwt"] ?? "";
            if (!_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return StatusCode(401, "Unauthorized");
            }
            return StatusCode(200, "Cookie is valid");
        }

        #endregion
    }
}

