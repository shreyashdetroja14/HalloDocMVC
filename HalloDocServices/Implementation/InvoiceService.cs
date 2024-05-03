using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Implementation
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IShiftRepository shiftRepository, IInvoiceRepository invoiceRepository)
        {
            _shiftRepository = shiftRepository;
            _invoiceRepository = invoiceRepository;
        }

        public TimesheetViewModel GetTimesheetViewModelData(TimesheetViewModel TimesheetData)
        {
            DateTime startDateTime = DateTime.ParseExact(TimesheetData.TimesheetStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(TimesheetData.TimesheetEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            for (var i = startDateTime; i <= endDateTime; i = i.AddDays(1))
            {
                List<ShiftDetail> shiftDetails = _shiftRepository.GetShiftDetails().Where(x => (x.IsDeleted != true) && (DateOnly.FromDateTime(x.ShiftDate) == DateOnly.FromDateTime(i)) && (x.Shift.PhysicianId == TimesheetData.PhysicianId)).OrderBy(x => x.StartTime).ToList();

                int totalHours = 0;
                foreach(var item in shiftDetails)
                {
                    TimeSpan timeDifference = item.EndTime - item.StartTime;
                    totalHours += (int) Math.Ceiling(timeDifference.TotalHours);
                }

                TimesheetData.TimesheetDetails.Add(new TimesheetDetailViewModel
                {
                    TimesheetDetailDate = DateOnly.FromDateTime(i).ToString("yyyy-MM-dd"),
                    OnCallHours = totalHours,

                });
            }

            return TimesheetData;
        }

        public async Task<bool> AddOrEditTimesheet(TimesheetViewModel TimesheetData)
        {
            Timesheet timesheet = new Timesheet();
            if(TimesheetData.TimesheetId == 0)
            {
                timesheet.StartDate = DateTime.ParseExact(TimesheetData.TimesheetStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                timesheet.EndDate = DateTime.ParseExact(TimesheetData.TimesheetEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                timesheet.PhysicianId = TimesheetData.PhysicianId;
                timesheet.CreatedBy = TimesheetData.CreatedBy;
                timesheet.CreatedDate = DateTime.Now;

                timesheet = await _invoiceRepository.CreateTimesheet(timesheet);
            }
            else
            {
                timesheet = _invoiceRepository.GetTimesheetById(TimesheetData.TimesheetId);
            }

            List<TimesheetDetail> timesheetDetailsFetched = _invoiceRepository.GetTimesheetDetailsByTimesheetId(timesheet.TimesheetId);
            List<TimesheetDetail> timesheetDetailsToAdd = new List<TimesheetDetail>();
            List<TimesheetDetail> timesheetDetailsToUpdate = new List<TimesheetDetail>();
            foreach (var item in TimesheetData.TimesheetDetails)
            {
                TimesheetDetail timesheetDetail = new TimesheetDetail();
                if(item.TimesheetDetailId != 0)
                {
                    timesheetDetail = timesheetDetailsFetched.First(x => x.TimesheetDetailId == item.TimesheetDetailId);
                    timesheetDetail.TimesheetId = timesheet.TimesheetId;
                    timesheetDetail.TimesheetDetailDate = DateTime.ParseExact(item.TimesheetDetailDate ?? "", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    timesheetDetail.OnCallHours = item.OnCallHours;
                    timesheetDetail.TotalHours = item.TotalHours;
                    timesheetDetail.IsNightWeekend = item.IsNightWeekend;
                    timesheetDetail.HousecallsCount = item.HousecallsCount;
                    timesheetDetail.PhoneconsultCount = item.PhoneconsultCount;
                    timesheetDetail.ModifiedBy = TimesheetData.CreatedBy;
                    timesheetDetail.ModifiedDate = DateTime.Now;

                    timesheetDetailsToUpdate.Add(timesheetDetail);
                }
                else
                {
                    timesheetDetail.TimesheetId = timesheet.TimesheetId;
                    timesheetDetail.TimesheetDetailDate = DateTime.ParseExact(item.TimesheetDetailDate ?? "", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    timesheetDetail.OnCallHours = item.OnCallHours;
                    timesheetDetail.TotalHours = item.TotalHours;
                    timesheetDetail.IsNightWeekend = item.IsNightWeekend;
                    timesheetDetail.HousecallsCount = item.HousecallsCount;
                    timesheetDetail.PhoneconsultCount = item.PhoneconsultCount;
                    timesheetDetail.ModifiedBy = TimesheetData.CreatedBy;
                    timesheetDetail.ModifiedDate = DateTime.Now;

                    timesheetDetailsToAdd.Add(timesheetDetail);
                }
            }

            await _invoiceRepository.UpdateTimeshiftDetails(timesheetDetailsToUpdate);
            await _invoiceRepository.AddTimeshiftDetails(timesheetDetailsToAdd);

            return true;
        }
    }
}
