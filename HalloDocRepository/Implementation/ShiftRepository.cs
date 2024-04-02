using HalloDocEntities.Data;
using HalloDocRepository.Interface;
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
    }
}
