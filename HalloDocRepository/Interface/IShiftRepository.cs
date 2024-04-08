using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface IShiftRepository
    {
        #region SHIFT

        Task<Shift> CreateShift(Shift shift);

        IQueryable<Shift> GetIqueryableShift();

        #endregion

        #region SHIFT DETAILS

        Task<List<ShiftDetail>> CreateShiftDetails(List<ShiftDetail> shiftDetails);

        List<ShiftDetail> GetAllShiftDetails();

        List<ShiftDetail> GetShiftDetailsByRegionId(int regionId);

        IQueryable<ShiftDetail> GetShiftDetails();

        List<ShiftDetail> GetShiftDetails(List<int> shiftDetailIds);

        Task<List<ShiftDetail>> UpdateShiftDetails(List<ShiftDetail> shiftDetails);

        Task<ShiftDetail> UpdateShiftDetail(ShiftDetail shiftDetail);

        ShiftDetail GetShiftDetailByShiftDetailId(int shiftDetailId);

        #endregion

        #region SHIFT DETAIL REGIONS

        Task<List<ShiftDetailRegion>> CreateShiftDetailRegions(List<ShiftDetailRegion> shiftDetailRegions);

        #endregion
    }
}
