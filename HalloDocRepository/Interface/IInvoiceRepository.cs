using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface IInvoiceRepository
    {
        #region PAYRATE

        List<Payrate> GetPayratesByPhysicianId(int physicianId);

        Payrate GetPayrateByPayrateId(int payrateId);

        Task<bool> UpdatePayrate(Payrate payrate);

        Task<bool> CreatePayrates(List<Payrate> payrates);

        #endregion

        #region TIMESHEET

        Task<Timesheet> CreateTimesheet(Timesheet timesheet);

        Timesheet GetTimesheetById(int timesheetId);

        #endregion

        #region TIMESHEET DETAIL

        List<TimesheetDetail> GetTimesheetDetailsByTimesheetId(int timesheetId);

        Task<List<TimesheetDetail>> UpdateTimeshiftDetails(List<TimesheetDetail> timesheetDetails);

        Task<List<TimesheetDetail>> AddTimeshiftDetails(List<TimesheetDetail> timesheetDetails);

        #endregion
    }
}
