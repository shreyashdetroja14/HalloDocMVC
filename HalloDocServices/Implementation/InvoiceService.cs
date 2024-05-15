using HalloDocEntities.Models;
using HalloDocRepository.Implementation;
using HalloDocRepository.Interface;
using HalloDocServices.Constants;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HalloDocServices.Implementation
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPhysicianRepository _physicianRepository;

        public InvoiceService(IShiftRepository shiftRepository, IInvoiceRepository invoiceRepository, IPhysicianRepository physicianRepository)
        {
            _shiftRepository = shiftRepository;
            _invoiceRepository = invoiceRepository;
            _physicianRepository = physicianRepository;
        }

        public string UploadFileToServer(IFormFile? UploadFile, int timesheetId)
        {
            if (UploadFile == null) { return string.Empty; }

            string FilePath = "wwwroot\\Upload\\Receipts\\" + timesheetId;
            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";

            string fileNameWithPath = Path.Combine(path, newfilename);
            string file = FilePath.Replace("wwwroot\\Upload\\Receipts\\", "/Upload/Receipts/") + "/" + newfilename;

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                UploadFile.CopyTo(stream);
            }

            return file;
        }

        public TimesheetViewModel GetTimesheetViewModelData(TimesheetViewModel TimesheetData)
        {
            DateTime startDateTime = DateTime.ParseExact(TimesheetData.TimesheetStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(TimesheetData.TimesheetEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            Timesheet? timesheet = _invoiceRepository.GetIQueryableTimesheets().Include(x => x.TimesheetDetails).Include(x => x.TimesheetReceipts).Where(x => (TimesheetData.PhysicianId == 0 || x.PhysicianId == TimesheetData.PhysicianId) && DateOnly.FromDateTime(x.StartDate ?? DateTime.Now) == DateOnly.FromDateTime(startDateTime)).FirstOrDefault();

            List<TimesheetDetail>? timesheetDetails = timesheet?.TimesheetDetails.OrderBy(x => x.TimesheetDetailDate).ToList();
            List<TimesheetReceipt>? timesheetReceipts = timesheet?.TimesheetReceipts.Where(x => x.IsDeleted != true).OrderBy(x => x.ReceiptDate).ToList();

            TimesheetData.TimesheetId = timesheet?.TimesheetId ?? 0;
            TimesheetData.IsFinalized = timesheet?.IsFinalized ?? false;
            TimesheetData.IsApproved = timesheet?.IsApproved ?? false;

            for (var i = startDateTime; i <= endDateTime; i = i.AddDays(1))
            {
                List<ShiftDetail> shiftDetails = _shiftRepository.GetShiftDetails().Where(x => (x.IsDeleted != true) && (DateOnly.FromDateTime(x.ShiftDate) == DateOnly.FromDateTime(i)) && (x.Shift.PhysicianId == TimesheetData.PhysicianId)).OrderBy(x => x.StartTime).ToList();

                int totalHours = 0;
                foreach (var item in shiftDetails)
                {
                    TimeSpan timeDifference = item.EndTime - item.StartTime;
                    totalHours += (int)Math.Ceiling(timeDifference.TotalHours);
                }

                TimesheetDetail? timesheetDetail = timesheetDetails?.FirstOrDefault(x => DateOnly.FromDateTime(x.TimesheetDetailDate ?? DateTime.Now) == DateOnly.FromDateTime(i));

                TimesheetData.TimesheetDetails.Add(new TimesheetDetailViewModel
                {
                    TimesheetDetailDate = DateOnly.FromDateTime(i).ToString("yyyy-MM-dd"),
                    OnCallHours = totalHours,
                    TimesheetDetailId = timesheetDetail?.TimesheetDetailId ?? 0,
                    TotalHours = timesheetDetail?.TotalHours ?? 0,
                    IsNightWeekend = timesheetDetail?.IsNightWeekend ?? false,
                    HousecallsCount = timesheetDetail?.HousecallsCount,
                    PhoneconsultCount = timesheetDetail?.PhoneconsultCount,

                });

                TimesheetReceipt? timesheetReceipt = timesheetReceipts?.FirstOrDefault(x => DateOnly.FromDateTime(x.ReceiptDate ?? DateTime.Now) == DateOnly.FromDateTime(i));
                string? filename = null;
                if (timesheetReceipt != null)
                {
                    string filenameFetched = Path.GetFileName(timesheetReceipt.FileName ?? "");
                    int startIndex = filenameFetched.LastIndexOf("-");
                    int endIndex = filenameFetched.LastIndexOf(".");
                    filename = filenameFetched.Remove(startIndex, endIndex - startIndex);
                }

                TimesheetData.TimesheetReceipts.Add(new TimesheetReceiptViewModel
                {
                    ReceiptId = timesheetReceipt?.ReceiptId ?? 0,
                    ReceiptDate = DateOnly.FromDateTime(i).ToString("yyyy-MM-dd"),
                    TimesheetId = TimesheetData.TimesheetId,
                    ItemName = timesheetReceipt?.ItemName,
                    Amount = timesheetReceipt?.Amount,
                    FileName = filename,
                    FilePath = timesheetReceipt?.FileName
                });
            }

            if(TimesheetData.IsFinalized == true && TimesheetData.AspNetUserRole == "admin")
            {
                var payrates = _invoiceRepository.GetPayratesByPhysicianId(TimesheetData.PhysicianId);

                TimesheetData.PayrateTotals.Add(
                    payrates.FirstOrDefault(x => x.PayrateCategoryId == (int)PayrateCategories.Shift)?.Payrate1 ?? 0, 
                    TimesheetData.TimesheetDetails.Sum(x => x.TotalHours)
                );
                TimesheetData.PayrateTotals.Add(
                    payrates.FirstOrDefault(x => x.PayrateCategoryId == (int)PayrateCategories.ShiftNightWeekend)?.Payrate1 ?? 0,
                    TimesheetData.TimesheetDetails.Where(x => x.IsNightWeekend).Count()
                );
                TimesheetData.PayrateTotals.Add(
                    payrates.FirstOrDefault(x => x.PayrateCategoryId == (int)PayrateCategories.Housecall)?.Payrate1 ?? 0,
                    TimesheetData.TimesheetDetails.Sum(x => x.HousecallsCount) ?? 0
                );
                TimesheetData.PayrateTotals.Add(
                    payrates.FirstOrDefault(x => x.PayrateCategoryId == (int)PayrateCategories.PhoneConsult)?.Payrate1 ?? 0,
                    TimesheetData.TimesheetDetails.Sum(x => x.PhoneconsultCount) ?? 0
                );

                //100 20 15 30
            }


            return TimesheetData;
        }

        public async Task<bool> AddOrEditTimesheet(TimesheetViewModel TimesheetData)
        {
            Timesheet timesheet = new Timesheet();
            if (TimesheetData.TimesheetId == 0)
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
                if (item.TimesheetDetailId != 0)
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

        public async Task<int> AddOrEditReceipt(TimesheetReceiptViewModel TimesheetReceipt)
        {
            if (TimesheetReceipt.FileToUpload == null) return 0;

            TimesheetReceipt timesheetReceipt = new();

            if (TimesheetReceipt.ReceiptId != 0)
            {
                timesheetReceipt = _invoiceRepository.GetTimesheetReceiptById(TimesheetReceipt.ReceiptId);
            }
            else
            {
                timesheetReceipt.TimesheetId = TimesheetReceipt.TimesheetId;
            }

            timesheetReceipt.ReceiptDate = DateTime.ParseExact(TimesheetReceipt.ReceiptDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            timesheetReceipt.ItemName = TimesheetReceipt.ItemName;
            timesheetReceipt.Amount = TimesheetReceipt.Amount;
            timesheetReceipt.FileName = UploadFileToServer(TimesheetReceipt.FileToUpload, TimesheetReceipt.TimesheetId);
            timesheetReceipt.CreatedDate = DateTime.Now;
            timesheetReceipt.AdminId = TimesheetReceipt.AdminId;
            timesheetReceipt.PhysicianId = TimesheetReceipt.PhysicianId;

            if (TimesheetReceipt.ReceiptId != 0)
            {
                await _invoiceRepository.UpdateTimesheetReceipt(timesheetReceipt);
            }
            else
            {
                await _invoiceRepository.CreateTimesheetReceipt(timesheetReceipt);
            }
            

            return timesheetReceipt.ReceiptId;
        }

        public async Task<bool> DeleteReceipt(int receiptId)
        {
            TimesheetReceipt timesheetReceipt = _invoiceRepository.GetTimesheetReceiptById(receiptId);
            if(timesheetReceipt.ReceiptId == 0)
            {
                return false;
            }

            timesheetReceipt.IsDeleted = true;

            await _invoiceRepository.UpdateTimesheetReceipt(timesheetReceipt);

            return true;
        }

        public async Task<bool> FinalizeTimesheet(int timesheetId)
        {
            Timesheet timesheet = _invoiceRepository.GetTimesheetById(timesheetId);
            if (timesheet.TimesheetId == 0) return false;

            timesheet.IsFinalized = true;

            await _invoiceRepository.UpdateTimesheet(timesheet);

            return true;
        }

        public bool CheckFinalizeStatus(TimesheetViewModel TimesheetData)
        {
            DateTime startDateTime = DateTime.ParseExact(TimesheetData.TimesheetStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(TimesheetData.TimesheetEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            Timesheet? timesheet = _invoiceRepository.GetIQueryableTimesheets().Where(x => DateOnly.FromDateTime(x.StartDate ?? DateTime.Now) == DateOnly.FromDateTime(startDateTime)).FirstOrDefault();

            return timesheet?.IsFinalized ?? false;
        }

        public List<SelectListItem> GetPhysicianList()
        {
            var physicians = _physicianRepository.GetAllPhysicians().Where(x => x.IsDeleted != true);

            List<SelectListItem> physicianList = new List<SelectListItem>();

            foreach (var physician in physicians)
            {
                physicianList.Add(new SelectListItem
                {
                    Value = physician.PhysicianId.ToString(),
                    Text = physician.FirstName + " " + physician.LastName,
                });
            }

            return physicianList;
        }

        public async Task<bool> ApproveTimesheet(TimesheetViewModel TimesheetData)
        {
            Timesheet timesheet = _invoiceRepository.GetTimesheetById(TimesheetData.TimesheetId);
            if (timesheet.TimesheetId == 0) return false;

            timesheet.IsApproved = true;
            timesheet.BonusAmount = TimesheetData.BonusAmount;
            timesheet.AdminDescription = TimesheetData.AdminDescription;

            await _invoiceRepository.UpdateTimesheet(timesheet);
            return true;
        }

        public bool CheckApprovedStatus(TimesheetViewModel TimesheetData)
        {
            DateTime startDateTime = DateTime.ParseExact(TimesheetData.TimesheetStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime endDateTime = DateTime.ParseExact(TimesheetData.TimesheetEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            Timesheet? timesheet = _invoiceRepository.GetIQueryableTimesheets().Where(x => DateOnly.FromDateTime(x.StartDate ?? DateTime.Now) == DateOnly.FromDateTime(startDateTime)).FirstOrDefault();

            return timesheet?.IsApproved ?? false;
        }
    }
}
