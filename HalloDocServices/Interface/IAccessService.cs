using HalloDocServices.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IAccessService
    {
        AccessViewModel GetAccessViewModel(AccessViewModel AccessData);

        CreateRoleViewModel GetCreateRoleViewModel(CreateRoleViewModel CreateRoleData);

        Task<bool> CreateRole(CreateRoleViewModel CreateRoleData);

        Task<bool> DeleteRole(int roleId, string modifiedBy);

        Task<bool> EditRole(CreateRoleViewModel EditRoleData);

        Task<List<UserAccessRow>> GetUserAccessList();

        AdminProfileViewModel GetCreateAdminViewModel(AdminProfileViewModel AdminDetails);

        Task<bool> CreateAdmin(AdminProfileViewModel AdminDetails);
    }
}
