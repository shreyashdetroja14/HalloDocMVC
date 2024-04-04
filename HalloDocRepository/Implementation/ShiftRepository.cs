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
    public class ShiftRepository : IShiftRepository
    {
        private readonly HalloDocContext _context;

        public ShiftRepository(HalloDocContext context) 
        {
            _context = context;
        }

        public async Task<Shift> CreateShift(Shift shift)
        {
            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            return shift;
        }

        public async Task<List<ShiftDetail>> CreateShiftDetails(List<ShiftDetail> shiftDetails)
        {
            _context.ShiftDetails.AddRange(shiftDetails);
            await _context.SaveChangesAsync();

            return shiftDetails;
        }

        public async Task<List<ShiftDetailRegion>> CreateShiftDetailRegions(List<ShiftDetailRegion> shiftDetailRegions)
        {
            _context.ShiftDetailRegions.AddRange(shiftDetailRegions);
            await _context.SaveChangesAsync();

            return shiftDetailRegions;
        }

        public List<ShiftDetail> GetAllShiftDetails()
        {
            var shiftDetails = _context.ShiftDetails.ToList();
            return shiftDetails;
        }

        public List<ShiftDetail> GetShiftDetailsByRegionId(int regionId)
        {
            var shiftDetails = _context.ShiftDetails.Where(x => x.RegionId == regionId).ToList();
            return shiftDetails;
        }

        public IQueryable<ShiftDetail> GetShiftDetails()
        {
            var shiftDetails = _context.ShiftDetails.AsQueryable();

            return shiftDetails;
                
        }

        public List<ShiftDetail> GetShiftDetails(List<int> shiftDetailIds)
        {
            var shiftDetails = _context.ShiftDetails.Where(x => shiftDetailIds.Contains(x.ShiftDetailId)).ToList();
            return shiftDetails;
        }

        public async Task<List<ShiftDetail>> UpdateShiftDetails(List<ShiftDetail> shiftDetails)
        {
            _context.ShiftDetails.UpdateRange(shiftDetails);
            await _context.SaveChangesAsync();

            return shiftDetails;
        }
    }
}
