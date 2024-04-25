using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface IAdminRepository
    {
        #region ADMIN

        #region GET
        Admin GetAdminByAdminId(int adminId);

        Admin GetAdminByRoleId(int roleId);

        Admin GetAdminByEmail(string email);

        #endregion

        #region UPDATE

        Task<bool> UpdateAdminAsync(Admin admin);

        #endregion

        #region ADD

        Task<Admin> CreateAdmin(Admin admin);

        #endregion

        #endregion

        #region ADMIN REGION

        #region GET

        List<AdminRegion> GetRegionsByAdminId(int adminId);

        #endregion

        #region UPDATE

        #endregion

        #region ADD

        Task<List<int>> AddAdminRegionsAsync(List<int> regionsToAdd, int adminId);

        #endregion

        #region REMOVE

        Task<List<AdminRegion>> RemoveAdminRegionsAsync(List<AdminRegion> regionsToRemove);

        #endregion


        #endregion
    }
}
