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


        #endregion
    }
}
