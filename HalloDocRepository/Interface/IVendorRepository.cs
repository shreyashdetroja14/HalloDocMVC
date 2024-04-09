using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface IVendorRepository
    {
        #region HEALTH PROFESSIONALS

        List<HealthProfessional> GetAllHealthProfessionals();

        HealthProfessional GetVendorById(int vendorId);

        IQueryable<HealthProfessional> GetIQueryableVendors();

        Task<HealthProfessional> CreateHealthProfessional(HealthProfessional vendor);

        Task<HealthProfessional> UpdateHealthProfessional(HealthProfessional vendor);

        #endregion

        #region HEALTH PROFESSIONAL TYPES

        List<HealthProfessionType> GetAllHealthProfessionTypes();

        #endregion
    }
}
