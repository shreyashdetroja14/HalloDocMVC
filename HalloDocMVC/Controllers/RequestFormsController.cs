using HalloDocMVC.Models;
using HalloDocMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HalloDocMVC.Controllers
{
    public class RequestFormsController : Controller
    {
        private readonly HallodocContext _context;

        public RequestFormsController(HallodocContext context)
        {
            _context = context;
        }

        public AspNetUser CreateAspnetuser(PatientRequestViewModel prvm)
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

            /*userNew.IntDate = prvm.DOB?.Day;
            userNew.StrMonth = prvm.DOB?.ToString("MMMM");
            userNew.IntYear = prvm.DOB?.Year;*/

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
            if (prvm.Password == null || prvm.ConfirmPassword == null)
            {
                var emailFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == prvm.Email);
                if (emailFetched == null)
                {
                    ViewBag.Msg = "Please Enter And Confirm Password";
                    return View(prvm);
                }
            }

            if (ModelState.IsValid)
            {
                var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == prvm.Email);
                if (aspnetuserFetched == null)
                {
                    var aspnetuserNew = CreateAspnetuser(prvm);
                    _context.Add(aspnetuserNew);
                    await _context.SaveChangesAsync();

                    var userNew = CreateUser(prvm, aspnetuserNew);
                    _context.Add(userNew);
                    await _context.SaveChangesAsync();
                }

                aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == prvm.Email);
                var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.Email == prvm.Email);
                if (userFetched != null && aspnetuserFetched != null)
                {
                    var requestNew = new Request();

                    requestNew.RequestTypeId = 2;
                    requestNew.UserId = userFetched.UserId;
                    requestNew.FirstName = prvm.FirstName;
                    requestNew.LastName = prvm.LastName;
                    requestNew.PhoneNumber = prvm.PhoneNumber;
                    requestNew.Email = prvm.Email;
                    requestNew.Status = 1;
                    requestNew.CreatedDate = DateTime.Now;
                    requestNew.IsUrgentEmailSent = false;
                    requestNew.PatientAccountId = aspnetuserFetched?.Id;
                    requestNew.CreatedUserId = userFetched.UserId;

                    _context.Add(requestNew);
                    await _context.SaveChangesAsync();

                    var requestClientNew = CreateRequestClient(prvm, requestNew);

                    _context.Add(requestClientNew);
                    await _context.SaveChangesAsync();

                    if (prvm.MultipleFiles != null)
                    {
                        /*foreach (var UploadFile in prvm.MultipleFiles)
                        {
                            string FilePath = "wwwroot\\Upload\\" + requestNew.RequestId;
                            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";

                            string fileNameWithPath = Path.Combine(path, newfilename);
                            prvm.File = FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + newfilename;

                            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                            {
                                UploadFile.CopyTo(stream);
                            }
                            var reqwisefileNew = new RequestWiseFile();
                            reqwisefileNew.RequestId = requestNew.RequestId;
                            reqwisefileNew.FileName = prvm.File;
                            reqwisefileNew.CreatedDate = DateTime.Now;

                            _context.Add(reqwisefileNew);
                            await _context.SaveChangesAsync();
                        }*/

                        List<string> files = UploadFilesToServer(prvm.MultipleFiles, requestNew.RequestId);
                        foreach (string file in files)
                        {
                            var reqwisefileNew = new RequestWiseFile();
                            reqwisefileNew.RequestId = requestNew.RequestId;
                            reqwisefileNew.FileName = file;
                            reqwisefileNew.CreatedDate = DateTime.Now;

                            _context.Add(reqwisefileNew);
                            await _context.SaveChangesAsync();
                        }

                    }
                }
                return RedirectToAction("SubmitRequest");
            }
            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult FamilyRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyRequest(FamilyRequestViewModel frvm)
        {
            if (ModelState.IsValid)
            {
                var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == frvm.PatientInfo.Email);
                if (aspnetuserFetched == null)
                {
                    // SEND MAIL WITH REGISTER LINK
                }

                var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.Email == frvm.PatientInfo.Email);
                var requestorUser = await _context.Users.FirstOrDefaultAsync(m => m.Email == frvm.FamilyEmail);

                var requestNew = new Request();

                requestNew.RequestTypeId = 3;
                requestNew.UserId = userFetched?.UserId;
                requestNew.FirstName = frvm.FamilyFirstName;
                requestNew.LastName = frvm.FamilyLastName;
                requestNew.PhoneNumber = frvm.FamilyPhoneNumber;
                requestNew.Email = frvm.FamilyEmail;
                requestNew.Status = 1;
                requestNew.CreatedDate = DateTime.Now;
                requestNew.IsUrgentEmailSent = false;
                requestNew.RelationName = frvm.FamilyRelation;
                requestNew.PatientAccountId = aspnetuserFetched?.Id;
                requestNew.CreatedUserId = requestorUser?.UserId;

                _context.Add(requestNew);
                await _context.SaveChangesAsync();

                var requestClientNew = CreateRequestClient(frvm.PatientInfo, requestNew);

                _context.Add(requestClientNew);
                await _context.SaveChangesAsync();

                if (frvm.PatientInfo.MultipleFiles != null)
                {

                    List<string> files = UploadFilesToServer(frvm.PatientInfo.MultipleFiles, requestNew.RequestId);
                    foreach (string file in files)
                    {
                        var reqwisefileNew = new RequestWiseFile();
                        reqwisefileNew.RequestId = requestNew.RequestId;
                        reqwisefileNew.FileName = file;
                        reqwisefileNew.CreatedDate = DateTime.Now;

                        _context.Add(reqwisefileNew);
                        await _context.SaveChangesAsync();
                    }

                }

                return RedirectToAction("SubmitRequest");
            }
            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult ConciergeRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConciergeRequest(ConciergeRequestViewModel crvm)
        {
            if (ModelState.IsValid)
            {
                var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == crvm.PatientInfo.Email);
                if (aspnetuserFetched == null)
                {
                    // SEND MAIL WITH REGISTER LINK
                }
                var conciergeFetched = await _context.Requests.FirstOrDefaultAsync(m => m.Email == crvm.ConciergeEmail && m.RequestTypeId == 4);
                if (conciergeFetched == null)
                {
                    var conciergeNew = new Concierge();
                    conciergeNew.ConciergeName = crvm.ConciergePropertyName;
                    conciergeNew.Address = crvm.ConciergeStreet + ", " + crvm.ConciergeCity + ", " + crvm.ConciergeState + ", " + crvm.ConciergeZipCode;
                    conciergeNew.Street = crvm.ConciergeStreet;
                    conciergeNew.City = crvm.ConciergeCity;
                    conciergeNew.State = crvm.ConciergeState;
                    conciergeNew.ZipCode = crvm.ConciergeZipCode;
                    conciergeNew.CreatedDate = DateTime.Now;
                    conciergeNew.RegionId = 1;

                    _context.Add(conciergeNew);
                    await _context.SaveChangesAsync();
                }
                var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.Email == crvm.PatientInfo.Email);
                var requestorUser = await _context.Users.FirstOrDefaultAsync(m => m.Email == crvm.ConciergeEmail);

                var requestNew = new Request();

                requestNew.RequestTypeId = 4;
                requestNew.UserId = userFetched?.UserId;
                requestNew.FirstName = crvm.ConciergeFirstName;
                requestNew.LastName = crvm.ConciergeLastName;
                requestNew.PhoneNumber = crvm.ConciergePhoneNumber;
                requestNew.Email = crvm.ConciergeEmail;
                requestNew.Status = 1;
                requestNew.CreatedDate = DateTime.Now;
                requestNew.IsUrgentEmailSent = false;
                requestNew.PatientAccountId = aspnetuserFetched?.Id;
                requestNew.CreatedUserId = requestorUser?.UserId;

                _context.Add(requestNew);
                await _context.SaveChangesAsync();

                var requestClientNew = CreateRequestClient(crvm.PatientInfo, requestNew);

                _context.Add(requestClientNew);
                await _context.SaveChangesAsync();

                return RedirectToAction("SubmitRequest");
            }
            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult BusinessRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BusinessRequest(BusinessRequestViewModel brvm)
        {
            if (ModelState.IsValid)
            {
                var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == brvm.PatientInfo.Email);
                if (aspnetuserFetched == null)
                {
                    // SEND MAIL WITH REGISTER LINK
                }
                var businessFetched = await _context.Businesses.FirstOrDefaultAsync(m => m.PhoneNumber == brvm.BusinessPhoneNumber && m.Name == brvm.BusinessName);
                if (businessFetched == null)
                {
                    var businessNew = new Business();
                    businessNew.Name = brvm.BusinessName;
                    businessNew.PhoneNumber = brvm.BusinessPhoneNumber;
                    businessNew.CreatedBy = "admin";
                    businessNew.CreatedDate = DateTime.Now;

                    _context.Add(businessNew);
                    await _context.SaveChangesAsync();
                }

                var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.Email == brvm.PatientInfo.Email);
                var requestorUser = await _context.Users.FirstOrDefaultAsync(m => m.Email == brvm.BusinessEmail);

                var requestNew = new Request();

                requestNew.RequestTypeId = 1;
                requestNew.UserId = userFetched?.UserId;
                requestNew.FirstName = brvm.BusinessFirstName;
                requestNew.LastName = brvm.BusinessLastName;
                requestNew.PhoneNumber = brvm.BusinessPhoneNumber;
                requestNew.Email = brvm.BusinessEmail;
                requestNew.Status = 1;
                requestNew.CreatedDate = DateTime.Now;
                requestNew.IsUrgentEmailSent = false;
                requestNew.PatientAccountId = aspnetuserFetched?.Id;
                requestNew.CreatedUserId = requestorUser?.UserId;

                _context.Add(requestNew);
                await _context.SaveChangesAsync();

                var requestClientNew = CreateRequestClient(brvm.PatientInfo, requestNew);

                _context.Add(requestClientNew);
                await _context.SaveChangesAsync();

                businessFetched = await _context.Businesses.FirstOrDefaultAsync(m => m.PhoneNumber == brvm.BusinessPhoneNumber && m.Name == brvm.BusinessName);

                var requestBusinessNew = new RequestBusiness();
                requestBusinessNew.RequestId = requestNew.RequestId;
                if (businessFetched != null)
                {
                    requestBusinessNew.BusinessId = businessFetched.BusinessId;
                }

                _context.Add(requestBusinessNew);
                await _context.SaveChangesAsync();

                return RedirectToAction("SubmitRequest");
            }
            return View("~/Views/Home/Index.cshtml");
        }


        public async Task<IActionResult> CheckUserAccount(string? email)
        {
            if (email != null)
            {
                var emailFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == email);
                if (emailFetched != null)
                {
                    return Json(new { status = "valid" });
                }
            }
            return Json(new { status = "invalid" });
        }

    }
}
