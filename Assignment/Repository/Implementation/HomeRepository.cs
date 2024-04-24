using Entities.Data;
using Entities.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementation
{
    public class HomeRepository : IHomeRepository
    {
        private readonly HMSContext _context;
        public HomeRepository(HMSContext context)
        {
            _context = context;
        }

        private List<Doctor> GetAllDoctors()
        {
            return _context.Doctors.ToList();
        }

        public PatientFormViewModel GetPatientFormViewModel(PatientFormViewModel PatientForm)
        {
            PatientForm.IsEditPatient = false;
            List<Doctor> doctors = GetAllDoctors();
            foreach (Doctor doctor in doctors)
            {
                PatientForm.SpecialistList.Add(new SelectListItem
                {
                    Value = doctor.DoctorId.ToString(),
                    Text = doctor.Specialist
                });
            }

            PatientForm.DiseaseDictionary = new Common().Diseases;

            Patient? patient = _context.Patients.FirstOrDefault(x => x.PatientId == PatientForm.PatientId);
            if(patient != null)
            {
                Doctor? doctor = _context.Doctors.FirstOrDefault(x => x.DoctorId == patient.DoctorId);
                var dictionary = new Common().Diseases;

                PatientForm.IsEditPatient = true;
                PatientForm.PatientId = patient.PatientId;
                PatientForm.FirstName = patient.FirstName;
                PatientForm.LastName = patient.LastName;
                PatientForm.Email = patient.Email;
                PatientForm.PhoneNumber = patient.PhoneNumber;
                PatientForm.Age = patient.Age;
                PatientForm.Gender = patient.Gender;
                PatientForm.DoctorId = doctor?.DoctorId;
                PatientForm.DiseaseKey = dictionary.FirstOrDefault(x => x.Value == patient.Disease).Key;
            }

            return PatientForm;
        }

        public async Task<bool> CreatePatient(PatientFormViewModel PatientForm)
        {
            var dictionary = new Common().Diseases;
            Doctor? doctor = _context.Doctors.FirstOrDefault(x => x.DoctorId == PatientForm.DoctorId);
            if (doctor == null)
            {
                return false;
            }

            Patient patient = new Patient();
            patient.FirstName = PatientForm.FirstName;
            patient.LastName = PatientForm.LastName;
            patient.Email = PatientForm.Email;
            patient.Age = PatientForm.Age;
            patient.Gender = PatientForm.Gender;
            patient.PhoneNumber = PatientForm.PhoneNumber;
            patient.DoctorId = PatientForm.DoctorId;
            patient.Disease = dictionary[PatientForm.DiseaseKey ?? 0];
            patient.Specialist = doctor?.Specialist;
            patient.IsDeleted = false;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return true;
        }

        public PaginatedList<PatientRowViewModel> GetPatientList(string? searchPattern, int pageNumber)
        {
            var patients = _context.Patients.AsQueryable().Where(x => x.IsDeleted == false);

            if(searchPattern != null)
            {
                patients = patients.Where(x => EF.Functions.ILike(x.FirstName, "%" + searchPattern + "%"));
            }

            patients = patients.OrderBy(x => x.PatientId);

            int patientCount = patients.Count();
            int pageSize = 5;

            PagerViewModel PagerData = new PagerViewModel(patientCount, pageNumber, pageSize);

            //pagination query for db 

            int rowsToSkip = (pageNumber - 1) * pageSize;

            patients = patients.Skip(rowsToSkip).Take(pageSize);

            List<PatientRowViewModel> patientsList = patients.Select(x => new PatientRowViewModel
            {
                PatientId = x.PatientId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Age = x.Age,
                Gender = ((Gender)(x.Gender ?? 0)).ToString(),
                PhoneNumber = x.PhoneNumber,
                Disease = x.Disease,
                Doctor = x.Doctor != null ? x.Doctor.Specialist : null,

            }).ToList();

            PaginatedList<PatientRowViewModel> paginatedList = new PaginatedList<PatientRowViewModel>();
            paginatedList.PagerData = PagerData;
            paginatedList.DataRows = patientsList;

            return paginatedList;
        }

        public async Task<bool> EditPatient(PatientFormViewModel PatientForm)
        {
            var dictionary = new Common().Diseases;
            Patient? patient = _context.Patients.FirstOrDefault(x => x.PatientId == PatientForm.PatientId);
            if (patient == null)
            {
                return false;
            }

            Doctor? doctor = _context.Doctors.FirstOrDefault(x => x.DoctorId == PatientForm.DoctorId);
            if (doctor == null)
            {
                return false;
            }

            patient.FirstName = PatientForm.FirstName;
            patient.LastName = PatientForm.LastName;
            patient.Email = PatientForm.Email;
            patient.Age = PatientForm.Age;
            patient.Gender = PatientForm.Gender;
            patient.PhoneNumber = PatientForm.PhoneNumber;
            patient.DoctorId = PatientForm.DoctorId;
            patient.Disease = dictionary[PatientForm.DiseaseKey ?? 0];
            patient.Specialist = doctor?.Specialist;
            patient.IsDeleted = false;

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletePatient(int patientId)
        {
            Patient? patient = _context.Patients.FirstOrDefault(x => x.PatientId == patientId);
            if (patient == null)
            {
                return false;
            }

            patient.IsDeleted = true;

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
