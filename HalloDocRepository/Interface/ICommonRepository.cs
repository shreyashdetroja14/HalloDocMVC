using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface ICommonRepository
    {

        // Case Tags

        List<CaseTag> GetAllCaseTags();


        // Region

        List<Region> GetAllRegions();


        // Health Profession Type

        List<HealthProfessionType> GetAllHealthProfessionTypes();


        // Health Professionals

        List<HealthProfessional> GetAllHealthProfessionals();

        HealthProfessional GetVendorById(int vendorId);


        // Order Details

        Task<OrderDetail> CreateOrder(OrderDetail order);


        // Encounter Form

        EncounterForm GetEncounterFormByRequestId(int requestId);

        Task<bool> UpdateEncounterForm(EncounterForm encounterForm);


        #region ROLE

        Role GetRoleById(int roleId);

        List<Role> GetAllRoles();

        #endregion
    }
}
