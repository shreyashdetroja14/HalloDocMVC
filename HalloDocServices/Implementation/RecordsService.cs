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

        public List<RecordRowViewModel> GetRecordsList(SearchRecordsViewModel SearchFilter)
        {
            //var requests = _requestRepository.GetIQueryableRequests();

            //List<RecordRowViewModel> RecordsList = new List<RecordRowViewModel>();

            /*foreach (var request in requests)
            {
                RequestClient? requestClient = request.RequestClients.FirstOrDefault();
                RequestStatusLog? closeCaseLog = request.RequestStatusLogs.FirstOrDefault(x => x.Status == (int)RequestStatus.Closed);
                RequestNote? requestNote = request.RequestNotes.FirstOrDefault();
                RequestStatusLog? cancelledLog = request.RequestStatusLogs.FirstOrDefault(x => x.Status == (int)RequestStatus.Cancelled);

                RecordsList.Add(new RecordRowViewModel
                {
                    RequestId = request.RequestId,
                    PatientName = requestClient?.FirstName + " " + requestClient?.LastName,
                    DateOfService = DateOnly.FromDateTime(request.AcceptedDate ?? DateTime.Now).ToString("MM dd, yyyy"),
                    CloseCaseDate = DateOnly.FromDateTime(closeCaseLog?.CreatedDate ?? DateTime.Now).ToString("MM dd, yyyy"),
                    Email = requestClient?.Email,
                    PhoneNumber = requestClient?.PhoneNumber,
                    Address = requestClient?.Address,
                    ZipCode = requestClient?.ZipCode,
                    //map request status to dashboard request status
                    RequestStatus = new CommonMethods().GetDashboardStatus(request.Status),
                    PhysicianName = request.Physician?.FirstName + " " + request.Physician?.LastName,
                    PhysicianNote = requestNote?.PhysicianNotes,
                    //if phy id not null then cancelled by physician. get string after last ':' 
                    CancelledByProviderNote = cancelledLog?.PhysicianId != null ? cancelledLog.Notes?.Substring(cancelledLog.Notes.LastIndexOf(':')) : null,
                    AdminNote = requestNote?.AdminNotes,
                    PatientNote = requestClient?.Notes,
                    CancellationReason = request.CaseTag,

                });
            }*/

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

            var data = rawData.Select(request => new
            {
                request,
                requestClient = request.RequestClients.FirstOrDefault(),
                physician = request.Physician,
                closeCaseLog = request.RequestStatusLogs.FirstOrDefault(x => x.Status == (int)RequestStatus.Unpaid),
                requestNote = request.RequestNotes.FirstOrDefault(),
                cancelledLog = request.RequestStatusLogs.FirstOrDefault(x => x.Status == (int)RequestStatus.Cancelled),
            }).ToList();

            

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
            
            return RecordsList;
        }
    }
}
