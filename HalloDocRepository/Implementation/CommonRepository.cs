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
    public class CommonRepository : ICommonRepository
    {
        private readonly HalloDocContext _context;

        public CommonRepository(HalloDocContext context) 
        {
            _context = context;
        }
        public List<CaseTag> GetAllCaseTags()
        {
            var caseTags = _context.CaseTags.ToList();
            return caseTags;
        }

        public List<Region> GetAllRegions()
        {
            var regions = _context.Regions.ToList();
            return regions;
        }

        public Region GetRegionById(int regionId)
        {
            var region = _context.Regions.FirstOrDefault(x => x.RegionId == regionId);
            return region ?? new Region();
        }

        public async Task<OrderDetail> CreateOrder(OrderDetail order)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public EncounterForm GetEncounterFormByRequestId(int requestId)
        {
            var EncounterForm = _context.EncounterForms.FirstOrDefault(x => x.RequestId == requestId);
            /*if(EncounterForm == null)
            {
                return new EncounterForm();
            }*/
            return EncounterForm;
        }

        public async Task<bool> UpdateEncounterForm(EncounterForm encounterForm)
        {
            _context.Update(encounterForm);
            await _context.SaveChangesAsync();

            return true;
        }

        
    }
}
