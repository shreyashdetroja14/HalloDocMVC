using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IProvidersService
    {
        #region PROVIDERS LIST

        ProvidersViewModel GetProvidersViewModel(ProvidersViewModel Providers);

        List<ProviderRowViewModel> GetProvidersList(int regionId);

        ContactProviderViewModel GetContactProvider(ContactProviderViewModel ContactProvider);

        Task<bool> UpdateNotiStatus(List<int> StopNotificationIds);

        #endregion

        #region EDIT PROVIDER

        EditProviderViewModel GetEditProviderViewModel(EditProviderViewModel EditProvider);

        Task<bool> ResetPassword(EditProviderViewModel AccountInfo);

        Task<bool> EditAccountInfo(EditProviderViewModel AccountInfo);

        Task<bool> EditPhysicianInfo(EditProviderViewModel PhysicianInfo);

        Task<bool> EditBillingInfo(EditProviderViewModel BillingInfo);

        Task<bool> EditProfileInfo(EditProviderViewModel ProfileInfo);

        Task<bool> Onboarding(IFormFile UploadDoc, int docId, int providerId);

        Task<bool> DeleteProvider(int providerId);

        #endregion

        #region CREATE PROVIDER 

        Task<bool> CreateProvider(EditProviderViewModel ProviderInfo);

        int CheckUserName(string username);

        #endregion

        #region SCHEDULING

        SchedulingViewModel GetSchedulingViewModel(SchedulingViewModel SchedulingData);

        Task<bool> CreateShift(CreateShiftViewModel CreateShiftData);

        #endregion

        #region REQUEST SHIFT 

        RequestedShiftViewModel GetRequestedShiftViewModel(RequestedShiftViewModel RequestedShiftData);

        List<RequestShiftRowViewModel> GetShiftsList(int regionId);

        Task<bool> ApproveShifts(List<int> shiftDetailIds, string modifiedBy);

        Task<bool> DeleteShifts(List<int> shiftDetailIds, string modifiedBy);

        CalendarViewModel GetCalendarViewModel(int regionId);

        #endregion
    }
}
