﻿using HalloDocServices.ViewModels.AdminViewModels;
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
        ProvidersViewModel GetProvidersViewModel(ProvidersViewModel Providers);

        List<ProviderRowViewModel> GetProvidersList(int regionId);

        ContactProviderViewModel GetContactProvider(ContactProviderViewModel ContactProvider);

        Task<bool> UpdateNotiStatus(List<int> StopNotificationIds);

        EditProviderViewModel GetEditProviderViewModel(EditProviderViewModel EditProvider);

        Task<bool> ResetPassword(EditProviderViewModel AccountInfo);

        Task<bool> EditAccountInfo(EditProviderViewModel AccountInfo);

        Task<bool> EditPhysicianInfo(EditProviderViewModel PhysicianInfo);

        Task<bool> EditBillingInfo(EditProviderViewModel BillingInfo);

        Task<bool> EditProfileInfo(EditProviderViewModel ProfileInfo);

        Task<bool> Onboarding(IFormFile UploadDoc, int docId, int providerId);

        Task<bool> DeleteProvider(int providerId);

        Task<bool> CreateProvider(EditProviderViewModel ProviderInfo);

        int CheckUserName(string username);

        #region SCHEDULING

        SchedulingViewModel GetSchedulingViewModel(SchedulingViewModel SchedulingData);

        #endregion
    }
}
