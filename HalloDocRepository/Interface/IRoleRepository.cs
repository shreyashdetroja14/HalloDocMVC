using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface IRoleRepository
    {
        #region ROLE

        Role GetRoleById(int roleId);

        List<Role> GetAllRoles();

        Task<Role> CreateRoleAsync(Role role);

        Task<Role> UpdateRoleAsync(Role role);

        #endregion

        #region ROLEMENU

        Task<List<int>> CreateRoleMenusAsync(List<int> MenuIds, int roleId);

        #endregion

        #region MENU

        List<Menu> GetAllMenus();

        #endregion
    }
}
