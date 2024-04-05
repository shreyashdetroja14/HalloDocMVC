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
    public class PhysicianRepository : IPhysicianRepository
    {
        private readonly HalloDocContext _context;
        public PhysicianRepository(HalloDocContext context)
        {
            _context = context;
        }

        #region GET

        public List<Physician> GetAllPhysicians()
        {
            List<Physician> physicians = _context.Physicians.ToList();
            return physicians;
        }

        public IQueryable<Physician> GetIQueryablePhysicians()
        {
            var physicians = _context.Physicians.AsQueryable();
            return physicians;
        }
        
        public IQueryable<Physician> GetIQueryablePhysicians(int physicianId)
        {
            var physician = _context.Physicians.AsQueryable().Where(x => x.PhysicianId == physicianId);
            return physician;
        }

        public Physician GetPhysicianByPhysicianId(int physicianId)
        {
            var physician = _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianId);
            return physician;
        }

        #endregion

        #region UPDATE

        public async Task<List<Physician>> Update(List<Physician> physicians)
        {
            _context.UpdateRange(physicians);
            await _context.SaveChangesAsync();

            return physicians;
        }

        public async Task<Physician> Update(Physician physician)
        {
            _context.Update(physician);
            await _context.SaveChangesAsync();

            return physician;
        }

        public async Task<Physician> CreateAsync(Physician physician)
        {
            _context.Add(physician);
            await _context.SaveChangesAsync();

            return physician;
        }

        #endregion


        #region PHYSICIAN REGION

        #region GET

        public List<PhysicianRegion> GetRegionsByPhysicianId(int physicianId)
        {
            var physicianRegions = _context.PhysicianRegions.Where(x => x.PhysicianId == physicianId).ToList();
            return physicianRegions;
        }

        public List<Physician> GetPhysiciansByRegionId(int regionId)
        {
            var physicians = _context.PhysicianRegions.Where(x => regionId == 0 || x.RegionId == regionId).Select(x => x.Physician).ToList();
            return physicians;
        }


        #endregion

        #region UPDATE

        #endregion

        #region ADD

        public async Task<List<int>> AddPhysicianRegionsAsync(List<int> regionsToAdd, int physicianId)
        {
            foreach (var regionId in regionsToAdd)
            {
                _context.PhysicianRegions.Add(new PhysicianRegion { PhysicianId = physicianId, RegionId = regionId });
            }

            await _context.SaveChangesAsync();

            return regionsToAdd;
        }



        #endregion

        #region REMOVE

        public async Task<List<PhysicianRegion>> RemovePhysicianRegionsAsync(List<PhysicianRegion> regionsToRemove)
        {
            _context.PhysicianRegions.RemoveRange(regionsToRemove);
            await _context.SaveChangesAsync();

            return regionsToRemove;
        }

        #endregion

        #endregion
    }
}
