using HalloDocServices.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IRecordsService
    {
        PaginatedListViewModel<RecordRowViewModel> GetRecordsList(SearchRecordsViewModel SearchFilter);

        Task<bool> DeleteRecord(int requestId);

        byte[] ExportToExcel(SearchRecordsViewModel SearchFilter);

        PaginatedListViewModel<PatientRowViewModel> GetPatientList(SearchRecordsViewModel SearchFilter);

        PaginatedListViewModel<PatientRowViewModel> GetPatientRecordList(int userId, int pageNumber);

        PaginatedListViewModel<PatientRowViewModel> GetBlockedList(SearchRecordsViewModel SearchFilter);
    }
}
