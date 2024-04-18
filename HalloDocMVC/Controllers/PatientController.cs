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

        #region DASHBOARD

        [CustomAuthorize("patient")]
        public async Task<IActionResult> Dashboard()
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            int userId = claimsData.Id;

            List<DashboardRequestViewModel> requestlist = await _patientService.GetRequestList(userId);

            return View(requestlist);
        }

        #endregion

        #region VIEW DOCUMENTS

        [CustomAuthorize("patient")]
        public async Task<IActionResult> ViewDocuments(int requestid)
        {
            ViewDocumentsViewModel vrvm = await _patientService.GetRequestFiles(requestid);

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


        [HttpPost]
        [CustomAuthorize("patient")]
        public IActionResult DownloadMultipleFiles([FromBody] DownloadRequest requestData)
        {

            List<int> selectedValues = requestData.SelectedValues;
            int requestId = requestData.RequestId;

            var zipdata = _patientService.GetFilesAsZip(selectedValues, requestId);

            if (zipdata != null)
            {
                return File(zipdata, "application/zip", "download.zip");
            }

            TempData["ErrorMessage"] = "Unable To Update Profile";

            return RedirectToAction("ViewDocuments", new { requestid = requestData.RequestId });
        }

        #endregion

        #region PROFILE 

        [CustomAuthorize("patient")]
        public async Task<IActionResult> Profile()
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            ProfileViewModel profileDetails = await _patientService.GetProfileDetails(claimsData.Id);
            
            return View(profileDetails);
        }

        [CustomAuthorize("patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel ProfileDetails)
        {
            if (!ModelState.IsValid)
            {
                var regionList = _patientService.GetRegionList();
                ProfileDetails.RegionList = regionList;
                return View("~/Views/Patient/Profile.cshtml", ProfileDetails);
            }

            bool isProfileEdited = await _patientService.EditProfile(ProfileDetails);
            if (isProfileEdited)
            {
                TempData["SuccessMessage"] = "Profile Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable To Update Profile";

            }

            return RedirectToAction("Profile");
        }

        #endregion

        #region PATIENT REQUEST

        [CustomAuthorize("patient")]
        public async Task<IActionResult> PatientRequest()
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            PatientRequestViewModel PatientInfo = new PatientRequestViewModel();
            PatientInfo = await _patientService.GetPatientInfo(claimsData.Id);

            FamilyRequestViewModel frvm = new FamilyRequestViewModel();
            frvm.PatientInfo = PatientInfo;

            ViewBag.RegionList = _patientService.GetRegionList();
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
                ViewBag.RegionList = _patientService.GetRegionList();
                return View(frvm);
            }

            bool isRequestCreated = await _patientService.CreatePatientRequest(frvm);
            if (isRequestCreated)
            {
                TempData["SuccessMessage"] = "Request Created Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable To Create Request";
                return RedirectToAction("PatientRequest");

            }

            return RedirectToAction("Dashboard");

        }

        #endregion

        #region FAMILY REQUEST

        [CustomAuthorize("patient")]
        public IActionResult FamilyRequest()
        {
            ViewBag.RequestType = 3;
            ViewBag.RegionList = _patientService.GetRegionList();

            return View("~/Views/Patient/PatientRequest.cshtml");
        }

        [CustomAuthorize("patient")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyRequest(FamilyRequestViewModel frvm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RequestType = 3;
                ViewBag.RegionList = _patientService.GetRegionList();
                return View("~/Views/Patient/PatientRequest.cshtml", frvm);
            }

            ClaimsData claimsData = _jwtService.GetClaimValues();

            bool isRequestCreated = await _patientService.CreateFamilyRequest(frvm, claimsData.Id);
            if (isRequestCreated)
            {
                TempData["SuccessMessage"] = "Request Created Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable To Create Request";
                return RedirectToAction("FamilyRequest");

            }

            return RedirectToAction("Dashboard");
        }

        #endregion

        #region AGREEMENT

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

        #endregion
    }
}
