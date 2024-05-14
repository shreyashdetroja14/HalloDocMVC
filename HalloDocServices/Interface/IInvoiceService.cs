using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IInvoiceService
    {
        //List<TimesheetDetailViewModel> GetTimesheetDetailsViewModelData(DateTime startDate, DateTime endDate);

        TimesheetViewModel GetTimesheetViewModelData(TimesheetViewModel TimesheetData);

        Task<bool> AddOrEditTimesheet(TimesheetViewModel TimesheetData);

        Task<int> AddOrEditReceipt(TimesheetReceiptViewModel TimesheetReceipt);

        Task<bool> DeleteReceipt(int receiptId);

        Task<bool> FinalizeTimesheet(int timesheetId);

        bool CheckFinalizeStatus(TimesheetViewModel TimesheetData);

        List<SelectListItem> GetPhysicianList();
    }
}
