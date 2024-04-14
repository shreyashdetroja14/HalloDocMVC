using HalloDocEntities.Models;
using HalloDocMVC.Auth;
using HalloDocServices.Implementation;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;

namespace HalloDocMVC.Controllers
{

    
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IJwtService _jwtService;

        public PatientController( IPatientService patientService, IJwtService jwtService)
        {
            _patientService = patientService;
            _jwtService = jwtService;
        }

        #region Encode_Decode
        public string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }
        public string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }
        #endregion

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

        [CustomAuthorize("patient")]
        public async Task<IActionResult> Dashboard(string id)
        {
            string token = Request.Cookies["jwt"] ?? "";
            if (_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                var aspnetuseridclaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "aspnetuserId");
                id = aspnetuseridclaim?.Value ?? "";
            }

            int userId = await _patientService.CheckUser(id);
            if (userId == 0)
            {
                return NotFound();
            }

            List<DashboardRequestViewModel> requestlist = await _patientService.GetRequestList(userId);

            ViewBag.UserId = userId;

            return View(requestlist);
        }

        [CustomAuthorize("patient")]
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

        [CustomAuthorize("patient")]
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

        [CustomAuthorize("patient")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var requestFileData = await _patientService.RequestFileData(id);

            string FilePath = "wwwroot\\" + requestFileData?.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
            var bytes = System.IO.File.ReadAllBytes(path);
            var newfilename = Path.GetFileName(path);

            return File(bytes, "application/octet-stream", newfilename);

        }

        [CustomAuthorize("patient")]
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

        [CustomAuthorize("patient")]
        public async Task<IActionResult> Profile(int UserId)
        {
            var profileDetails = await _patientService.GetProfileDetails(UserId);
            
            ViewBag.UserId = profileDetails.UserId;
            return View(profileDetails);
        }

        [CustomAuthorize("patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel ProfileDetails)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Patient/Profile.cshtml", ProfileDetails);
            }

            await _patientService.EditProfile(ProfileDetails);

            return RedirectToAction("Profile", new { UserId = ProfileDetails.UserId });
        }

        [CustomAuthorize("patient")]
        public async Task<IActionResult> PatientRequest(int UserId)
        {
            PatientRequestViewModel PatientInfo = new PatientRequestViewModel();

            PatientInfo = await _patientService.GetPatientInfo(UserId);

            FamilyRequestViewModel frvm = new FamilyRequestViewModel();
            frvm.PatientInfo = PatientInfo;

            ViewBag.RegionList = _patientService.GetRegionList();

            // Send data to view
            //ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
            ViewBag.UserId = UserId;
            ViewBag.RequestType = 2;

            return View(frvm);
        }


        [CustomAuthorize("patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientRequest(FamilyRequestViewModel frvm)
        {
            if (!ModelState.IsValid)
            {
                return View(frvm);
            }

            int userRequestUserId = await _patientService.CreatePatientRequest(frvm);

            return RedirectToAction("GoToDashboard", new { UserId = userRequestUserId });

        }

        [CustomAuthorize("patient")]
        public IActionResult FamilyRequest(int UserId)
        {
            ViewBag.UserId = UserId;
            ViewBag.RequestType = 3;

            ViewBag.RegionList = _patientService.GetRegionList();

            return View("~/Views/Patient/PatientRequest.cshtml");
        }

        [CustomAuthorize("patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyRequest(FamilyRequestViewModel frvm, int UserId)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UserId = UserId;
                ViewBag.RequestType = 3;
                return View("~/Views/Patient/PatientRequest.cshtml", frvm);
            }

            //frvm.PatientInfo.EmailToken = _jwtService.GenerateEmailToken(frvm.PatientInfo.Email, isExpireable: false);

            await _patientService.CreateFamilyRequest(frvm, UserId);

            return RedirectToAction("GoToDashboard", new { UserId });
        }

        public async Task<IActionResult> Agreement(string requestId)
        {
            int decryptedRequestId;
            try
            {
                decryptedRequestId = int.Parse(Decode(requestId));
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                return NotFound();
            }

            AgreementViewModel AgreementInfo = new AgreementViewModel();
            AgreementInfo.RId = (int)decryptedRequestId;
            AgreementInfo = await _patientService.GetAgreementViewModelData(AgreementInfo);
            return View(AgreementInfo);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptAgreement(AgreementViewModel AgreementInfo)
        {
            bool isAgreementAccepted = await _patientService.AcceptAgreement(AgreementInfo);
            if(isAgreementAccepted)
            {
                TempData["SuccessMessage"] = "Agreement Accepted Successfully";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.ErrorMessage = "Some Error Occured";
                return View("Agreement", AgreementInfo);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CancelAgreement(AgreementViewModel CancelAgreementInfo)
        {
            bool isAgreementCancelled = await _patientService.CancelAgreement(CancelAgreementInfo);
            if (isAgreementCancelled)
            {
                TempData["SuccessMessage"] = "Agreement Cancelled Successfully";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.ErrorMessage = "Some Error Occured";
                return View("Agreement", CancelAgreementInfo);

            }
        }
    }
}
