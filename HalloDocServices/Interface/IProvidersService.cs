using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        Task<bool> ContactProvider(ContactProviderViewModel ContactProvider);

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

        List<PayrateCategoryViewModel> GetPayrateViewModelData(int providerId);

        Task<bool> EditPayrate(PayrateCategoryViewModel payrateDetails);

        #endregion

        #region CREATE PROVIDER 

        Task<bool> CreateProvider(EditProviderViewModel ProviderInfo);

        int CheckUserName(string username);

        #endregion

        #region SCHEDULING

        SchedulingViewModel GetSchedulingViewModel(SchedulingViewModel SchedulingData);

        CalendarViewModel GetCalendarViewModel(int regionId, int? physicianId = null);

        bool CheckAvailableShift(CreateShiftViewModel CreateShiftData);

        Task<bool> CreateShift(CreateShiftViewModel CreateShiftData);

        List<SelectListItem> GetPhysiciansByRegion(int regionId);

        CreateShiftViewModel GetViewShiftViewModel(CreateShiftViewModel ViewShiftData);

        Task<bool> EditShift(CreateShiftViewModel EditShiftData);

        Task<bool> ReturnShift(CreateShiftViewModel ReturnShiftData);

        Task<bool> DeleteShift(CreateShiftViewModel DeleteShiftData);

        #endregion

        #region REQUEST SHIFT 

        RequestedShiftViewModel GetRequestedShiftViewModel(RequestedShiftViewModel RequestedShiftData);

        List<RequestShiftRowViewModel> GetShiftsList(int regionId);

        Task<bool> ApproveShifts(List<int> shiftDetailIds, string modifiedBy);

        Task<bool> DeleteShifts(List<int> shiftDetailIds, string modifiedBy);

        #endregion

        #region MDS ON CALL 

        MDsOnCallViewModel GetMDsOnCallViewModel(MDsOnCallViewModel MDsOnCallData);

        MDsListViewModel GetMDsList(int regionId);

        #endregion

        #region PROVIDER LOCATION

        List<ProviderLocationViewModel> GetProviderLocations();

        #endregion
    }
}
