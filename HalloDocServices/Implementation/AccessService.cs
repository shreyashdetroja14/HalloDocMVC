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

        public AccessService(IRoleRepository roleRepository, IRequestRepository requestRepository, IUserRepository userRepository) 
        {
            _roleRepository = roleRepository;
            _requestRepository = requestRepository;
            _userRepository = userRepository;
        }


        string GetFullName(Admin adminUser, Physician physicianUser)
        {
            if (adminUser != null)
            {
                return $"{adminUser.FirstName} {adminUser.LastName}";
            }
            else if (physicianUser != null)
            {
                return $"{physicianUser.FirstName} {physicianUser.LastName}";
            }
            else
            {
                return "";
            }
        }

        string GetPhoneNumber(Admin adminUser, Physician physicianUser)
        {
            return adminUser?.Mobile ?? physicianUser?.Mobile;
        }

        string GetStatus(Admin adminUser, Physician physicianUser)
        {
            if (adminUser?.Status != null)
            {
                return Enum.GetName(typeof(HalloDocServices.Constants.Status), adminUser.Status.Value);
            }
            else if (physicianUser?.Status != null)
            {
                return Enum.GetName(typeof(HalloDocServices.Constants.Status), physicianUser.Status.Value);
            }
            else
            {
                return null;
            }
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

            foreach(var menu in menuList)
            {
                CreateRoleData.MenuItems.Add(new MenuItemViewModel
                {
                    MenuId = menu.MenuId,
                    Name = menu.Name,
                    AccountType = menu.AccountType,
                    IsChecked = false
                }) ;
            }

            var role = _roleRepository.GetRoleById(CreateRoleData.RoleId);

            CreateRoleData.IsCreateRole = (role.RoleId == 0);

            if(role.RoleId != 0)
            {
                var roleMenus = _roleRepository.GetRolesMenusByRoleId(role.RoleId);
                var roleMenuIds = roleMenus.Select(x => x.MenuId).ToList();

                foreach(var menuItem in CreateRoleData.MenuItems)
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

            if(role == null)
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

            var aspnet = _userRepository.GetIQueryableAspNetUsers().ToList();

            List<UserAccessRow> userAccessList = _userRepository.GetIQueryableAspNetUsers().Where(x => x.Users.Count() == 0)
                .Select(x => new UserAccessRow
                {

                    AspNetUserId = x.Id,
                    AdminId = x.AdminAspNetUsers.FirstOrDefault() != null ? x.AdminAspNetUsers.FirstOrDefault().AdminId : 0,
                    PhysicianId = x.PhysicianAspNetUsers.FirstOrDefault() != null ? x.PhysicianAspNetUsers.FirstOrDefault().PhysicianId : 0,
                    FullName = x.AdminAspNetUsers.FirstOrDefault() != null ? x.AdminAspNetUsers.FirstOrDefault().FirstName : (x.PhysicianAspNetUsers.FirstOrDefault() != null ? x.PhysicianAspNetUsers.FirstOrDefault().FirstName : ""),
                    PhoneNumber = x.AdminAspNetUsers.FirstOrDefault() != null ? x.AdminAspNetUsers.FirstOrDefault().Mobile : (x.PhysicianAspNetUsers.FirstOrDefault() != null ? x.PhysicianAspNetUsers.FirstOrDefault().Mobile : ""),
                    Status = x.AdminAspNetUsers.FirstOrDefault() != null ? ((Status)(x.AdminAspNetUsers.FirstOrDefault().Status)).ToString() : (x.PhysicianAspNetUsers.FirstOrDefault() != null ? ((Status)(x.PhysicianAspNetUsers.FirstOrDefault().Status)).ToString() : ""),
                    OpenRequests = x.PhysicianAspNetUsers.FirstOrDefault() != null ? x.PhysicianAspNetUsers.FirstOrDefault().Requests.Count() : newRequestCount
                }).ToList();

            return userAccessList;
        }
    }

    
}
