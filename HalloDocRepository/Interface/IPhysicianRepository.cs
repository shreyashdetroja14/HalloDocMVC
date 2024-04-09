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
        #region Physician

        List<Physician> GetAllPhysicians();

        Physician GetPhysicianByPhysicianId(int physicianId);

        IQueryable<Physician> GetIQueryablePhysicians();

        IQueryable<Physician> GetIQueryablePhysicians(int physicianId);

        Task<List<Physician>> Update(List<Physician> physicians);

        Task<Physician> Update(Physician physician);

        Task<Physician> CreateAsync(Physician physician);

        #endregion

        #region PHYSICIAN REGION

        List<PhysicianRegion> GetRegionsByPhysicianId(int physicianId);

        Task<List<int>> AddPhysicianRegionsAsync(List<int> regionsToAdd, int physicianId);

        Task<List<PhysicianRegion>> RemovePhysicianRegionsAsync(List<PhysicianRegion> regionsToRemove);

        List<Physician> GetPhysiciansByRegionId(int regionId);

        #endregion

        #region PHYSICIAN LOCATION

        List<PhysicianLocation> GetAllPhysicianLocations();

        Task<PhysicianLocation> CreatePhysicianLocation(PhysicianLocation physicianLocation);

        #endregion
    }
}
