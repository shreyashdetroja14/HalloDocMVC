using HalloDocMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO.Compression;

namespace HalloDocMVC.Controllers
{
    public class PatientController : Controller
    {
        private readonly HallodocContext _context;

        public PatientController(HallodocContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GoToDashboard(int UserId)
        {
            var userFetched = await _context.Users.FindAsync(UserId);
            if (userFetched != null)
            {
                var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Id == userFetched.AspNetUserId);
                if (aspnetuserFetched != null)
                {
                    return RedirectToAction("Dashboard", new { id = aspnetuserFetched.Id });
                }
            }
            return NotFound();
        }


        public async Task<IActionResult> Dashboard(string id)
        {
            /*if (string.IsNullOrWhiteSpace(id))
            {
                
            }*/

            var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == id);

            if (userFetched == null)
            {
                return NotFound();
            }
            var filecountgrouped = (from rwf in _context.RequestWiseFiles
                                    group rwf by rwf.RequestId into gp
                                    select new
                                    {
                                        RequestId = gp.Key,
                                        Cnt = gp.Count()
                                    }).ToList();


            var data = from requests in _context.Requests.ToList()
                       join count in filecountgrouped
                       on requests.RequestId equals count.RequestId into joined
                       from j in joined.DefaultIfEmpty()
                       where requests.UserId == userFetched?.UserId
                       orderby requests.CreatedDate descending
                       select new
                       {
                           RequestId = requests.RequestId,
                           CreatedDate = requests.CreatedDate,
                           Status = requests.Status,
                           FileCount = j?.Cnt ?? 0,
                           PhysicianId = requests.PhysicianId ?? 0,
                       };

            List<DashboardRequestViewModel> requestlist = new List<DashboardRequestViewModel>();
            foreach (var r in data)
            {
                /*Debug.Print(($@"""{r.RequestId}"" ""{r.CreatedDate}"" ""{r.FileCount}"" "));*/
                requestlist.Add(new DashboardRequestViewModel
                {
                    RequestId = r.RequestId,
                    CreateDate = DateOnly.FromDateTime(r.CreatedDate),
                    Status = r.Status,
                    Count = r.FileCount,
                    PhysicianId = r.PhysicianId
                });


            }

            //Pass user id to layout
            ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
            ViewBag.UserId = userFetched?.UserId;

            return View(requestlist);
        }

        public async Task<IActionResult> ViewDocuments(int requestid)
        {
            var requestwisefilesFetched = _context.RequestWiseFiles.Where(x => x.RequestId == requestid).ToList();
            var data = from requests in _context.Requests.ToList()
                       join files in requestwisefilesFetched
                       on requests.RequestId equals files.RequestId
                       select new
                       {
                           requests,
                           files
                       };

            List<RequestFileViewModel> requestfilelist = new List<RequestFileViewModel>();
            foreach (var row in data)
            {
                requestfilelist.Add(new RequestFileViewModel
                {
                    FileId = row.files.RequestWiseFileId,
                    FileName = Path.GetFileName(row.files.FileName),
                    Uploader = row.requests.FirstName + " " + row.requests.LastName,
                    UploadDate = DateOnly.FromDateTime(row.files.CreatedDate),
                    FilePath = row.files.FileName
                });
            }
            ViewDocumentsViewModel vrvm = new()
            {
                RequestId = requestid,
                FileInfo = requestfilelist
            };



            // Pass user id to layout
            var requestFetched = await _context.Requests.FirstOrDefaultAsync(x => x.RequestId == requestid);
            if (requestFetched != null)
            {
                var userFetched = await _context.Users.FirstOrDefaultAsync(x => x.UserId == requestFetched.UserId);
                ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
                ViewBag.UserId = userFetched?.UserId;
            }



            return View(vrvm);
        }

