using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IRequestFormService
    {
        List<SelectListItem> GetRegionList();

        Task<bool> CheckUser(string email);

        Task<bool> SendMail(PatientRequestViewModel PatientInfo);

        Task<bool> ValidateToken(string token);

        Task<bool> CreatePatientRequest(PatientRequestViewModel prvm);

        Task<bool> CreateFamilyRequest(FamilyRequestViewModel prvm);

        Task<bool> CreateConciergeRequest(ConciergeRequestViewModel crvm);

        Task<bool> CreateBusinessRequest(BusinessRequestViewModel brvm);
    }
}
