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

        Region GetRegionById(int regionId);

        // Order Details

        Task<OrderDetail> CreateOrder(OrderDetail order);


        // Encounter Form

        EncounterForm GetEncounterFormByRequestId(int requestId);

        Task<bool> UpdateEncounterForm(EncounterForm encounterForm);

        Task<EncounterForm> CreateEncounterForm(EncounterForm encounterForm);


        
    }
}
