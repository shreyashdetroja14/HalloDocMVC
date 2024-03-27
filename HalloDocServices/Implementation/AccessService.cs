using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public AccessService(IRoleRepository roleRepository) 
        {
            _roleRepository = roleRepository;
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

            return CreateRoleData;
        }

        public async Task<bool> CreateRole(CreateRoleViewModel CreateRoleData)
        {
            Role role = new Role();
            role.Name = CreateRoleData.RoleName ?? "";
            role.AccountType = (short)(CreateRoleData.AccountType ?? 0);
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
    }

    public enum AccountType
    {
        Admin = 1,
        Physician = 2,
        Patient = 3,
    }
}