        public async Task<IActionResult> UploadRequestFile(IEnumerable<IFormFile>? MultipleFiles, int requestid)
        {

            if (MultipleFiles != null)
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
                foreach (string file in files)
                {
                    var reqwisefileNew = new RequestWiseFile();
                    reqwisefileNew.RequestId = requestid;
                    reqwisefileNew.FileName = file;
                    reqwisefileNew.CreatedDate = DateTime.Now;

                    _context.Add(reqwisefileNew);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("ViewDocuments", new { requestid });
        }

        public IActionResult DownloadFile(int id)
        {
            var requestwisefileFetched = _context.RequestWiseFiles.Find(id);
            string FilePath = "wwwroot\\" + requestwisefileFetched?.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
            //var path = "G:\\test\\hallodoccopy\\HalloDocMVC\\wwwroot\\" + requestwisefileFetched?.FileName;
            var bytes = System.IO.File.ReadAllBytes(path);
            var newfilename = Path.GetFileName(path);

            return File(bytes, "application/octet-stream", newfilename);
        }

        public IActionResult DownloadAllFiles(int id)
        {

            var requestFetched = _context.RequestWiseFiles.Find(id);
            if (requestFetched != null)
            {
                id = requestFetched.RequestId;
            }

            var filesRow = _context.RequestWiseFiles.Where(x => x.RequestId == id).ToList();
            MemoryStream ms = new MemoryStream();
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                filesRow.ForEach(file =>
                {
                    string FilePath = "wwwroot\\" + file.FileName;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                    //string path = "G:\\test\\hallodoccopy\\HalloDocMVC\\wwwroot\\" + file.FileName;


                    ZipArchiveEntry zipEntry = zip.CreateEntry(Path.GetFileName(file.FileName));
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    using (Stream zipEntryStream = zipEntry.Open())
                    {
                        fs.CopyTo(zipEntryStream);
                    }
                });
            return File(ms.ToArray(), "application/zip", "download.zip");
        }



        public async Task<IActionResult> Profile(int UserId)
        {
            var userFetched = await _context.Users.FirstOrDefaultAsync(x => x.UserId == UserId);
            if (userFetched != null)
            {
                ProfileViewModel ProfileDetails = new ProfileViewModel();
                ProfileDetails.FirstName = userFetched.FirstName;
                ProfileDetails.LastName = userFetched.LastName;
                if (userFetched.IntDate.HasValue && userFetched.IntYear.HasValue && userFetched.StrMonth != null)
                {
                    DateTime monthDateTime = DateTime.ParseExact(userFetched.StrMonth, "MMMM", CultureInfo.InvariantCulture);
                    int month = monthDateTime.Month;
                    DateOnly date = new DateOnly((int)userFetched.IntYear, month, userFetched.IntDate.Value);
                    ProfileDetails.DOB = date.ToString("yyyy-MM-dd");
                }
                ProfileDetails.PhoneNumber = userFetched.Mobile;
                ProfileDetails.Email = userFetched.Email;
                ProfileDetails.Street = userFetched.Street;
                ProfileDetails.City = userFetched.City;
                ProfileDetails.State = userFetched.State;
                ProfileDetails.ZipCode = userFetched.ZipCode;



                // Send userid to view 
                ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
                ViewBag.UserId = userFetched?.UserId;

                return View(ProfileDetails);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel ProfileDetails)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Patient/Profile.cshtml", ProfileDetails);
            }
            var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == ProfileDetails.Email);
            if (aspnetuserFetched != null)
            {
                aspnetuserFetched.PhoneNumber = ProfileDetails.PhoneNumber;

                _context.Update(aspnetuserFetched);
                await _context.SaveChangesAsync();
            }
            var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.Email == ProfileDetails.Email);
            if (userFetched != null)
            {
                userFetched.FirstName = ProfileDetails.FirstName;
                userFetched.LastName = ProfileDetails.LastName;

                DateTime dateTime = DateTime.ParseExact(ProfileDetails.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                userFetched.IntYear = dateTime.Year;
                userFetched.StrMonth = dateTime.ToString("MMMM");
                userFetched.IntDate = dateTime.Day;

                userFetched.Mobile = ProfileDetails.PhoneNumber;
                userFetched.Email = ProfileDetails.Email;
                userFetched.Street = ProfileDetails.Street;
                userFetched.City = ProfileDetails.City;
                userFetched.State = ProfileDetails.State;
                userFetched.ZipCode = ProfileDetails.ZipCode;

                _context.Users.Update(userFetched);
                await _context.SaveChangesAsync();
            }

            var requestsFetched = await _context.Requests.Where(m => m.Email == ProfileDetails.Email).ToListAsync();
            if (requestsFetched.Count > 0)
            {
                foreach (var request in requestsFetched)
                {
                    request.FirstName = ProfileDetails.FirstName;
                    request.LastName = ProfileDetails.LastName;
                    request.PhoneNumber = ProfileDetails.PhoneNumber;

                    _context.Requests.Update(request);

                }
                await _context.SaveChangesAsync();
            }

            var requestClientsFetched = await _context.RequestClients.Where(m => m.Email == ProfileDetails.Email).ToListAsync();
            if (requestClientsFetched.Count > 0)
            {
                foreach (var requestClient in requestClientsFetched)
                {
                    requestClient.FirstName = ProfileDetails.FirstName;
                    requestClient.LastName = ProfileDetails.LastName;
                    requestClient.PhoneNumber = ProfileDetails.PhoneNumber;
                    requestClient.Email = ProfileDetails.Email;

                    DateTime dateTime = DateTime.ParseExact(ProfileDetails.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    requestClient.IntYear = dateTime.Year;
                    requestClient.StrMonth = dateTime.ToString("MMMM");
                    requestClient.IntDate = dateTime.Day;

                    _context.RequestClients.Update(requestClient);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Dashboard", new { id = aspnetuserFetched?.Id });
        }

        public async Task<IActionResult> PatientRequest(int UserId)
        {
            PatientRequestViewModel PatientInfo = new PatientRequestViewModel();

            var userFetched = await _context.Users.FindAsync(UserId);
            if (userFetched != null)
            {
                PatientInfo.FirstName = userFetched.FirstName;
                PatientInfo.LastName = userFetched.LastName;
                PatientInfo.Email = userFetched.Email;
                PatientInfo.PhoneNumber = userFetched.Mobile;
                if (userFetched.IntDate.HasValue && userFetched.IntYear.HasValue && userFetched.StrMonth != null)
                {
                    DateTime monthDateTime = DateTime.ParseExact(userFetched.StrMonth, "MMMM", CultureInfo.InvariantCulture);
                    int month = monthDateTime.Month;
                    DateOnly date = new DateOnly((int)userFetched.IntYear, month, userFetched.IntDate.Value);
                    PatientInfo.DOB = date.ToString("yyyy-MM-dd");
                }
            }

            FamilyRequestViewModel frvm = new FamilyRequestViewModel();
            frvm.PatientInfo = PatientInfo;

            // Send data to view
            ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
            ViewBag.UserId = userFetched?.UserId;
            ViewBag.RequestType = 2;
            return View(frvm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientRequest(FamilyRequestViewModel frvm)
        {
            if (ModelState.IsValid)
            {
                var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == frvm.PatientInfo.Email);
                var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.Email == frvm.PatientInfo.Email);
                if (userFetched != null && aspnetuserFetched != null)
                {
                    var requestNew = new Request();

                    requestNew.RequestTypeId = 2;
                    requestNew.UserId = userFetched.UserId;
                    requestNew.FirstName = frvm.PatientInfo.FirstName;
                    requestNew.LastName = frvm.PatientInfo.LastName;
                    requestNew.PhoneNumber = frvm.PatientInfo.PhoneNumber;
                    requestNew.Email = frvm.PatientInfo.Email;
                    requestNew.Status = 1;
                    requestNew.CreatedDate = DateTime.Now;
                    requestNew.IsUrgentEmailSent = false;
                    requestNew.PatientAccountId = aspnetuserFetched?.Id;
                    requestNew.CreatedUserId = userFetched.UserId;

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
                    return RedirectToAction("Dashboard", new { id = aspnetuserFetched?.Id });
                }
            }
            return View("~/Views/Home/Index.cshtml");
        }

        public async Task<IActionResult> FamilyRequest(int UserId)
        {
            var userFetched = await _context.Users.FindAsync(UserId);
            // Send data to view 
            ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
            ViewBag.UserId = userFetched?.UserId;
            ViewBag.RequestType = 3;

            return View("~/Views/Patient/PatientRequest.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyRequest(FamilyRequestViewModel frvm, int UserId)
        {
            if (ModelState.IsValid)
            {
                var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == frvm.PatientInfo.Email);
                var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.Email == frvm.PatientInfo.Email);
                var requestorUser = await _context.Users.FindAsync(UserId);
                if (requestorUser != null)
                {
                    var requestNew = new Request();

                    requestNew.RequestTypeId = 3;
                    requestNew.UserId = userFetched?.UserId;
                    requestNew.FirstName = requestorUser?.FirstName;
                    requestNew.LastName = requestorUser?.LastName;
                    requestNew.PhoneNumber = requestorUser?.Mobile;
                    requestNew.Email = requestorUser?.Email;
                    requestNew.Status = 1;
                    requestNew.CreatedDate = DateTime.Now;
                    requestNew.IsUrgentEmailSent = false;
                    requestNew.PatientAccountId = aspnetuserFetched?.Id;
                    requestNew.CreatedUserId = requestorUser?.UserId;
                    requestNew.RelationName = frvm.FamilyRelation;

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
                    return RedirectToAction("GoToDashboard", new { UserId = requestorUser?.UserId });
                }
            }
            return View("~/Views/Home/Index.cshtml");
        }
    }
}
