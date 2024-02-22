using HalloDocServices.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IRequestFormService
    {
        Task<bool> CheckUser(string email);

        Task<bool> SendMail(string receiver, string subject, string message);

        Task<bool> ValidateToken(string token);

        Task<bool> CreatePatientRequest(PatientRequestViewModel prvm);

        Task<bool> CreateFamilyRequest(FamilyRequestViewModel prvm);

        Task<bool> CreateConciergeRequest(ConciergeRequestViewModel crvm);

        Task<bool> CreateBusinessRequest(BusinessRequestViewModel brvm);
    }
}
