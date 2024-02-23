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

        public async Task<List<Physician>> GetAllPhysicians()
        {
            List<Physician> physicians = await _context.Physicians.ToListAsync();
            return physicians;
        }
    }
}
