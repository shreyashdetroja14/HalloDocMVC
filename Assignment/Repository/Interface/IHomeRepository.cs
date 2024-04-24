using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IHomeRepository
    {
        PatientFormViewModel GetPatientFormViewModel(PatientFormViewModel PatientForm);

        Task<bool> CreatePatient(PatientFormViewModel PatientForm);

        PaginatedList<PatientRowViewModel> GetPatientList(string? searchPattern, int pageNumber);

        Task<bool> EditPatient(PatientFormViewModel PatientForm);

        Task<bool> DeletePatient(int patientId);
    }
}
