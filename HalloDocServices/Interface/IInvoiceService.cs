using HalloDocServices.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IInvoiceService
    {
        TimesheetViewModel GetTimesheetViewModelData(TimesheetViewModel TimesheetData);

        Task<bool> AddOrEditTimesheet(TimesheetViewModel TimesheetData);
    }
}
