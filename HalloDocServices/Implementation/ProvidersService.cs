using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
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
        private readonly IMailService _mailService;
        private readonly IEmailSMSLogRepository _emailSMSLogRepository;

        public ProvidersService(IUserRepository userRepository, IPhysicianRepository physicianRepository, INotesAndLogsRepository notesAndLogsRepository, ICommonRepository commonRepository, IRoleRepository roleRepository, IShiftRepository shiftRepository, IMailService mailService, IEmailSMSLogRepository emailSMSLogRepository)
        {
            _userRepository = userRepository;
            _physicianRepository = physicianRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
            _commonRepository = commonRepository;
            _roleRepository = roleRepository;
            _shiftRepository = shiftRepository;
            _mailService = mailService;
            _emailSMSLogRepository = emailSMSLogRepository;
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

        public async Task<bool> ContactProvider(ContactProviderViewModel ContactProvider)
        {
            var provider = _physicianRepository.GetPhysicianByPhysicianId(ContactProvider.ProviderId);
            if(provider == null)
            {
                return false;
            }

            if (ContactProvider.CommunicationType == "email")
            {
                List<string> receivers = new List<string>
                {
                    ContactProvider.ProviderEmail ?? ""
                };
                string subject = ContactProvider.Subject ?? "";
                string body = ContactProvider.Message ?? "";

                bool isMailSent = await _mailService.SendMail(receivers, subject, body, false);

                if (isMailSent)
                {
                    EmailLog emailLog = new EmailLog();
                    emailLog.EmailTemplate = body;
                    emailLog.SubjectName = subject;
                    emailLog.EmailId = ContactProvider.ProviderEmail ?? "";
                    emailLog.Action = (int)ActionEnum.AdminMessage;
                    emailLog.RoleId = (int)AccountType.Physician;
                    emailLog.AdminId = ContactProvider.AdminId;
                    emailLog.PhysicianId = ContactProvider.ProviderId;
                    emailLog.CreatedDate = DateTime.Now;
                    emailLog.SentDate = DateTime.Now;
                    emailLog.IsEmailSent = isMailSent;
                    emailLog.SentTries = 1;
                    emailLog.RecipientName = provider.FirstName + " " + provider.LastName;

                    await _emailSMSLogRepository.CreateEmailLog(emailLog);

                    return true;
                }
            }
            else
            {
                if(provider.Mobile == null)
                {
                    return false;
                }

                string subject = ContactProvider.Subject ?? "";
                string body = ContactProvider.Message ?? "";

                SmsLog smsLog = new SmsLog();
                smsLog.SmsTemplate = body;
                smsLog.MobileNumber = provider.Mobile ?? "";
                smsLog.Action = (int)ActionEnum.AdminMessage;
                smsLog.RoleId = (int)AccountType.Physician;
                smsLog.AdminId = ContactProvider.AdminId;
                smsLog.PhysicianId = provider.PhysicianId;
                smsLog.CreatedDate = DateTime.Now;
                smsLog.SentDate = DateTime.Now;
                smsLog.IsSmsSent = false;
                smsLog.SentTries = 1;
                smsLog.RecipientName = provider.FirstName + " " + provider.LastName;

                await _emailSMSLogRepository.CreateSmsLog(smsLog);

                return true;
            }

            return false;
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

            var rolesList = _roleRepository.GetAllRoles().Where(x => x.IsDeleted != true && x.AccountType == (short)(AccountType.Physician));

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
            if (docId == 6)
            {
                newfilename = $"ProfilePhoto.{Path.GetExtension(UploadFile.FileName).Trim('.')}";
            }
            else if(docId == 7)
            {
                newfilename = $"Signature.{Path.GetExtension(UploadFile.FileName).Trim('.')}";
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
            if(ProfileInfo.AdminNotes != null)
            {
                provider.AdminNotes = ProfileInfo.AdminNotes;
            }

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
            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(ProviderInfo.Email);
            if(aspnetuserFetched != null)
            {
                return false;
            }

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

            PhysicianLocation location = new PhysicianLocation();
            location.PhysicianId = provider.PhysicianId;
            location.PhysicianName = provider.FirstName + " " + provider.LastName;
            location.Address = provider.City + " " + provider.ZipCode + " ";
            location.Latitude = ProviderInfo.Latitude;
            location.Longitude = ProviderInfo.Longitude;
            location.CreatedDate = DateTime.Now;

            await _physicianRepository.CreatePhysicianLocation(location);


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
            List<Region> regionList = new List<Region>();
            if(SchedulingData.IsPhysician)
            {
                var physicianRegionIds = _physicianRepository.GetRegionsByPhysicianId(SchedulingData.PhysicianId ?? 0).Select(x => x.RegionId);
                if(physicianRegionIds != null || physicianRegionIds?.Count() != 0)
                {
                    regionList = _commonRepository.GetAllRegions().Where(x => physicianRegionIds.Contains(x.RegionId)).ToList();
                }
            }
            else
            {
                regionList = _commonRepository.GetAllRegions();

                var physicianList = _physicianRepository.GetAllPhysicians();

                foreach (var physian in physicianList)
                {
                    SchedulingData.CreateShiftData.PhysicianList.Add(new SelectListItem()
                    {
                        Text = physian.FirstName + " " + physian.LastName,
                        Value = physian.PhysicianId.ToString(),
                    });
                }
            }

            foreach (var region in regionList)
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

            return SchedulingData;
        }

        public List<SelectListItem> GetPhysiciansByRegion(int regionId)
        {
            //var physicians = _physicianRepository.GetPhysiciansByRegionId(regionId);
            
            var physicians = _physicianRepository.GetIQueryablePhysicians().Where(x => x.PhysicianRegions.Where(x => regionId == 0 || x.RegionId == regionId).Any());

            List<SelectListItem> physicianList = new List<SelectListItem>();

            foreach(var physician in physicians)
            {
                physicianList.Add(new SelectListItem
                {
                    Value = physician.PhysicianId.ToString(),
                    Text = physician.FirstName + " " + physician.LastName,
                });
            }

            return physicianList;

        }

        public bool CheckAvailableShift(CreateShiftViewModel CreateShiftData)
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

            List<ShiftDetail> shiftDetails = new List<ShiftDetail>();

            shiftDetails.Add(new ShiftDetail
            {
                ShiftId = shift.ShiftId,
                ShiftDate = shift.StartDate,
                RegionId = CreateShiftData.RegionId,
                StartTime = CreateShiftData.StartTime,
                EndTime = CreateShiftData.EndTime,
                Status = (short)ShiftStatus.Approved,
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
                            Status = (short)ShiftStatus.Approved,
                            IsDeleted = false,
                        });
                    }
                }
            }

            var shiftDetailsFetched = _shiftRepository.GetShiftDetails().Where(x => x.IsDeleted == false && x.Shift.PhysicianId == CreateShiftData.PhysicianId).OrderBy(x => x.ShiftDate).ThenBy(x => x.StartTime).ToList();

            foreach(var shiftDetailFetched in shiftDetailsFetched)
            {
                if(CreateShiftData.ShiftDetailId == shiftDetailFetched.ShiftDetailId)
                {
                    continue;
                }
                var shiftDetailNew = shiftDetails.FirstOrDefault(x => x.ShiftDate == shiftDetailFetched.ShiftDate);
                if(shiftDetailNew != null)
                {
                    if(shiftDetailNew.StartTime < shiftDetailFetched.StartTime && shiftDetailNew.EndTime > shiftDetailFetched.StartTime)
                    {
                        return false;
                    }
                    else if(shiftDetailNew.StartTime > shiftDetailFetched.StartTime && shiftDetailNew.StartTime < shiftDetailFetched.EndTime)
                    {
                        return false;
                    }
                }
            }

            return true;
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
                Status = CreateShiftData.CreatorRole == "admin" ? (short)ShiftStatus.Approved : (short)ShiftStatus.Unapproved,
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
                            Status = CreateShiftData.CreatorRole == "admin" ? (short)ShiftStatus.Approved : (short)ShiftStatus.Unapproved,
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

        public RequestedShiftViewModel GetRequestedShiftViewModel(RequestedShiftViewModel RequestedShiftData)
        {
            var regions = _commonRepository.GetAllRegions();

            foreach (var region in regions)
            {
                RequestedShiftData.RegionList.Add(new SelectListItem()
                {
                    Text = region.Name,
                    Value = region.RegionId.ToString(),
                });
            }

            return RequestedShiftData;
        }

        public List<RequestShiftRowViewModel> GetShiftsList(int regionId)
        {
            List<RequestShiftRowViewModel> shiftsList = _shiftRepository.GetShiftDetails().Where(x => x.Status == (short)ShiftStatus.Unapproved).Select(x => new RequestShiftRowViewModel
            {
                ShiftDetailId = x.ShiftDetailId,
                PhysicianId = x.Shift.PhysicianId,
                Staff = x.Shift.Physician.FirstName + " " + x.Shift.Physician.LastName,
                Day = DateOnly.FromDateTime(x.ShiftDate).ToString("MMMM dd, yyyy"),
                Time = x.StartTime.ToString("h:mm tt") + " - " + x.EndTime.ToString("h:mm tt"),
                RegionName = x.ShiftDetailRegions.First().Region.Name,
                RegionId = x.RegionId ?? 0

            }).ToList();

            if(regionId != 0)
            {
                shiftsList = shiftsList.Where(x => x.RegionId == regionId).ToList();
            }
             
            return shiftsList;
        }

        public async Task<bool> ApproveShifts(List<int> shiftDetailIds, string modifiedBy)
        {
            List<ShiftDetail> shiftDetails = _shiftRepository.GetShiftDetails(shiftDetailIds);
            foreach (var shiftDetail in shiftDetails)
            {
                shiftDetail.Status = (short)ShiftStatus.Approved;
                shiftDetail.ModifiedBy = modifiedBy;
                shiftDetail.ModifiedDate = DateTime.Now;
            }

            await _shiftRepository.UpdateShiftDetails(shiftDetails);

            return true;
        }

        public async Task<bool> DeleteShifts(List<int> shiftDetailIds, string modifiedBy)
        {
            List<ShiftDetail> shiftDetails = _shiftRepository.GetShiftDetails(shiftDetailIds);
            foreach (var shiftDetail in shiftDetails)
            {
                shiftDetail.Status = (short)ShiftStatus.Deleted;
                shiftDetail.ModifiedBy = modifiedBy;
                shiftDetail.ModifiedDate = DateTime.Now;
            }

            await _shiftRepository.UpdateShiftDetails(shiftDetails);

            return true;
        }

        public CalendarViewModel GetCalendarViewModel(int regionId, int? physicianId = null)
        {
            CalendarViewModel calendarData = new CalendarViewModel();

            calendarData.Resources = _physicianRepository.GetIQueryablePhysicians().Where(x => (physicianId == null || x.PhysicianId == physicianId) && x.PhysicianRegions.Where(x => regionId == 0 || x.RegionId == regionId).Any())
                                                            .Select(x => new ResourceViewModel
                                                            {
                                                                PhysicianId = x.PhysicianId,
                                                                PhysicianName = x.FirstName + " " + x.LastName,
                                                            }).ToList();

            calendarData.Events = _shiftRepository.GetShiftDetails().Where(x => (physicianId == null || x.Shift.PhysicianId == physicianId) && x.IsDeleted == false && (regionId == 0 || x.RegionId == regionId)).Select(x => new EventViewModel
            {
                ShiftDetailId = x.ShiftDetailId,
                PhysicianId = x.Shift.PhysicianId,
                PhysicianName = x.Shift.Physician.FirstName + " " + x.Shift.Physician.LastName,
                ShiftDate = x.ShiftDate.ToString("yyyy-MM-dd"),
                StartTime = x.StartTime.ToString(),
                EndTime = x.EndTime.ToString(),
                IsApproved = x.Status == (short)ShiftStatus.Approved

            }).ToList();

            return calendarData;
        }

        public CreateShiftViewModel GetViewShiftViewModel(CreateShiftViewModel ViewShiftData)
        {
            var shiftDetails = _shiftRepository.GetShiftDetails().Include(x => x.Shift).Where(x => x.ShiftDetailId == ViewShiftData.ShiftDetailId).ToList();

            ShiftDetail shiftDetail = shiftDetails.FirstOrDefault() ?? new ShiftDetail();

            if(shiftDetail.ShiftDetailId != 0)
            {
                var regions = _commonRepository.GetAllRegions();

                foreach (var region in regions)
                {
                    ViewShiftData.RegionList.Add(new SelectListItem()
                    {
                        Text = region.Name,
                        Value = region.RegionId.ToString(),
                        Selected = region.RegionId == shiftDetail.RegionId
                    });
                }

                var physicianList = _physicianRepository.GetAllPhysicians();

                foreach (var physian in physicianList)
                {
                    ViewShiftData.PhysicianList.Add(new SelectListItem()
                    {
                        Text = physian.FirstName + " " + physian.LastName,
                        Value = physian.PhysicianId.ToString(),
                        Selected = physian.PhysicianId == shiftDetail.Shift.PhysicianId
                    });
                }

                ViewShiftData.RegionId = shiftDetail.RegionId ?? 0;
                ViewShiftData.PhysicianId = shiftDetail.Shift.PhysicianId;
                ViewShiftData.ShiftDate = DateOnly.FromDateTime(shiftDetail.ShiftDate).ToString("yyyy-MM-dd");
                ViewShiftData.StartTime = shiftDetail.StartTime;
                ViewShiftData.EndTime = shiftDetail.EndTime;
                ViewShiftData.CreatedBy = shiftDetail.Shift.CreatedBy;

                if(DateOnly.FromDateTime(shiftDetail.ShiftDate) < DateOnly.FromDateTime(DateTime.Now))
                {
                    ViewShiftData.IsOldShift = true;
                }
                else if(DateOnly.FromDateTime(shiftDetail.ShiftDate) == DateOnly.FromDateTime(DateTime.Now))
                {
                    if(shiftDetail.StartTime < TimeOnly.FromDateTime(DateTime.Now))
                    {
                        ViewShiftData.IsOldShift = true;
                    }
                }
                else
                {
                    ViewShiftData.IsOldShift = false;
                }

            }
            return ViewShiftData;
            
        }

        public async Task<bool> EditShift(CreateShiftViewModel EditShiftData)
        {
            var shiftDetail = _shiftRepository.GetShiftDetailByShiftDetailId(EditShiftData.ShiftDetailId);
            if (shiftDetail == null)
            {
                return false;
            }

            shiftDetail.ShiftDate = DateTime.ParseExact(EditShiftData.ShiftDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); 
            shiftDetail.StartTime = EditShiftData.StartTime;
            shiftDetail.EndTime = EditShiftData.EndTime;
            if(EditShiftData.CreatorRole == "physician")
            {
                shiftDetail.Status = (short)ShiftStatus.Unapproved;
            }

            await _shiftRepository.UpdateShiftDetail(shiftDetail);

            return true;
        }
        
        public async Task<bool> ReturnShift(CreateShiftViewModel ReturnShiftData)
        {
            var shiftDetail = _shiftRepository.GetShiftDetailByShiftDetailId(ReturnShiftData.ShiftDetailId);
            if (shiftDetail == null)
            {
                return false;
            }
            
            if(shiftDetail.Status == (short)ShiftStatus.Unapproved)
            {
                shiftDetail.Status = (short)ShiftStatus.Approved;
            }
            else if (shiftDetail.Status == (short)ShiftStatus.Approved)
            {
                shiftDetail.Status = (short)ShiftStatus.Unapproved;
            }

            await _shiftRepository.UpdateShiftDetail(shiftDetail);

            return true;
        }
        
        public async Task<bool> DeleteShift(CreateShiftViewModel DeleteShiftData)
        {
            var shiftDetail = _shiftRepository.GetShiftDetailByShiftDetailId(DeleteShiftData.ShiftDetailId);
            if (shiftDetail == null)
            {
                return false;
            }

            shiftDetail.IsDeleted = true;

            await _shiftRepository.UpdateShiftDetail(shiftDetail);

            return true;
        }

        public MDsOnCallViewModel GetMDsOnCallViewModel(MDsOnCallViewModel MDsOnCallData)
        {

            var regions = _commonRepository.GetAllRegions();

            foreach (var region in regions)
            {
                MDsOnCallData.RegionList.Add(new SelectListItem()
                {
                    Text = region.Name,
                    Value = region.RegionId.ToString(),
                });
            }

            return MDsOnCallData;
        }

        public MDsListViewModel GetMDsList(int regionId)
        {

            var activeMDs = _physicianRepository.GetIQueryablePhysicians().Where(z => z.Shifts.Where(y => y.ShiftDetails.Where(x => x.IsDeleted == false && DateOnly.FromDateTime(x.ShiftDate) == DateOnly.FromDateTime(DateTime.Now) && x.StartTime <= TimeOnly.FromDateTime(DateTime.Now) && x.EndTime >= TimeOnly.FromDateTime(DateTime.Now)).Any()).Any());

            var inactiveMDs = _physicianRepository.GetIQueryablePhysicians().Except(activeMDs);

            if (regionId != 0)
            {
                activeMDs = activeMDs.Where(x => x.PhysicianRegions.Where(x => regionId == 0 || x.RegionId == regionId).Any());
                inactiveMDs = inactiveMDs.Where(x => x.PhysicianRegions.Where(x => regionId == 0 || x.RegionId == regionId).Any());
            }

            List<MDCardViewModel> UnavailableMDs = activeMDs.Select(x => new MDCardViewModel
            {
                PhysicianId = x.PhysicianId,
                FullName = x.FirstName + " " + x.LastName,
                ProfilePhotoPath = x.Photo,
                OnCallStatus = (OnCallStatus.Unavailable).ToString()

            }).ToList();

            List<MDCardViewModel> AvailableMDs = inactiveMDs.Select(x => new MDCardViewModel
            {
                PhysicianId = x.PhysicianId,
                FullName = x.FirstName + " " + x.LastName,
                ProfilePhotoPath = x.Photo,
                OnCallStatus = (OnCallStatus.Available).ToString()

            }).ToList();

            MDsListViewModel MDsList = new MDsListViewModel();

            MDsList.AvailableMDs = AvailableMDs;
            MDsList.UnavailableMDs = UnavailableMDs;

            return MDsList;
        }

        #endregion

        #region PROVIDER LOCATION

        public List<ProviderLocationViewModel> GetProviderLocations()
        {
            List<ProviderLocationViewModel> ProviderLocations = _physicianRepository.GetAllPhysicianLocations().Where(x => x.Latitude != null && x.Longitude != null).Select(x => new ProviderLocationViewModel
            {
                LocationId = x.LocationId,
                PhysicianId = x.PhysicianId,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                PhysicianName = x.PhysicianName,
                Address = x.Address,

            }).ToList();

            return ProviderLocations;
        }

        #endregion
    }
}
