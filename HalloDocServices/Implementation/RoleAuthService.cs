using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Implementation
{
    public class RoleAuthService : IRoleAuthService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleAuthService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public bool CheckAccess(int roleId, string[] menus)
        {
            if(roleId == 0)
            {
                return false;
            }
            var roleMenus = _roleRepository.GetAllRolesMenus().Where(x => x.RoleId == roleId).Select(x => x.Menu).ToList();

            foreach( var menu in roleMenus)
            {
                if(menus.Contains(menu.Name))
                {
                    return true;
                }
            }

            return false;
        }

        public List<string> GetMenuNamesByRoleId(int roleId)
        {
            var roleMenus = _roleRepository.GetAllRolesMenus().Where(x => x.RoleId == roleId).Select(x => x.Menu.Name).ToList();
            return roleMenus;
        }
    }
}
