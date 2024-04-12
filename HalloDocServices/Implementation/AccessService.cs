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
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Implementation
{
    public class AccessService : IAccessService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IAdminRepository _adminRepository;

        public AccessService(IRoleRepository roleRepository, IRequestRepository requestRepository, IUserRepository userRepository, ICommonRepository commonRepository, IAdminRepository adminRepository)
        {
            _roleRepository = roleRepository;
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _commonRepository = commonRepository;
            _adminRepository = adminRepository;
        }

        public AccessViewModel GetAccessViewModel(AccessViewModel AccessData)
        {
            var roles = _roleRepository.GetAllRoles().Where(x => x.IsDeleted == false);
            List<AccountAccessViewModel> AccountList = new List<AccountAccessViewModel>();
            foreach (var role in roles)
            {
                AccountList.Add(new AccountAccessViewModel
                {
                    RoleId = role.RoleId,
                    RoleName = role.Name,
                    AccountType = ((AccountType)role.AccountType).ToString(),
                });
            }

            AccessData.AccountList = AccountList;

            return AccessData;
        }

        public CreateRoleViewModel GetCreateRoleViewModel(CreateRoleViewModel CreateRoleData)
        {
            var menuList = _roleRepository.GetAllMenus();

            foreach (var menu in menuList)
            {
                CreateRoleData.MenuItems.Add(new MenuItemViewModel
                {
                    MenuId = menu.MenuId,
                    Name = menu.Name,
                    AccountType = menu.AccountType,
                    IsChecked = false
                });
            }

            var role = _roleRepository.GetRoleById(CreateRoleData.RoleId);

            CreateRoleData.IsCreateRole = (role.RoleId == 0);

            if (role.RoleId != 0)
            {
                var roleMenus = _roleRepository.GetRolesMenusByRoleId(role.RoleId);
                var roleMenuIds = roleMenus.Select(x => x.MenuId).ToList();

                foreach (var menuItem in CreateRoleData.MenuItems)
                {
                    if (roleMenuIds.Contains(menuItem.MenuId))
                    {
                        menuItem.IsChecked = true;
                    }
                }

                CreateRoleData.RoleName = role.Name;
                CreateRoleData.AccountType = role.AccountType;
            }

            return CreateRoleData;
        }

        public async Task<bool> CreateRole(CreateRoleViewModel CreateRoleData)
        {
            Role role = new Role();
            role.Name = CreateRoleData.RoleName ?? "";
            role.AccountType = (short)(CreateRoleData.AccountType);
            role.CreatedBy = CreateRoleData.CreatedBy ?? "";
            role.CreatedDate = DateTime.Now;
            role.IsDeleted = false;

            role = await _roleRepository.CreateRoleAsync(role);

            await _roleRepository.CreateRoleMenusAsync(CreateRoleData.MenuIds, role.RoleId);

            return true;
        }

        public async Task<bool> DeleteRole(int roleId, string modifiedBy)
        {
            var role = _roleRepository.GetRoleById(roleId);
            role.IsDeleted = true;
            role.ModifiedDate = DateTime.Now;
            role.ModifiedBy = modifiedBy;

            await _roleRepository.UpdateRoleAsync(role);

            return true;
        }

        public async Task<bool> EditRole(CreateRoleViewModel EditRoleData)
        {
            var role = _roleRepository.GetRoleById(EditRoleData.RoleId);
            var roleMenus = _roleRepository.GetRolesMenusByRoleId(EditRoleData.RoleId);

            if (role == null)
            {
                return false;
            }

            role.Name = EditRoleData.RoleName;
            role.AccountType = (short)(EditRoleData.AccountType);
            role.ModifiedBy = EditRoleData.ModifiedBy;
            role.ModifiedDate = DateTime.Now;

            await _roleRepository.UpdateRoleAsync(role);

            var menusToRemove = roleMenus.Where(x => !EditRoleData.MenuIds.Contains(x.MenuId)).ToList();
            var menuIdsToAdd = EditRoleData.MenuIds.Except(roleMenus.Select(x => x.MenuId)).ToList();

            if (menuIdsToAdd.Count > 0)
            {
                menuIdsToAdd = await _roleRepository.CreateRoleMenusAsync(menuIdsToAdd, EditRoleData.RoleId);
            }

            if (menusToRemove.Count > 0)
            {
                menusToRemove = await _roleRepository.RemoveRoleMenusAsync(menusToRemove);
            }

            return true;
        }

        public async Task<List<UserAccessRow>> GetUserAccessList()
        {
            /*var aspnetusers = _userRepository.GetIQueryableAspNetUsers().Include(x => x.AdminAspNetUsers).ThenInclude(x => x.Role).Include(x => x.PhysicianAspNetUsers).ThenInclude(x => x.Role).Include(x => x.PhysicianAspNetUsers).ThenInclude(x => x.Requests);*/
            var newRequestCount = await _requestRepository.GetNewRequestCount();

            List<UserAccessRow> userAccessList = _userRepository.GetIQueryableAspNetUsers().Where(x => x.AdminAspNetUsers.Count() != 0 || x.PhysicianAspNetUsers.Count() != 0)
                     .Select(x => new UserAccessRow
                     {
                         AspNetUserId = x.Id,
                         AdminId = x.AdminAspNetUsers.FirstOrDefault() != null ? x.AdminAspNetUsers.FirstOrDefault().AdminId : 0,
                         PhysicianId = x.PhysicianAspNetUsers.FirstOrDefault() != null ? x.PhysicianAspNetUsers.FirstOrDefault().PhysicianId : 0,
                         AccountType = x.AdminAspNetUsers.FirstOrDefault() != null ? 1 : (x.PhysicianAspNetUsers.FirstOrDefault() != null ? 2 : 0),
                         FullName = x.AdminAspNetUsers.FirstOrDefault() != null ? x.AdminAspNetUsers.FirstOrDefault().FirstName : (x.PhysicianAspNetUsers.FirstOrDefault() != null ? x.PhysicianAspNetUsers.FirstOrDefault().FirstName : ""),
                         PhoneNumber = x.AdminAspNetUsers.FirstOrDefault() != null ? x.AdminAspNetUsers.FirstOrDefault().Mobile : (x.PhysicianAspNetUsers.FirstOrDefault() != null ? x.PhysicianAspNetUsers.FirstOrDefault().Mobile : ""),
                         Status = x.AdminAspNetUsers.FirstOrDefault() != null ? ((Status)(x.AdminAspNetUsers.FirstOrDefault().Status)).ToString() : (x.PhysicianAspNetUsers.FirstOrDefault() != null ? ((Status)(x.PhysicianAspNetUsers.FirstOrDefault().Status)).ToString() : ""),
                         OpenRequests = x.PhysicianAspNetUsers.FirstOrDefault() != null ? x.PhysicianAspNetUsers.FirstOrDefault().Requests.Where(x => x.Status == 2).Count() : newRequestCount
                     }).ToList();

            return userAccessList;
        }

        public AdminProfileViewModel GetCreateAdminViewModel(AdminProfileViewModel AdminDetails)
        {
            var rolesList = _roleRepository.GetAllRoles();

            AdminDetails.RoleList.Add(new SelectListItem()
            {
                Value = "",
                Text = "Set a role",
                Selected = true
            });

            foreach (var role in rolesList)
            {
                AdminDetails.RoleList.Add(new SelectListItem()
                {
                    Value = role.RoleId.ToString(),
                    Text = role.Name,
                });
            }

            var regions = _commonRepository.GetAllRegions();

            foreach (var region in regions)
            {
                AdminDetails.StateList.Add(new SelectListItem()
                {
                    Value = region.RegionId.ToString(),
                    Text = region.Name,
                });
            }

            return AdminDetails;
        }

        public async Task<bool> CreateAdmin(AdminProfileViewModel AdminDetails)
        {
            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(AdminDetails.Email);

            if (aspnetuserFetched != null)
            {
                return false;
            }

            var aspnetuserNew = new AspNetUser();

            aspnetuserNew.Id = Guid.NewGuid().ToString();

            aspnetuserNew.UserName = AdminDetails.Username;
            aspnetuserNew.PasswordHash = BCrypt.Net.BCrypt.HashPassword(AdminDetails.Password);
            aspnetuserNew.Email = AdminDetails.Email;
            aspnetuserNew.PhoneNumber = AdminDetails.PhoneNumber;
            aspnetuserNew.CreatedDate = DateTime.Now;

            aspnetuserNew = await _userRepository.CreateAspNetUser(aspnetuserNew);

            var aspnetuserRole = new AspNetUserRole();
            aspnetuserRole.AspNetUserId = aspnetuserNew.Id;
            aspnetuserRole.RoleId = "94cfca38-7a3a-43ee-8f95-6afca87753b5";

            await _userRepository.CreateAspNetUserRole(aspnetuserRole);

            var admin = new Admin();

            admin.AspNetUserId = aspnetuserNew.Id;
            admin.FirstName = AdminDetails.FirstName;
            admin.LastName = AdminDetails.LastName;
            admin.Email = AdminDetails.Email;
            admin.Mobile = AdminDetails.PhoneNumber;
            admin.Address1 = AdminDetails.Address1;
            admin.Address2 = AdminDetails.Address2;
            admin.City = AdminDetails.City;
            admin.RegionId = AdminDetails.RegionId;
            admin.ZipCode = AdminDetails.ZipCode;
            admin.AltPhone = AdminDetails.SecondPhoneNumber;
            admin.CreatedBy = AdminDetails.CreatedBy;
            admin.CreatedDate = DateTime.Now;
            admin.Status = (short)(AdminDetails.Status ?? 0);
            admin.IsDeleted = false;
            admin.RoleId = AdminDetails.RoleId;

            admin = await _adminRepository.CreateAdmin(admin);

            if(AdminDetails.AdminRegions.Count > 0)
            {
                await _adminRepository.AddAdminRegionsAsync(AdminDetails.AdminRegions, admin.AdminId);
            }

            return true;

        }
    }


}
