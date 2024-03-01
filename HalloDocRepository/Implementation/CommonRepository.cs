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
    }
}
