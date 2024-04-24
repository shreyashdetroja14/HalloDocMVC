using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using Repository.ViewModels;
using System.Diagnostics;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepository;

        public HomeController(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddPatient()
        {
            PatientFormViewModel PatientForm = new PatientFormViewModel();
            PatientForm = _homeRepository.GetPatientFormViewModel(PatientForm);

            return PartialView("_PatientFormPartial", PatientForm);
        }

        [HttpPost]
        public async Task<IActionResult> AddPatient(PatientFormViewModel PatientForm)
        {
            bool isPatientCreated = await _homeRepository.CreatePatient(PatientForm);
            if (isPatientCreated)
            {
                return Json(new {status = true});
            }
            else
            {
                return Json(new {status = false});
            }
        }

        public IActionResult GetPatientList(string? searchPattern, int pageNumber)
        {
            if(pageNumber <= 0)
            {
                pageNumber = 1;
            }

            PaginatedList<PatientRowViewModel> paginatedList = _homeRepository.GetPatientList(searchPattern, pageNumber);

            ViewBag.PagerData = paginatedList.PagerData;
            return PartialView("_PatientsTablePartial", paginatedList.DataRows);
        }

        public IActionResult EditPatient(int patientId)
        {
            PatientFormViewModel PatientForm = new PatientFormViewModel();
            PatientForm.PatientId = patientId;
            PatientForm = _homeRepository.GetPatientFormViewModel(PatientForm);

            return PartialView("_PatientFormPartial", PatientForm);
        }

        [HttpPost]
        public async Task<IActionResult> EditPatient(PatientFormViewModel PatientForm)
        {
            bool isPatientEdited = await _homeRepository.EditPatient(PatientForm);
            //bool isPatientEdited = true;
            if (isPatientEdited)
            {
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false });
            }
        }

        public async Task<IActionResult> DeletePatient(int patientId)
        {
            bool isPatientDeleted = await _homeRepository.DeletePatient(patientId);
            //bool isPatientEdited = true;
            if (isPatientDeleted)
            {
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}