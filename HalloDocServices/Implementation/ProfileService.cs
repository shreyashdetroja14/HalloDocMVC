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
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly INotesAndLogsRepository _notesAndLogsRepository;
        private readonly ICommonRepository _commonRepository;

        public ProfileService(IUserRepository userRepository, IPhysicianRepository physicianRepository, INotesAndLogsRepository notesAndLogsRepository, ICommonRepository commonRepository)
        {
            _userRepository = userRepository;
            _physicianRepository = physicianRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
            _commonRepository = commonRepository;
        }
        public AdminProfileViewModel GetAdminProfileViewModelData(string aspnetuserId)
        {
            AdminProfileViewModel AdminProfileDetails = new AdminProfileViewModel();
            var aspnetuserIQueryable = _userRepository.GetIQueryableAspNetUserById(aspnetuserId);
            var aspnetuserFetched = aspnetuserIQueryable.Include(x => x.AdminAspNetUsers).ThenInclude(x => x.Role).Include(x => x.AdminAspNetUsers).ThenInclude(x => x.AdminRegions).FirstOrDefault();

            if (aspnetuserFetched != null)
            {
                var adminFetched = aspnetuserFetched.AdminAspNetUsers.FirstOrDefault();

                AdminProfileDetails.AdminId = adminFetched?.AdminId??0;
                AdminProfileDetails.Username = aspnetuserFetched.UserName;
                AdminProfileDetails.Status = adminFetched?.Status;
                AdminProfileDetails.RoleId = adminFetched?.RoleId;
                AdminProfileDetails.RoleName = adminFetched?.Role?.Name;

                AdminProfileDetails.FirstName = adminFetched?.FirstName;
                AdminProfileDetails.LastName = adminFetched?.LastName;
                AdminProfileDetails.Email = adminFetched?.Email;
                AdminProfileDetails.ConfirmEmail = adminFetched?.Email;
                AdminProfileDetails.PhoneNumber = adminFetched?.Mobile;

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
    }
}
