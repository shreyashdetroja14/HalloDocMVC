using HalloDocEntities.Models;
using HalloDocRepository.Implementation;
using HalloDocRepository.Interface;
using HalloDocServices.Constants;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Implementation
{


    public class ProvidersService : IProvidersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly INotesAndLogsRepository _notesAndLogsRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IShiftRepository _shiftRepository;

        public ProvidersService(IUserRepository userRepository, IPhysicianRepository physicianRepository, INotesAndLogsRepository notesAndLogsRepository, ICommonRepository commonRepository, IRoleRepository roleRepository, IShiftRepository shiftRepository)
        {
            _userRepository = userRepository;
            _physicianRepository = physicianRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
            _commonRepository = commonRepository;
            _roleRepository = roleRepository;
            _shiftRepository = shiftRepository;
        }

        #region PROVIDERS LIST

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

        #endregion

        #region EDIT PROVIDER

        public EditProviderViewModel GetEditProviderViewModel(EditProviderViewModel EditProvider)
        {
            var providers = _physicianRepository.GetIQueryablePhysicians(EditProvider.ProviderId);
            var provider = providers.Include(x => x.AspNetUser).Include(x => x.Role).Include(x => x.Region).Include(x => x.PhysicianRegions).FirstOrDefault();

            /*EditProvider.StatusList = Enum.GetValues(typeof(Status))
                                    .Cast<Status>()
                                    .Select(status => new SelectListItem
                                    {
                                        Value = ((int)status).ToString(),
                                        Text = status.ToString(),
                                        Selected = (status == (provider?.Status != null ? ((Status)provider.Status) : null))
                                    }).ToList();*/

            var rolesList = _roleRepository.GetAllRoles();

            EditProvider.RoleList.Add(new SelectListItem()
            {
                Value = "",
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
                EditProvider.PhotoPath = provider.Photo;

                EditProvider.IsContractorDoc = provider.IsAgreementDoc ?? false;
                EditProvider.IsBackgroundDoc = provider.IsBackgroundDoc ?? false;
                EditProvider.IsHippaDoc = provider.IsTrainingDoc ?? false;
                EditProvider.IsNonDisclosureDoc = provider.IsNonDisclosureDoc ?? false;
                EditProvider.IsLicenseDoc = provider.IsLicenseDoc ?? false;

            }

            return EditProvider;

        }

        public async Task<bool> ResetPassword(EditProviderViewModel AccountInfo)
        {
            var provider = _physicianRepository.GetIQueryablePhysicians(AccountInfo.ProviderId).Include(x => x.AspNetUser).FirstOrDefault();
            var aspnetUser = provider?.AspNetUser;

            if (aspnetUser == null)
            {
                return false;
            }

            aspnetUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(AccountInfo.Password);

            await _userRepository.UpdateAspNetUser(aspnetUser);

            return true;

        }

        public async Task<bool> EditAccountInfo(EditProviderViewModel AccountInfo)
        {
            var provider = _physicianRepository.GetIQueryablePhysicians(AccountInfo.ProviderId).Include(x => x.AspNetUser).FirstOrDefault();
            var aspnetUser = provider?.AspNetUser;

            if (provider == null || aspnetUser == null)
            {
                return false;
            }

            provider.Status = (short?)AccountInfo.Status;
            provider.RoleId = AccountInfo.RoleId;

            await _physicianRepository.Update(provider);

            aspnetUser.UserName = AccountInfo.Username;

            await _userRepository.UpdateAspNetUser(aspnetUser);

            return true;
        }

        public async Task<bool> EditPhysicianInfo(EditProviderViewModel PhysicianInfo)
        {
            var provider = _physicianRepository.GetIQueryablePhysicians(PhysicianInfo.ProviderId).Include(x => x.PhysicianRegions).Include(x => x.AspNetUser).FirstOrDefault();
            var providerRegions = provider?.PhysicianRegions.ToList();
            var aspnetuser = provider?.AspNetUser;

            if (provider == null || aspnetuser == null)
            {
                return false;
            }

            aspnetuser.UserName = PhysicianInfo.Username;
            aspnetuser.Email = PhysicianInfo.Email;
            aspnetuser.PhoneNumber = PhysicianInfo.PhoneNumber;

            await _userRepository.UpdateAspNetUser(aspnetuser);

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

        public string UploadFilesToServer(IFormFile? UploadFile, int docId, int providerId)
        {
            if (UploadFile == null) { return string.Empty; }

            string FilePath = "wwwroot\\Upload\\Physician\\" + providerId;
            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string newfilename = "";
            if (docId > 5)
            {
                newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";
            }
            else
            {
                newfilename = $"{docId}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";
            }

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

            string photoFileName = UploadFilesToServer(ProfileInfo.Photo, 6, ProfileInfo.ProviderId);
            if (photoFileName != string.Empty)
            {
                provider.Photo = photoFileName;
            }

            string signatureFileName = UploadFilesToServer(ProfileInfo.Signature, 7, ProfileInfo.ProviderId);
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

            UploadFilesToServer(UploadDoc, docId, providerId);

            switch (docId)
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

        #endregion

        #region CREATE PROVIDER

        public async Task<bool> CreateProvider(EditProviderViewModel ProviderInfo)
        {
            var aspnetuserNew = new AspNetUser();

            aspnetuserNew.Id = Guid.NewGuid().ToString();

            aspnetuserNew.UserName = ProviderInfo.Username;
            aspnetuserNew.PasswordHash = ProviderInfo.Password != null ? BCrypt.Net.BCrypt.HashPassword(ProviderInfo.Password) : null;
            aspnetuserNew.Email = ProviderInfo.Email;
            aspnetuserNew.PhoneNumber = ProviderInfo.PhoneNumber;
            aspnetuserNew.CreatedDate = DateTime.Now;

            aspnetuserNew = await _userRepository.CreateAspNetUser(aspnetuserNew);

            var aspnetuserRole = new AspNetUserRole();
            aspnetuserRole.AspNetUserId = aspnetuserNew.Id;
            aspnetuserRole.RoleId = "527aa89f-ae69-411b-9795-69675356349a";

            await _userRepository.CreateAspNetUserRole(aspnetuserRole);


            var provider = new Physician();

            provider.AspNetUserId = aspnetuserNew.Id;
            provider.FirstName = ProviderInfo.FirstName;
            provider.LastName = ProviderInfo.LastName;
            provider.Email = ProviderInfo.Email;
            provider.Mobile = ProviderInfo.PhoneNumber;
            provider.MedicalLicense = ProviderInfo.MedicalLicense;
            provider.AdminNotes = ProviderInfo.AdminNotes;
            provider.IsAgreementDoc = ProviderInfo.IsContractorDoc;
            provider.IsBackgroundDoc = ProviderInfo.IsBackgroundDoc;
            provider.IsTrainingDoc = ProviderInfo.IsContractorDoc;
            provider.IsNonDisclosureDoc = ProviderInfo.IsNonDisclosureDoc;
            provider.IsLicenseDoc = ProviderInfo.IsLicenseDoc;
            provider.Address1 = ProviderInfo.Address1;
            provider.Address2 = ProviderInfo.Address2;
            provider.City = ProviderInfo.City;
            provider.RegionId = ProviderInfo.RegionId;
            provider.ZipCode = ProviderInfo.ZipCode;
            provider.AltPhone = ProviderInfo.SecondPhoneNumber;
            provider.CreatedBy = ProviderInfo.CreatedBy;
            provider.CreatedDate = DateTime.Now;
            provider.Status = (short)(ProviderInfo.Status ?? 0);
            provider.BusinessName = ProviderInfo.BusinessName;
            provider.BusinessWebsite = ProviderInfo.BusinessWebsite;
            provider.IsDeleted = false;
            provider.RoleId = ProviderInfo.RoleId;
            provider.NpiNumber = ProviderInfo.NPINumber;
            provider.SyncEmailAddress = ProviderInfo.SyncEmail;
            provider.IsNotificationStopped = false;

            await _physicianRepository.CreateAsync(provider);

            UploadFilesToServer(ProviderInfo.ContractorDoc, 1, provider.PhysicianId);
            UploadFilesToServer(ProviderInfo.BackgroundDoc, 2, provider.PhysicianId);
            UploadFilesToServer(ProviderInfo.HippaDoc, 3, provider.PhysicianId);
            UploadFilesToServer(ProviderInfo.NonDisclosureDoc, 4, provider.PhysicianId);
            UploadFilesToServer(ProviderInfo.LicenseDoc, 5, provider.PhysicianId);

            provider.Photo = UploadFilesToServer(ProviderInfo.Photo, 6, provider.PhysicianId);

            await _physicianRepository.Update(provider);

            await _physicianRepository.AddPhysicianRegionsAsync(ProviderInfo.ProviderRegions, provider.PhysicianId);

            return true;
        }

        public int CheckUserName(string username)
        {
            int count = _userRepository.GetMatchingUserNameCount(username);
            return count;
        }

        #endregion

        #region SCHEDULING 

        public SchedulingViewModel GetSchedulingViewModel(SchedulingViewModel SchedulingData)
        {
            var regions = _commonRepository.GetAllRegions();

            foreach (var region in regions)
            {
                SchedulingData.RegionList.Add(new SelectListItem()
                {
                    Text = region.Name,
                    Value = region.RegionId.ToString(),
                });

                SchedulingData.CreateShiftData.RegionList.Add(new SelectListItem()
                {
                    Text = region.Name,
                    Value = region.RegionId.ToString(),
                });
            }

            var physicianList = _physicianRepository.GetAllPhysicians();

            foreach (var physian in physicianList)
            {
                SchedulingData.CreateShiftData.PhysicianList.Add(new SelectListItem()
                {
                    Text = physian.FirstName + " " + physian.LastName,
                    Value = physian.PhysicianId.ToString(),
                });
            }

            return SchedulingData;
        }

        public async Task<bool> CreateShift(CreateShiftViewModel CreateShiftData)
        {
            var shift = new Shift();

            shift.PhysicianId = CreateShiftData.PhysicianId;

            shift.StartDate = DateTime.ParseExact(CreateShiftData.ShiftDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var initialShiftDate = DateOnly.FromDateTime(shift.StartDate);

            shift.IsRepeat = CreateShiftData.IsRepeat;

            for (int i = 0; i < 7; i++)
            {
                if (CreateShiftData.RepeatDays.Contains(i + 1))
                {
                    shift.WeekDays += "1";
                }
                else
                {
                    shift.WeekDays += "0";
                }
            }

            shift.RepeatUpto = CreateShiftData.RepeatUpto;
            shift.CreatedBy = CreateShiftData.CreatedBy;
            shift.CreatedDate = DateTime.Now;

            shift = await _shiftRepository.CreateShift(shift);

            List<ShiftDetail> shiftDetails = new List<ShiftDetail>();

            shiftDetails.Add(new ShiftDetail
            {
                ShiftId = shift.ShiftId,
                ShiftDate = shift.StartDate,
                RegionId = CreateShiftData.RegionId,
                StartTime = CreateShiftData.StartTime,
                EndTime = CreateShiftData.EndTime,
                //status
                IsDeleted = false,
            });

            if (CreateShiftData.IsRepeat)
            {
                int initialDay = (int)(initialShiftDate.DayOfWeek + 1);
                for (int i = 0; i < CreateShiftData.RepeatUpto; i++)
                {
                    foreach (var day in CreateShiftData.RepeatDays)
                    {
                        int offset = ((7 - (initialDay - day)) % 7) + (7 * i);
                        if (offset % 7 == 0)
                        {
                            offset += 7;
                        }
                        var newDate = initialShiftDate.AddDays(offset);
                        shiftDetails.Add(new ShiftDetail
                        {
                            ShiftId = shift.ShiftId,
                            ShiftDate = new DateTime(newDate.Year, newDate.Month, newDate.Day),
                            RegionId = CreateShiftData.RegionId,
                            StartTime = CreateShiftData.StartTime,
                            EndTime = CreateShiftData.EndTime,
                            IsDeleted = false,
                        });
                    }
                }
            }

            shiftDetails = await _shiftRepository.CreateShiftDetails(shiftDetails);

            List<ShiftDetailRegion> shiftDetailRegions = new List<ShiftDetailRegion>();
            foreach(var shiftDetail in shiftDetails)
            {
                shiftDetailRegions.Add(new ShiftDetailRegion
                {
                    ShiftDetailId = shiftDetail.ShiftDetailId,
                    RegionId = CreateShiftData.RegionId,
                    IsDeleted = false,
                });
            }

            shiftDetailRegions = await _shiftRepository.CreateShiftDetailRegions(shiftDetailRegions);

            return true;
        }

        #endregion
    }
}
