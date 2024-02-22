using HalloDocEntities.Data;

using HalloDocEntities.Models;
using HalloDocServices.Implementation;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HalloDocMVC.Controllers
{
    public class RequestFormsController : Controller
    {
        private readonly HalloDocContext _context;
        private readonly IRequestFormService _requestFormService;

        public RequestFormsController(HalloDocContext context, IRequestFormService requestFormService)
        {
            _context = context;
            _requestFormService = requestFormService;
        }

        /*public AspNetUser CreateAspnetuser(PatientRequestViewModel prvm)
        {
            var aspnetuserNew = new AspNetUser();

            aspnetuserNew.Id = Guid.NewGuid().ToString();

            aspnetuserNew.UserName = prvm.Email;
            aspnetuserNew.PasswordHash = prvm.Password;
            aspnetuserNew.Email = prvm.Email;
            aspnetuserNew.PhoneNumber = prvm.PhoneNumber;
            aspnetuserNew.CreatedDate = DateTime.Now;

            return aspnetuserNew;
        }
        public User CreateUser(PatientRequestViewModel prvm, AspNetUser aspnetuserNew)
        {
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

            if (prvm.DOB != null)
            {
                DateTime dateTime = DateTime.ParseExact(prvm.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                userNew.IntYear = dateTime.Year;
                userNew.StrMonth = dateTime.ToString("MMMM");
                userNew.IntDate = dateTime.Day;
            }

            *//*userNew.IntDate = prvm.DOB?.Day;
            userNew.StrMonth = prvm.DOB?.ToString("MMMM");
            userNew.IntYear = prvm.DOB?.Year;*//*

            userNew.CreatedBy = "admin";
            userNew.CreatedDate = DateTime.Now;

            return userNew;
        }
        public RequestClient CreateRequestClient(PatientRequestViewModel PatientInfo, Request requestNew)
        {
            var requestClientNew = new RequestClient();
            requestClientNew.RequestId = requestNew.RequestId;
            requestClientNew.FirstName = PatientInfo.FirstName;
            requestClientNew.LastName = PatientInfo.LastName;
            requestClientNew.PhoneNumber = PatientInfo.PhoneNumber;
            requestClientNew.Location = PatientInfo.Room;
            requestClientNew.Address = PatientInfo.Street + ", " + PatientInfo.City + ", " + PatientInfo.State + ", " + PatientInfo.ZipCode;
            requestClientNew.NotiMobile = PatientInfo.PhoneNumber;
            requestClientNew.NotiEmail = PatientInfo.Email;
            requestClientNew.Notes = PatientInfo.Symptoms;
            requestClientNew.Email = PatientInfo.Email;

            if (PatientInfo.DOB != null)
            {
                DateTime dateTime = DateTime.ParseExact(PatientInfo.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                requestClientNew.IntYear = dateTime.Year;
                requestClientNew.StrMonth = dateTime.ToString("MMMM");
                requestClientNew.IntDate = dateTime.Day;
            }

            requestClientNew.Street = PatientInfo.Street;
            requestClientNew.City = PatientInfo.City;
            requestClientNew.State = PatientInfo.State;
            requestClientNew.ZipCode = PatientInfo.ZipCode;
            return requestClientNew;
        }

        public List<string> UploadFilesToServer(IEnumerable<IFormFile> MultipleFiles, int requestid)
        {
            List<string> files = new List<string>();
            foreach (var UploadFile in MultipleFiles)
            {
                string FilePath = "wwwroot\\Upload\\" + requestid;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";

                string fileNameWithPath = Path.Combine(path, newfilename);
                files.Add(FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + newfilename);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
            }
            return files;
        }*/

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
            if (prvm.Password == null || prvm.ConfirmPassword == null)
            {
                bool isUserExist = await _requestFormService.CheckUser(prvm.Email);
                if (!isUserExist)
                {
                    ViewBag.Msg = "Please Enter And Confirm Password";
                    return View(prvm);
                }
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml");
            }

            bool isrequestcreated = await _requestFormService.CreatePatientRequest(prvm);
            if (isrequestcreated)
            {
                return RedirectToAction("SubmitRequest");
            }
            return RedirectToAction("Index", "Home");
            
        }

        public IActionResult FamilyRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyRequest(FamilyRequestViewModel frvm)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            
            bool isUserExists = await _requestFormService.CheckUser(frvm.PatientInfo.Email);
            if (!isUserExists)
            {
                var receiver = frvm.PatientInfo.Email;

                var subject = "Create Account from HalloDoc@Admin";
                var message = "Tap on link to create account on HalloDoc: http://localhost:5059/Login/CreateAccount";

                await _requestFormService.SendMail(receiver, subject, message);
            }

            bool isrequestcreated = await _requestFormService.CreateFamilyRequest(frvm);
            if (isrequestcreated)
            {
                return RedirectToAction("SubmitRequest");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ConciergeRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConciergeRequest(ConciergeRequestViewModel crvm)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            
            bool isUserExists = await _requestFormService.CheckUser(crvm.PatientInfo.Email);
            if (!isUserExists)
            {
                var receiver = crvm.PatientInfo.Email;

                var subject = "Create Account from HalloDoc@Admin";
                var message = "Tap on link to create account on HalloDoc: http://localhost:5059/Login/CreateAccount";

                await _requestFormService.SendMail(receiver, subject, message);
            }

            bool isrequestcreated = await _requestFormService.CreateConciergeRequest(crvm);
            if (isrequestcreated)
            {
                return RedirectToAction("SubmitRequest");
            }
            return RedirectToAction("Index", "Home");

        }

        public IActionResult BusinessRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BusinessRequest(BusinessRequestViewModel brvm)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml");
            }
            bool isUserExists = await _requestFormService.CheckUser(brvm.PatientInfo.Email);
            if (!isUserExists)
            {
                var receiver = brvm.PatientInfo.Email;

                var subject = "Create Account from HalloDoc@Admin";
                var message = "Tap on link to create account on HalloDoc: http://localhost:5059/Login/CreateAccount";

                await _requestFormService.SendMail(receiver, subject, message);
            }

            bool isrequestcreated = await _requestFormService.CreateBusinessRequest(brvm);
            if (isrequestcreated)
            {
                return RedirectToAction("SubmitRequest");
            }
            return RedirectToAction("Index", "Home");
            
        }


        public async Task<IActionResult> CheckUserAccount(string? email)
        {
            if (email != null)
            {
                bool isUserExist = await _requestFormService.CheckUser(email);
                if (isUserExist)
                {
                    return Json(new { status = "valid" });
                }
            }
            return Json(new { status = "invalid" });
        }

    }
}
