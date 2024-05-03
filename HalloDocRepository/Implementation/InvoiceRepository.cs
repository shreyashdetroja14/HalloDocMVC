using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Implementation
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly HalloDocContext _context;

        public InvoiceRepository(HalloDocContext context)
        {
            _context = context;
        }

        public List<Payrate> GetPayratesByPhysicianId(int physicianId)
        {
            return _context.Payrates.Include(x => x.PayrateCategory).Where(x => x.PhysicianId == physicianId).OrderBy(x => x.PayrateCategoryId).ToList();
        }

        public Payrate GetPayrateByPayrateId(int payrateId)
        {
            return _context.Payrates.FirstOrDefault(x => x.PayrateId == payrateId) ?? new Payrate();
        }

        public async Task<bool> UpdatePayrate(Payrate payrate)
        {
            _context.Payrates.Update(payrate);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreatePayrates(List<Payrate> payrates)
        {
            _context.Payrates.AddRange(payrates);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Timesheet> CreateTimesheet(Timesheet timesheet)
        {
            _context.Timesheets.Add(timesheet);
            await _context.SaveChangesAsync();

            return timesheet;
        }

        public Timesheet GetTimesheetById(int timesheetId)
        {
            return _context.Timesheets.FirstOrDefault(x => x.TimesheetId == timesheetId) ?? new Timesheet();
        }

        public List<TimesheetDetail> GetTimesheetDetailsByTimesheetId(int timesheetId)
        {
            return _context.TimesheetDetails.Where(x => x.TimesheetId == timesheetId).ToList();
            
        }

        public async Task<List<TimesheetDetail>> UpdateTimeshiftDetails(List<TimesheetDetail> timesheetDetails)
        {
            _context.TimesheetDetails.UpdateRange(timesheetDetails);
            await _context.SaveChangesAsync();

            return timesheetDetails;
        }
        
        public async Task<List<TimesheetDetail>> AddTimeshiftDetails(List<TimesheetDetail> timesheetDetails)
        {
            _context.TimesheetDetails.AddRange(timesheetDetails);
            await _context.SaveChangesAsync();

            return timesheetDetails;
        }
    }
}
