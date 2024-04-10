using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Constants;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HalloDocServices.Implementation
{
    public class RecordsService : IRecordsService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly INotesAndLogsRepository _notesAndLogsRepository;
        private readonly ICommonRepository _commonRepository;

        public RecordsService(IRequestRepository requestRepository, INotesAndLogsRepository notesAndLogsRepository, ICommonRepository commonRepository)
        {
            _requestRepository = requestRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
            _commonRepository = commonRepository;
        }

        public PaginatedListViewModel<RecordRowViewModel> GetRecordsList(SearchRecordsViewModel SearchFilter)
        {

            var rawData = _requestRepository.GetIQueryableRequests();

            if(SearchFilter.RequestStatus != null)
            {
                int[] statusArray = new CommonMethods().GetRequestStatus(SearchFilter.RequestStatus ?? 0);
                rawData = rawData.Where(x => statusArray.Contains(x.Status));
            }

            if(SearchFilter.PatientName != null)
            {
                rawData = rawData.Where(x => EF.Functions.ILike(x.RequestClients.FirstOrDefault().FirstName, "%" + SearchFilter.PatientName + "%"));
            }

            if(SearchFilter.RequestType  != null)
            {
                rawData = rawData.Where(x => x.RequestTypeId == SearchFilter.RequestType);
            }

            if(SearchFilter.FromDateOfService != null)
            {
                DateTime fromDate = DateTime.ParseExact(SearchFilter.FromDateOfService, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                rawData = rawData.Where(x => DateOnly.FromDateTime(x.AcceptedDate ?? DateTime.Now) >= DateOnly.FromDateTime(fromDate));
            }

            if (SearchFilter.ToDateOfService != null)
            {
                DateTime toDate = DateTime.ParseExact(SearchFilter.ToDateOfService, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                rawData = rawData.Where(x => DateOnly.FromDateTime(x.AcceptedDate ?? DateTime.Now) <= DateOnly.FromDateTime(toDate));
            }

            if(SearchFilter.ProviderName != null)
            {
                rawData = rawData.Where(x => EF.Functions.ILike(x.Physician != null ? x.Physician.FirstName : "", "%" + SearchFilter.ProviderName + "%"));
            }

            if (SearchFilter.Email != null)
            {
                rawData = rawData.Where(x => EF.Functions.ILike(x.RequestClients.FirstOrDefault().Email, "%" + SearchFilter.Email + "%"));
            }
            
            if (SearchFilter.Email != null)
            {
                rawData = rawData.Where(x => EF.Functions.ILike(x.RequestClients.FirstOrDefault().PhoneNumber, "%" + SearchFilter.PhoneNumber + "%"));
            }

            //pagination data initialize

            int requestCount = rawData.Count();
            int pageNumber = SearchFilter.PageNumber;
            int pageSize = 5;

            PagerViewModel PagerData = new PagerViewModel(requestCount, pageNumber, pageSize);

            //pagination query for db 

            int rowsToSkip = (pageNumber - 1) * pageSize;

            rawData = rawData.Skip(rowsToSkip).Take(pageSize);

            //select query on query with filters

            var data = rawData.Select(request => new
            {
                request,
                requestClient = request.RequestClients.FirstOrDefault(),
                physician = request.Physician,
                closeCaseLog = request.RequestStatusLogs.FirstOrDefault(x => x.Status == (int)RequestStatus.Unpaid),
                requestNote = request.RequestNotes.FirstOrDefault(),
                cancelledLog = request.RequestStatusLogs.FirstOrDefault(x => x.Status == (int)RequestStatus.Cancelled),
            }).ToList();

            
            //fill data in viewmodel for recordlist

            List<RecordRowViewModel> RecordsList = new List<RecordRowViewModel>();

            foreach (var item in data)
            {
                RecordsList.Add(new RecordRowViewModel
                {
                    RequestId = item.request.RequestId,
                    PatientName = item.requestClient?.FirstName + " " + item.requestClient?.LastName,
                    Requestor = ((RequestTypes)item.request.RequestTypeId).ToString(),
                    DateOfService = DateOnly.FromDateTime(item.request.AcceptedDate ?? DateTime.Now).ToString("MMMM dd, yyyy"),
                    CloseCaseDate = DateOnly.FromDateTime(item.closeCaseLog?.CreatedDate ?? DateTime.Now).ToString("MMMM dd, yyyy"),
                    Email = item.requestClient?.Email ?? "-",
                    PhoneNumber = item.requestClient?.PhoneNumber ?? "-",
                    Address = item.requestClient?.Address ?? "-",
                    ZipCode = item.requestClient?.ZipCode ?? "-",
                    //map request status to dashboard request status
                    RequestStatus = new CommonMethods().GetDashboardStatus(item.request.Status),
                    PhysicianName = item.physician?.FirstName + " " + item.physician?.LastName,
                    PhysicianNote = item.requestNote?.PhysicianNotes ?? "-",
                    //if phy id not null then cancelled by physician. get string after last ':' 
                    CancelledByProviderNote = item.cancelledLog?.PhysicianId != null ? item.cancelledLog.Notes?.Substring(item.cancelledLog.Notes.LastIndexOf(':')) : "-",
                    AdminNote = item.requestNote?.AdminNotes ?? "-",
                    PatientNote = item.requestClient?.Notes ?? "-",
                    CancellationReason = item.request.CaseTag ?? "-",
                });
            }

            RecordsList = RecordsList.OrderBy(x => x.RequestId).ToList();

            PaginatedListViewModel<RecordRowViewModel> PaginatedList = new PaginatedListViewModel<RecordRowViewModel>();
            PaginatedList.PagerData = PagerData;
            PaginatedList.DataRows = RecordsList;

            return PaginatedList;
        }
    }
}
