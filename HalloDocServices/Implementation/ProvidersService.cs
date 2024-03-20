using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Implementation
{
    public enum Status
    {
        Active = 1,
        Inactive = 2,
        Pending = 3
    }

    public class ProvidersService : IProvidersService
    {
        private readonly IPhysicianRepository _physicianRepository;
        private readonly INotesAndLogsRepository _notesAndLogsRepository;
        private readonly ICommonRepository _commonRepository;

        public ProvidersService(IPhysicianRepository physicianRepository, INotesAndLogsRepository notesAndLogsRepository, ICommonRepository commonRepository)
        {
            _physicianRepository = physicianRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
            _commonRepository = commonRepository;
        }

        public ProvidersViewModel GetProvidersViewModel(ProvidersViewModel Providers)
        {
            var regions = _commonRepository.GetAllRegions();

            Providers.RegionList.Add(new SelectListItem()
            {
                Text = "All",
                Value = "0",
                Selected = true
            });

            foreach (var region in regions)
            {

                Providers.RegionList.Add(new SelectListItem()
                {
                    Text = region.Name,
                    Value = region.RegionId.ToString()
                });
            }

            return Providers;

        }

        public List<ProviderRowViewModel> GetProvidersList(int regionId)
        {
            var providersFetched = _physicianRepository.GetIQueryablePhysicians();
            providersFetched = providersFetched.Include(x => x.Role);

            if(regionId != 0)
            {
                providersFetched = providersFetched.Where(x => x.RegionId == regionId);
            }

            List<ProviderRowViewModel> Providers = new List<ProviderRowViewModel>();

            foreach (var provider in providersFetched)
            {
                Providers.Add(new ProviderRowViewModel()
                {
                    ProviderId = provider.PhysicianId,
                    ProviderName = provider.FirstName + " " + provider.LastName,
                    IsNotificationStopped = provider.IsNotificationStopped,
                    Role = provider.Role?.Name,
                    //On call status
                    OnCallStatus = "unavailable",
                    //Status
                    Status = provider.Status != null ? ((Status)provider.Status).ToString() : null,
                });
            }

            return Providers;
        }

        public ContactProviderViewModel GetContactProvider(ContactProviderViewModel ContactProvider)
        {
            var provider = _physicianRepository.GetPhysicianByPhysicianId(ContactProvider.ProviderId);
            if (provider != null)
            {
                ContactProvider.ProviderName = provider.FirstName + " " + provider.LastName;
                ContactProvider.ProviderEmail = provider.Email;
            }

            return ContactProvider;
        }
    }
}
