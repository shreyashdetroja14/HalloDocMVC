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
        private readonly IRequestFormService _requestFormService;
        private readonly IJwtService _jwtService;

        public RequestFormsController(IRequestFormService requestFormService, IJwtService jwtService)
        {
            _requestFormService = requestFormService;
            _jwtService = jwtService;
        }

        public IActionResult SubmitRequest()
        {
            return View();
        }

        #region PATIENT REQUEST

        public IActionResult PatientRequest()
        {
            ViewBag.RegionList = _requestFormService.GetRegionList();
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
                    ViewBag.RegionList = _requestFormService.GetRegionList();
                    return View(prvm);
                }
            }

            bool isUserBlocked = _requestFormService.CheckBlockRequest(prvm.Email);
            if (isUserBlocked)
            {
                TempData["ErrorMessage"] = "Sorry, This patient email has been blocked.";
                return RedirectToAction("PatientRequest");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.RegionList = _requestFormService.GetRegionList();
                return View(prvm);
            }

            bool isrequestcreated = await _requestFormService.CreatePatientRequest(prvm);
            if(isrequestcreated)
            {
                TempData["SuccessMessage"] = "Request Created Successfully.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Request.";
                return RedirectToAction("PatientRequest");
            }

            
            
        }

        #endregion

        #region FAMILY REQUEST

        public IActionResult FamilyRequest()
        {
            ViewBag.RegionList = _requestFormService.GetRegionList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyRequest(FamilyRequestViewModel frvm)
        {
            bool isUserBlocked = _requestFormService.CheckBlockRequest(frvm.PatientInfo.Email);
            if (isUserBlocked)
            {
                TempData["ErrorMessage"] = "Sorry, This patient email has been blocked.";
                return RedirectToAction("FamilyRequest");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.RegionList = _requestFormService.GetRegionList();
                return View(frvm);
            }

            bool isUserExists = await _requestFormService.CheckUser(frvm.PatientInfo.Email);
            if (!isUserExists)
            {
                frvm.PatientInfo.EmailToken = _jwtService.GenerateEmailToken(frvm.PatientInfo.Email, isExpireable: false);
                await _requestFormService.SendMail(frvm.PatientInfo);
            }

            bool isrequestcreated = await _requestFormService.CreateFamilyRequest(frvm);
            if(isrequestcreated)
            {
                TempData["SuccessMessage"] = "Request Created Successfully.";
                return RedirectToAction("SubmitRequest");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Request.";
                return RedirectToAction("FamilyRequest");
            }
        }

        #endregion

        #region CONCIERGE REQUEST

        public IActionResult ConciergeRequest()
        {
            ViewBag.RegionList = _requestFormService.GetRegionList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConciergeRequest(ConciergeRequestViewModel crvm)
        {
            bool isUserBlocked = _requestFormService.CheckBlockRequest(crvm.PatientInfo.Email);
            if (isUserBlocked)
            {
                TempData["ErrorMessage"] = "Sorry, This patient email has been blocked.";
                return RedirectToAction("ConciergeRequest");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.RegionList = _requestFormService.GetRegionList();
                return View(crvm);
            }

            bool isUserExists = await _requestFormService.CheckUser(crvm.PatientInfo.Email);
            if (!isUserExists)
            {
                crvm.PatientInfo.EmailToken = _jwtService.GenerateEmailToken(crvm.PatientInfo.Email, isExpireable: false);
                await _requestFormService.SendMail(crvm.PatientInfo);
            }

            bool isrequestcreated = await _requestFormService.CreateConciergeRequest(crvm);
            if (isrequestcreated)
            {
                TempData["SuccessMessage"] = "Request Created Successfully.";
                return RedirectToAction("SubmitRequest");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Request.";
                return RedirectToAction("ConciergeRequest");
            }

        }

        #endregion

        #region BUSINESS REQUEST

        public IActionResult BusinessRequest()
        {
            ViewBag.RegionList = _requestFormService.GetRegionList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BusinessRequest(BusinessRequestViewModel brvm)
        {
            bool isUserBlocked = _requestFormService.CheckBlockRequest(brvm.PatientInfo.Email);
            if (isUserBlocked)
            {
                TempData["ErrorMessage"] = "Sorry, This patient email has been blocked.";
                return RedirectToAction("BusinessRequest");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.RegionList = _requestFormService.GetRegionList();
                return View("~/Views/Home/Index.cshtml");
            }
            bool isUserExists = await _requestFormService.CheckUser(brvm.PatientInfo.Email);
            if (!isUserExists)
            {
                brvm.PatientInfo.EmailToken = _jwtService.GenerateEmailToken(brvm.PatientInfo.Email, isExpireable: false);
                await _requestFormService.SendMail(brvm.PatientInfo);
            }

            bool isrequestcreated = await _requestFormService.CreateBusinessRequest(brvm);
            if (isrequestcreated)
            {
                TempData["SuccessMessage"] = "Request Created Successfully.";
                return RedirectToAction("SubmitRequest");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Request.";
                return RedirectToAction("BusinessRequest");
            }

        }

        #endregion

        #region CHECK USER ACCOUNT EXISTS OR NOT

        public async Task<IActionResult> CheckUserAccount(string? email)
        {
            email = email?.ToLower().Trim();
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

        #endregion
    }
}
