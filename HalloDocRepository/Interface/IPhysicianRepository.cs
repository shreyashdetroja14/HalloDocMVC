using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface IPhysicianRepository
    {
        List<Physician> GetAllPhysicians();

        Physician GetPhysicianByPhysicianId(int physicianId);

        IQueryable<Physician> GetIQueryablePhysicians();
    }
}
