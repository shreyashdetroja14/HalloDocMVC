using HalloDocMVC.Auth;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace HalloDocMVC.Controllers
{

    [CustomAuthorize("patient")]
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

            ViewDocumentsViewModel vrvm = new()
            {
                RequestId = requestid,
                FileInfo = requestfilelist
            };

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
            }
            return RedirectToAction("ViewDocuments", new { requestid });
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var requestFileData = await _patientService.RequestFileData(id);

            string FilePath = "wwwroot\\" + requestFileData?.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
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
