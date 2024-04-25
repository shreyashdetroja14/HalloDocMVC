using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Implementation
{
    public class AdminRepository : IAdminRepository
    {
        private readonly HalloDocContext _context;

        public AdminRepository(HalloDocContext context)
        {
            _context = context;
        }

        #region ADMIN

        #region GET

        public Admin GetAdminByAdminId(int adminId)
        {
            var adminFetched = _context.Admins.FirstOrDefault(x => x.AdminId == adminId);
            return adminFetched ?? new Admin();
        }

        public Admin GetAdminByRoleId(int roleId)
        {
            var adminFetched = _context.Admins.FirstOrDefault(x => x.RoleId == roleId);
            return adminFetched ?? new Admin();
        }
        
        public Admin GetAdminByEmail(string email)
        {
            var adminFetched = _context.Admins.FirstOrDefault(x => x.Email == email);
            return adminFetched ?? new Admin();
        }


        #endregion

        #region UPDATE

        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            _context.Update(admin);
            await _context.SaveChangesAsync();

            return true;
        }

        #endregion

        #region ADD

        public async Task<Admin> CreateAdmin(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return admin;
        }

        #endregion

        #region REMOVE

        #endregion

        #endregion

        #region ADMIN REGION

        #region GET

        public List<AdminRegion> GetRegionsByAdminId(int adminId)
        {
            var adminRegions = _context.AdminRegions.Where(x => x.AdminId == adminId).ToList();
            return adminRegions;
        }



        #endregion

        #region UPDATE

        #endregion

        #region ADD

        public async Task<List<int>> AddAdminRegionsAsync(List<int> regionsToAdd, int adminId)
        {
            foreach (var regionId in regionsToAdd)
            {
                _context.AdminRegions.Add(new AdminRegion { AdminId = adminId, RegionId = regionId });
            }

            await _context.SaveChangesAsync();

            return regionsToAdd;
        }



        #endregion

        #region REMOVE

        public async Task<List<AdminRegion>> RemoveAdminRegionsAsync(List<AdminRegion> regionsToRemove)
        {
            _context.AdminRegions.RemoveRange(regionsToRemove);
            await _context.SaveChangesAsync();

            return regionsToRemove;
        }

        #endregion

        #endregion
    }
}
