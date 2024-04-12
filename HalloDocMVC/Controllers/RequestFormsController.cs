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
                var receiver = frvm.PatientInfo.Email;

                var subject = "Create Account from HalloDoc@Admin";
                var message = "Tap on link to create account on HalloDoc: http://localhost:5059/Login/CreateAccount";

                await _requestFormService.SendMail(receiver, subject, message);
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
                var receiver = crvm.PatientInfo.Email;

                var subject = "Create Account from HalloDoc@Admin";
                var message = "Tap on link to create account on HalloDoc: http://localhost:5059/Login/CreateAccount";

                await _requestFormService.SendMail(receiver, subject, message);
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
                var receiver = brvm.PatientInfo.Email;

                var subject = "Create Account from HalloDoc@Admin";
                var message = "Tap on link to create account on HalloDoc: http://localhost:5059/Login/CreateAccount";

                await _requestFormService.SendMail(receiver, subject, message);
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
