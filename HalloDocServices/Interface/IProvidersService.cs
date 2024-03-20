﻿using HalloDocServices.ViewModels.AdminViewModels;
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
    }
}
