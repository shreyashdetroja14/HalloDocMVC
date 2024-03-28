using HalloDocEntities.Models;
using HalloDocRepository.Implementation;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        private readonly IUserRepository _userRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly INotesAndLogsRepository _notesAndLogsRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IRoleRepository _roleRepository;

        public ProvidersService(IUserRepository userRepository, IPhysicianRepository physicianRepository, INotesAndLogsRepository notesAndLogsRepository, ICommonRepository commonRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _physicianRepository = physicianRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
            _commonRepository = commonRepository;
            _roleRepository = roleRepository;
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
            var providersFetched = _physicianRepository.GetIQueryablePhysicians().Where(x => x.IsDeleted != true);
            providersFetched = providersFetched.Include(x => x.Role);

            if (regionId != 0)
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

        public async Task<bool> UpdateNotiStatus(List<int> StopNotificationIds)
        {
            var providers = _physicianRepository.GetAllPhysicians();

            foreach (var provider in providers)
            {
                provider.IsNotificationStopped = true;
            }

            providers = providers.Where(x => !StopNotificationIds.Contains(x.PhysicianId)).ToList();

            foreach (var provider in providers)
            {
                provider.IsNotificationStopped = false;
            }

            await _physicianRepository.Update(providers);

            return true;
        }

        public EditProviderViewModel GetEditProviderViewModel(EditProviderViewModel EditProvider)
        {
            var providers = _physicianRepository.GetIQueryablePhysicians(EditProvider.ProviderId);
            var provider = providers.Include(x => x.AspNetUser).Include(x => x.Role).Include(x => x.Region).Include(x => x.PhysicianRegions).FirstOrDefault();

            EditProvider.StatusList = Enum.GetValues(typeof(Status))
                                    .Cast<Status>()
                                    .Select(status => new SelectListItem
                                    {
                                        Value = ((int)status).ToString(),
                                        Text = status.ToString(),
                                        Selected = (status == (provider?.Status != null ? ((Status)provider.Status) : null))
                                    }).ToList();

            var rolesList = _roleRepository.GetAllRoles();

            EditProvider.RoleList.Add(new SelectListItem()
            {
                Value = "0",
                Text = "Set a role",
                Selected = true
            });

            foreach (var role in rolesList)
            {
                EditProvider.RoleList.Add(new SelectListItem()
                {
                    Value = role.RoleId.ToString(),
                    Text = role.Name,
                    Selected = (role.RoleId == provider?.RoleId)
                });
            }

            var regions = _commonRepository.GetAllRegions();

            EditProvider.StateList.Add(new SelectListItem()
            {
                Text = "Set a region",
                Value = "0",
                Selected = true
            });

            foreach (var region in regions)
            {
                EditProvider.StateList.Add(new SelectListItem()
                {
                    Text = region.Name,
                    Value = region.RegionId.ToString(),
                    Selected = (region.RegionId == provider?.RegionId)
                });
            }

            if (provider != null)
            {

                EditProvider.Username = provider.AspNetUser?.UserName ?? "";
                EditProvider.Status = provider.Status;

                EditProvider.RoleId = provider.RoleId;

                EditProvider.FirstName = provider.FirstName;
                EditProvider.LastName = provider.LastName;
                EditProvider.Email = provider.Email;
                EditProvider.MedicalLicense = provider.MedicalLicense;
                EditProvider.NPINumber = provider.NpiNumber;
                EditProvider.PhoneNumber = provider.Mobile;
                EditProvider.SyncEmail = provider.SyncEmailAddress;

                foreach (var providerRegion in provider.PhysicianRegions)
                {
                    EditProvider.ProviderRegions.Add(providerRegion.RegionId);
                }

                EditProvider.Address1 = provider.Address1;
                EditProvider.Address2 = provider.Address2;
                EditProvider.City = provider.City;
                EditProvider.RegionId = provider.RegionId;

                EditProvider.ZipCode = provider.ZipCode;
                EditProvider.SecondPhoneNumber = provider.AltPhone;
                EditProvider.BusinessName = provider.BusinessName;
                EditProvider.BusinessWebsite = provider.BusinessWebsite;
                EditProvider.AdminNotes = provider.AdminNotes;

                EditProvider.SignaturePath = provider.Signature;

                EditProvider.IsContractorDoc = provider.IsAgreementDoc;
                EditProvider.IsBackgroundDoc = provider.IsBackgroundDoc;
                EditProvider.IsHippaDoc = provider.IsTrainingDoc;
                EditProvider.IsNonDisclosureDoc = provider.IsNonDisclosureDoc;
                EditProvider.IsLicenseDoc = provider.IsLicenseDoc;

            }

            return EditProvider;

        }

        public async Task<bool> EditAccountInfo(EditProviderViewModel AccountInfo)
        {
            var provider = _physicianRepository.GetIQueryablePhysicians(AccountInfo.ProviderId).Include(x => x.AspNetUser).FirstOrDefault();
            var aspnetUser = provider?.AspNetUser;

            if (provider == null)
            {
                return false;
            }

            provider.Status = (short?)AccountInfo.Status;
            provider.RoleId = AccountInfo.RoleId;

            await _physicianRepository.Update(provider);

            if (aspnetUser == null)
            {
                return false;
            }

            aspnetUser.UserName = AccountInfo.Username;
            if (AccountInfo.Password != null)
            {
                aspnetUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(AccountInfo.Password);
            }

            await _userRepository.UpdateAspNetUser(aspnetUser);

            return true;
        }

        public async Task<bool> EditPhysicianInfo(EditProviderViewModel PhysicianInfo)
        {
            var provider = _physicianRepository.GetIQueryablePhysicians(PhysicianInfo.ProviderId).Include(x => x.PhysicianRegions).FirstOrDefault();
            var providerRegions = provider?.PhysicianRegions.ToList();

            if (provider == null)
            {
                return false;
            }

            provider.FirstName = PhysicianInfo.FirstName;
            provider.LastName = PhysicianInfo.LastName;
            provider.Email = PhysicianInfo.Email;
            provider.Mobile = PhysicianInfo.PhoneNumber;
            provider.MedicalLicense = PhysicianInfo.MedicalLicense;
            provider.NpiNumber = PhysicianInfo.NPINumber;
            provider.SyncEmailAddress = PhysicianInfo.SyncEmail;

            await _physicianRepository.Update(provider);

            var physicianRegions = _physicianRepository.GetRegionsByPhysicianId(provider.PhysicianId);

            if (physicianRegions == null)
            {
                return false;
            }

            var regionsToRemove = physicianRegions?.Where(x => !PhysicianInfo.ProviderRegions.Contains(x.RegionId)).ToList();
            var regionsToAdd = PhysicianInfo.ProviderRegions.Except(physicianRegions.Select(x => x.RegionId)).ToList();

            if (regionsToAdd.Count > 0)
            {
                regionsToAdd = await _physicianRepository.AddPhysicianRegionsAsync(regionsToAdd, provider.PhysicianId);
            }

            if (regionsToRemove != null && regionsToRemove.Count > 0)
            {
                regionsToRemove = await _physicianRepository.RemovePhysicianRegionsAsync(regionsToRemove);
            }

            return true;
        }

        public async Task<bool> EditBillingInfo(EditProviderViewModel BillingInfo)
        {
            var provider = _physicianRepository.GetPhysicianByPhysicianId(BillingInfo.ProviderId);
            if (provider == null) { return false; }

            provider.Address1 = BillingInfo.Address1;
            provider.Address2 = BillingInfo.Address2;
            provider.City = BillingInfo.City;
            provider.RegionId = BillingInfo.RegionId;
            provider.ZipCode = BillingInfo.ZipCode;
            provider.AltPhone = BillingInfo.SecondPhoneNumber;

            await _physicianRepository.Update(provider);

            return true;
        }

        public string UploadFilesToServer(IFormFile? UploadFile, int providerId)
        {
            if (UploadFile == null) { return string.Empty; }

            string FilePath = "wwwroot\\Upload\\Physician\\" + providerId;
            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";

            string fileNameWithPath = Path.Combine(path, newfilename);
            string file = FilePath.Replace("wwwroot\\Upload\\Physician\\", "/Upload/Physician/") + "/" + newfilename;

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                UploadFile.CopyTo(stream);
            }

            return file;
        }

        public async Task<bool> EditProfileInfo(EditProviderViewModel ProfileInfo)
        {
            var provider = _physicianRepository.GetPhysicianByPhysicianId(ProfileInfo.ProviderId);
            if (provider == null) { return false; }

            provider.BusinessName = ProfileInfo.BusinessName;
            provider.BusinessWebsite = ProfileInfo.BusinessWebsite;
            provider.AdminNotes = ProfileInfo.AdminNotes;

            string photoFileName = UploadFilesToServer(ProfileInfo.Photo, ProfileInfo.ProviderId);
            if (photoFileName != string.Empty)
            {
                provider.Photo = photoFileName;
            }

            string signatureFileName = UploadFilesToServer(ProfileInfo.Signature, ProfileInfo.ProviderId);
            if (signatureFileName != string.Empty)
            {
                provider.Signature = signatureFileName;
            }

            await _physicianRepository.Update(provider);

            return true;
        }

        public async Task<bool> Onboarding(IFormFile UploadDoc, int docId, int providerId)
        {
            var provider = _physicianRepository.GetPhysicianByPhysicianId(providerId);
            if (provider == null) { return false; }

            string FilePath = "wwwroot\\Upload\\Physician\\" + providerId;
            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string newfilename = $"{docId}.{Path.GetExtension(UploadDoc.FileName).Trim('.')}";
            //string newfilename = $"{docId}.pdf";

            string fileNameWithPath = Path.Combine(path, newfilename);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                UploadDoc.CopyTo(stream);
            }

            switch(docId)
            {
                case 1:
                    provider.IsAgreementDoc = true;
                    break;

                case 2:
                    provider.IsBackgroundDoc = true;
                    break;

                case 3:
                    provider.IsTrainingDoc = true;
                    break;

                case 4:
                    provider.IsNonDisclosureDoc = true;
                    break;

                case 5:
                    provider.IsLicenseDoc = true;
                    break;
            }

            await _physicianRepository.Update(provider);

            return true;
        }

        public async Task<bool> DeleteProvider(int providerId)
        {
            var provider = _physicianRepository.GetPhysicianByPhysicianId(providerId);
            if (provider == null) { return false; }

            provider.IsDeleted = true;

            await _physicianRepository.Update(provider);

            return true;
        }
    }
}
