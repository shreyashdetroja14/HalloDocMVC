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
        #region GET

        List<Physician> GetAllPhysicians();

        Physician GetPhysicianByPhysicianId(int physicianId);

        IQueryable<Physician> GetIQueryablePhysicians();

        #endregion

        #region UPDATE

        Task<List<Physician>> Update(List<Physician> physicians);

        #endregion
    }
}
