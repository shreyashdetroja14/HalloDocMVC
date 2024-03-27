using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Implementation
{
    public class RoleRepository : IRoleRepository
    {
        private readonly HalloDocContext _context;

        public RoleRepository(HalloDocContext context)
        {
            _context = context;
        }

        #region ROLE

        public Role GetRoleById(int roleId)
        {
            var role = _context.Roles.FirstOrDefault(x => x.RoleId == roleId);
            return role ?? new Role();
        }

        public List<Role> GetAllRoles()
        {
            var roles = _context.Roles.ToList();
            return roles;
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return role;
        }

        public async Task<Role> UpdateRoleAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            return role;
        }


        #endregion

        #region ROLE MENU

        public async Task<List<int>> CreateRoleMenusAsync(List<int> MenuIds, int roleId)
        {
            foreach (var menuId in MenuIds)
            {
                _context.RoleMenus.Add(new RoleMenu { RoleId = roleId, MenuId = menuId });
            }

            await _context.SaveChangesAsync();

            return MenuIds;
        }


        #endregion

        #region MENU

        public List<Menu> GetAllMenus()
        {
            var menus = _context.Menus.ToList();
            return menus;
        }

        #endregion
    }
}
