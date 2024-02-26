


using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace HalloDocMVC.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientController( IPatientService patientService)
        {
            _patientService = patientService;
        }

        public async Task<IActionResult> GoToDashboard(int UserId)
        {
            //var userFetched = await _context.Users.FindAsync(UserId);

            string aspnetuserId = await _patientService.GetAspNetUserIdByUserId(UserId);
            if (aspnetuserId.Equals(""))
            {
                return NotFound();
            }
            return RedirectToAction("Dashboard", new { id = aspnetuserId });
            
        }


        public async Task<IActionResult> Dashboard(string id)
        {
            /*if (string.IsNullOrWhiteSpace(id))
            {
                
            }*/

            /*var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == id);*/

            int userId = await _patientService.CheckUser(id);
            if (userId == 0)
            {
                return NotFound();
            }

            List<DashboardRequestViewModel> requestlist = await _patientService.GetRequestList(userId);

            /*var filecountgrouped = (from rwf in _context.RequestWiseFiles
                                    group rwf by rwf.RequestId into gp
                                    select new
                                    {
                                        RequestId = gp.Key,
                                        Cnt = gp.Count()
                                    }).ToList();

            var filecountgrouped = _context.RequestWiseFiles
                                    .GroupBy(rwf => rwf.RequestId)
                                    .Select(gp => new
                                    {
                                        RequestId = gp.Key,
                                        Cnt = gp.Count()
                                    })
                                    .ToList();



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
                *//*Debug.Print(($@"""{r.RequestId}"" ""{r.CreatedDate}"" ""{r.FileCount}"" "));*//*
                requestlist.Add(new DashboardRequestViewModel
                {
                    RequestId = r.RequestId,
                    CreateDate = DateOnly.FromDateTime(r.CreatedDate),
                    Status = r.Status,
                    Count = r.FileCount,
                    PhysicianId = r.PhysicianId
                });


            }*/

            //Pass user id to layout
            //ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
            //ViewBag.UserId = userFetched?.UserId;
            ViewBag.UserId = userId;

            return View(requestlist);
        }

        public async Task<IActionResult> ViewDocuments(int requestid)
        {

            List<RequestFileViewModel> requestfilelist = await _patientService.GetRequestFiles(requestid);

            /*var requestwisefilesFetched = _context.RequestWiseFiles.Where(x => x.RequestId == requestid).ToList();
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
            }*/
            ViewDocumentsViewModel vrvm = new()
            {
                RequestId = requestid,
                FileInfo = requestfilelist
            };



            // Pass user id to layout
            /*var requestFetched = await _context.Requests.FirstOrDefaultAsync(x => x.RequestId == requestid);
            if (requestFetched != null)
            {
                var userFetched = await _context.Users.FirstOrDefaultAsync(x => x.UserId == requestFetched.UserId);
                ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
                ViewBag.UserId = userFetched?.UserId;
            }*/
            int userId = await _patientService.GetUserIdByRequestId(requestid);
            ViewBag.UserId = userId;


            return View(vrvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadRequestFile(IEnumerable<IFormFile>? MultipleFiles, int requestid)
        {

            if (MultipleFiles != null && MultipleFiles?.Count() != 0)
            {
                await _patientService.UploadFiles(MultipleFiles, requestid);

                /*List<string> files = new List<string>();
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
                }*/
            }
            return RedirectToAction("ViewDocuments", new { requestid });
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var requestFileData = await _patientService.RequestFileData(id);

            //var requestwisefileFetched = _context.RequestWiseFiles.Find(id);
            string FilePath = "wwwroot\\" + requestFileData?.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
            //var path = "G:\\test\\hallodoccopy\\HalloDocMVC\\wwwroot\\" + requestwisefileFetched?.FileName;
            var bytes = System.IO.File.ReadAllBytes(path);
            var newfilename = Path.GetFileName(path);

            return File(bytes, "application/octet-stream", newfilename);

        }

        public async Task<IActionResult> DownloadAllFiles(int id)
        {
            var filesRow = await _patientService.GetRequestFiles(id);
            MemoryStream ms = new MemoryStream();
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                filesRow.ForEach(file =>
                {
                    string FilePath = "wwwroot\\" + file.FilePath;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                    ZipArchiveEntry zipEntry = zip.CreateEntry(Path.GetFileName(file.FileName));
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    using (Stream zipEntryStream = zipEntry.Open())
                    {
                        fs.CopyTo(zipEntryStream);
                    }
                });
            return File(ms.ToArray(), "application/zip", "download.zip");



            /*var filesRow = _context.RequestWiseFiles.Where(x => x.RequestId == id).ToList();
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
            return File(ms.ToArray(), "application/zip", "download.zip");*/
        }



        public async Task<IActionResult> Profile(int UserId)
        {
            var profileDetails = await _patientService.GetProfileDetails(UserId);
            
            ViewBag.UserId = profileDetails.UserId;
            return View(profileDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel ProfileDetails)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Patient/Profile.cshtml", ProfileDetails);
            }

            await _patientService.EditProfile(ProfileDetails);
            /*var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == ProfileDetails.Email);
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
            }*/

            return RedirectToAction("GoToDashboard", new { UserId = ProfileDetails.UserId });
        }

        public async Task<IActionResult> PatientRequest(int UserId)
        {
            PatientRequestViewModel PatientInfo = new PatientRequestViewModel();

            PatientInfo = await _patientService.GetPatientInfo(UserId);

            FamilyRequestViewModel frvm = new FamilyRequestViewModel();
            frvm.PatientInfo = PatientInfo;

            // Send data to view
            //ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
            ViewBag.UserId = UserId;
            ViewBag.RequestType = 2;

            return View(frvm);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientRequest(FamilyRequestViewModel frvm)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml");
            }

            int userRequestUserId = await _patientService.CreatePatientRequest(frvm);

            return RedirectToAction("GoToDashboard", new { UserId = userRequestUserId });



        }

        public IActionResult FamilyRequest(int UserId)
        {
            //var userFetched = await _context.Users.FindAsync(UserId);
            // Send data to view 
            //ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;


            ViewBag.UserId = UserId;
            ViewBag.RequestType = 3;

            return View("~/Views/Patient/PatientRequest.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyRequest(FamilyRequestViewModel frvm, int UserId)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/Index.cshtml");
            }

            await _patientService.CreateFamilyRequest(frvm, UserId);

            return RedirectToAction("GoToDashboard", new { UserId });
        }
    }
}
