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
    public class VendorRepository : IVendorRepository
    {
        private readonly HalloDocContext _context;

        public VendorRepository(HalloDocContext context)
        {
            _context = context;
        }

        public List<HealthProfessionType> GetAllHealthProfessionTypes()
        {
            var healthProfessionTypes = _context.HealthProfessionTypes.ToList();
            return healthProfessionTypes;
        }

        public List<HealthProfessional> GetAllHealthProfessionals()
        {
            var healthProfessionals = _context.HealthProfessionals.ToList();
            return healthProfessionals;
        }

        public HealthProfessional GetVendorById(int vendorId)
        {
            var vendor = _context.HealthProfessionals.FirstOrDefault(x => x.VendorId == vendorId);
            if (vendor == null)
            {
                return new HealthProfessional();
            }
            return vendor;
        }

        public IQueryable<HealthProfessional> GetIQueryableVendors()
        {
            return _context.HealthProfessionals.AsQueryable();
        }

        public async Task<HealthProfessional> CreateHealthProfessional(HealthProfessional vendor)
        {
            _context.HealthProfessionals.Add(vendor);
            await _context.SaveChangesAsync();

            return vendor;
        }
        
        public async Task<HealthProfessional> UpdateHealthProfessional(HealthProfessional vendor)
        {
            _context.HealthProfessionals.Update(vendor);
            await _context.SaveChangesAsync();

            return vendor;
        }
    }
}
