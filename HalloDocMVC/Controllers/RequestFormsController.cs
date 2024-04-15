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

            if (!ModelState.IsValid)
            {
                ViewBag.RegionList = _requestFormService.GetRegionList();
                return View(prvm);
            }

            bool isrequestcreated = await _requestFormService.CreatePatientRequest(prvm);
            if(isrequestcreated)
            {
                TempData["SuccessMessage"] = "Request Created Successfully.";
                return RedirectToAction("SubmitRequest");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Request.";
                return RedirectToAction("PatientRequest");
            }

            
            
        }

        public IActionResult FamilyRequest()
        {
            ViewBag.RegionList = _requestFormService.GetRegionList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamilyRequest(FamilyRequestViewModel frvm)
        {
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

        public IActionResult ConciergeRequest()
        {
            ViewBag.RegionList = _requestFormService.GetRegionList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConciergeRequest(ConciergeRequestViewModel crvm)
        {
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

        public IActionResult BusinessRequest()
        {
            ViewBag.RegionList = _requestFormService.GetRegionList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BusinessRequest(BusinessRequestViewModel brvm)
        {
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
