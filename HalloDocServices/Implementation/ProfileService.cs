using BCrypt.Net;
using DocumentFormat.OpenXml.Office2016.Excel;
using HalloDocEntities.Models;
using HalloDocRepository.Implementation;
using HalloDocRepository.Interface;
using HalloDocServices.Constants;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Implementation
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMailService _mailService;
        private readonly IEmailSMSLogRepository _emailSMSLogRepository;

        public ProfileService(IUserRepository userRepository, IPhysicianRepository physicianRepository, ICommonRepository commonRepository, IAdminRepository adminRepository, IRoleRepository roleRepository, IMailService mailService, IEmailSMSLogRepository emailSMSLogRepository)
        {
            _userRepository = userRepository;
            _physicianRepository = physicianRepository;
            _commonRepository = commonRepository;
            _adminRepository = adminRepository;
            _roleRepository = roleRepository;
            _mailService = mailService;
            _emailSMSLogRepository = emailSMSLogRepository;
        }

        public AdminProfileViewModel GetAdminProfileViewModelData(string aspnetuserId)
        {
            AdminProfileViewModel AdminProfileDetails = new AdminProfileViewModel();
            var aspnetuserIQueryable = _userRepository.GetIQueryableAspNetUserById(aspnetuserId);
            var aspnetuserFetched = aspnetuserIQueryable.Include(x => x.AdminAspNetUsers).ThenInclude(x => x.Role).Include(x => x.AdminAspNetUsers).ThenInclude(x => x.AdminRegions).FirstOrDefault();

            if (aspnetuserFetched != null)
            {
                var adminFetched = aspnetuserFetched.AdminAspNetUsers.FirstOrDefault();
                var adminRegionsFetched = adminFetched?.AdminRegions.ToList();

                AdminProfileDetails.AdminId = adminFetched?.AdminId??0;
                AdminProfileDetails.Username = aspnetuserFetched.UserName;
                AdminProfileDetails.Status = adminFetched?.Status;
                AdminProfileDetails.RoleId = adminFetched?.RoleId;

                var rolesList = _roleRepository.GetAllRoles().Where(x => x.IsDeleted != true && x.AccountType == (short)(AccountType.Admin));

                AdminProfileDetails.RoleList.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "Set a role",
                    Selected = true
                });

                foreach (var role in rolesList)
                {
                    AdminProfileDetails.RoleList.Add(new SelectListItem()
                    {
                        Value = role.RoleId.ToString(),
                        Text = role.Name,
                        Selected = (role.RoleId == adminFetched?.RoleId)
                    });
                }

                AdminProfileDetails.RoleName = adminFetched?.Role?.Name;

                AdminProfileDetails.FirstName = adminFetched?.FirstName ?? "";
                AdminProfileDetails.LastName = adminFetched?.LastName;
                AdminProfileDetails.Email = adminFetched?.Email ?? "";
                AdminProfileDetails.ConfirmEmail = adminFetched?.Email;
                AdminProfileDetails.PhoneNumber = adminFetched?.Mobile;

                if(adminRegionsFetched  != null)
                {
                    foreach (var adminregion in adminRegionsFetched)
                    {
                        AdminProfileDetails.AdminRegions.Add(adminregion.RegionId);
                    }
                }

                AdminProfileDetails.Address1 = adminFetched?.Address1;
                AdminProfileDetails.Address2 = adminFetched?.Address2;
                AdminProfileDetails.City = adminFetched?.City;
                AdminProfileDetails.RegionId = adminFetched?.RegionId;
                AdminProfileDetails.ZipCode = adminFetched?.ZipCode;
                AdminProfileDetails.SecondPhoneNumber = adminFetched?.AltPhone;

                var regions = _commonRepository.GetAllRegions();

                foreach(var region in regions)
                {
                    if(region.RegionId == adminFetched?.RegionId)
                    {
                        AdminProfileDetails.StateList.Add(new SelectListItem()
                        {
                            Text = region.Name,
                            Value = region.RegionId.ToString(),
                            Selected = true
                        });
                        continue;
                    }
                    AdminProfileDetails.StateList.Add(new SelectListItem()
                    {
                        Text = region.Name,
                        Value = region.RegionId.ToString()
                    });
                }
                
            }

            return AdminProfileDetails;
        }

        public async Task<bool> ResetPassword(AdminProfileViewModel AdminProfileDetails)
        {
            var adminFetched = _adminRepository.GetAdminByAdminId(AdminProfileDetails.AdminId);
            var aspnetuserFetched = _userRepository.GetAspNetUserById(adminFetched.AspNetUserId);
            if (aspnetuserFetched.Id != null)
            {
                aspnetuserFetched.PasswordHash = BCrypt.Net.BCrypt.HashPassword(AdminProfileDetails.Password);

                await _userRepository.UpdateAspNetUser(aspnetuserFetched);

                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAccountInfo(AdminProfileViewModel AdminProfileDetails)
        {
            var adminFetched = _adminRepository.GetAdminByAdminId(AdminProfileDetails.AdminId);
            var aspnetuserFetched = _userRepository.GetAspNetUserById(adminFetched.AspNetUserId);

            if (aspnetuserFetched.Id == null || adminFetched.AdminId == 0)
            {
                return false;
            }

            aspnetuserFetched.UserName = AdminProfileDetails.Username;

            await _userRepository.UpdateAspNetUser(aspnetuserFetched);

            adminFetched.Status = (short)(AdminProfileDetails.Status ?? 0);
            adminFetched.RoleId = AdminProfileDetails.RoleId;

            await _adminRepository.UpdateAdminAsync(adminFetched);

            return true;

        }

        public async Task<bool> UpdateAdminInfo(AdminProfileViewModel AdminProfileDetails)
        {
            var adminFetched = _adminRepository.GetAdminByAdminId(AdminProfileDetails.AdminId);
            if(adminFetched.AdminId == 0)
            {
                return false;
            }

            adminFetched.FirstName = AdminProfileDetails.FirstName;
            adminFetched.LastName = AdminProfileDetails.LastName;
            adminFetched.Email = AdminProfileDetails.Email;
            adminFetched.Mobile = AdminProfileDetails.PhoneNumber;
            
            await _adminRepository.UpdateAdminAsync(adminFetched);
            
            var aspnetuserFetched = _userRepository.GetAspNetUserById(adminFetched.AspNetUserId);

            aspnetuserFetched.Email = AdminProfileDetails.Email;
            aspnetuserFetched.PhoneNumber = AdminProfileDetails.PhoneNumber;

            await _userRepository.UpdateAspNetUser(aspnetuserFetched);

            var adminRegions = _adminRepository.GetRegionsByAdminId(adminFetched.AdminId);

            var regionsToRemove = adminRegions.Where(x => !AdminProfileDetails.AdminRegions.Contains(x.RegionId)).ToList();
            var regionsToAdd = AdminProfileDetails.AdminRegions.Except(adminRegions.Select(x => x.RegionId)).ToList();

            if(regionsToAdd.Count > 0)
            {
                regionsToAdd = await _adminRepository.AddAdminRegionsAsync(regionsToAdd, adminFetched.AdminId);
            }

            if(regionsToRemove.Count > 0)
            {
                regionsToRemove = await _adminRepository.RemoveAdminRegionsAsync(regionsToRemove);
            }

            return true;
        }

        public async Task<bool> UpdateBillingInfo(AdminProfileViewModel AdminProfileDetails)
        {
            var adminFetched = _adminRepository.GetAdminByAdminId(AdminProfileDetails.AdminId);
            if (adminFetched.AdminId != 0)
            {
                adminFetched.Address1 = AdminProfileDetails.Address1;
                adminFetched.Address2 = AdminProfileDetails.Address2;
                adminFetched.City = AdminProfileDetails.City;
                adminFetched.RegionId = AdminProfileDetails.RegionId;
                adminFetched.ZipCode = AdminProfileDetails.ZipCode;
                adminFetched.AltPhone = AdminProfileDetails.SecondPhoneNumber;

                await _adminRepository.UpdateAdminAsync(adminFetched);

                return true;
            }
            return false;
        }

        public async Task<bool> SendMailToAdmin(EditProviderViewModel MailDetails)
        {
            var physician = _physicianRepository.GetPhysicianByPhysicianId(MailDetails.ProviderId);
            var aspnetUser = _userRepository.GetIQueryableAspNetUserById(physician.CreatedBy ?? "");

            var admin = aspnetUser.Select(x => x.AdminAspNetUsers.FirstOrDefault()).FirstOrDefault();
            

            string subject = "Profile edit request for Physician ID: " + MailDetails.ProviderId;

            string body = MailDetails.AdminMessage ?? "";

            List<string> receivers = new List<string>
                {
                    admin?.Email ?? ""
                };

            bool isMailSent = await _mailService.SendMail(receivers, subject, body, false);

            if(isMailSent)
            {
                EmailLog emailLog = new EmailLog();
                emailLog.EmailTemplate = body;
                emailLog.SubjectName = subject;
                emailLog.EmailId = admin?.Email ?? "";
                emailLog.Action = (int)ActionEnum.ProfileUpdateMessage;
                emailLog.RoleId = (int)AccountType.Admin;
                emailLog.AdminId = admin?.AdminId;
                emailLog.PhysicianId = physician?.PhysicianId;
                emailLog.CreatedDate = DateTime.Now;
                emailLog.SentDate = DateTime.Now;
                emailLog.IsEmailSent = isMailSent;
                emailLog.SentTries = 1;
                emailLog.RecipientName = admin?.FirstName + " " + admin?.LastName;

                await _emailSMSLogRepository.CreateEmailLog(emailLog);

                SmsLog smsLog = new SmsLog();
                smsLog.SmsTemplate = body;
                smsLog.MobileNumber = admin?.Mobile ?? "";
                smsLog.Action = (int)ActionEnum.ProfileUpdateMessage;
                smsLog.RoleId = (int)AccountType.Admin;
                smsLog.AdminId = admin?.AdminId;
                smsLog.PhysicianId = physician?.PhysicianId;
                smsLog.CreatedDate = DateTime.Now;
                smsLog.SentDate = DateTime.Now;
                smsLog.IsSmsSent = isMailSent;
                smsLog.SentTries = 1;
                smsLog.RecipientName = admin?.FirstName + " " + admin?.LastName;

                await _emailSMSLogRepository.CreateSmsLog(smsLog);
            }

            return isMailSent;
        }

    }
}
