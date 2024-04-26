using ClosedXML.Excel;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Constants;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HalloDocServices.Implementation
{
    public class RecordsService : IRecordsService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotesAndLogsRepository _notesAndLogsRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IEmailSMSLogRepository _emailSMSLogRepository;

        public RecordsService(IRequestRepository requestRepository, IUserRepository userRepository, INotesAndLogsRepository notesAndLogsRepository, ICommonRepository commonRepository, IEmailSMSLogRepository emailSMSLogRepository)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
            _commonRepository = commonRepository;
            _emailSMSLogRepository = emailSMSLogRepository;
        }

        #region SEARCH RECORDS

        public IQueryable<Request> FilterRequests(SearchRecordsViewModel SearchFilter)
        {
            var rawData = _requestRepository.GetIQueryableRequests().Where(x => x.IsDeleted != true);

            if (SearchFilter.RequestStatus != null)
            {
                int[] statusArray = new CommonMethods().GetRequestStatus(SearchFilter.RequestStatus ?? 0);
                rawData = rawData.Where(x => statusArray.Contains(x.Status));
            }

            if (SearchFilter.PatientName != null)
            {
                rawData = rawData.Where(x => EF.Functions.ILike(x.RequestClients.FirstOrDefault().FirstName, "%" + SearchFilter.PatientName + "%"));
            }

            if (SearchFilter.RequestType != null)
            {
                rawData = rawData.Where(x => x.RequestTypeId == SearchFilter.RequestType);
            }

            if (SearchFilter.FromDateOfService != null)
            {
                DateTime fromDate = DateTime.ParseExact(SearchFilter.FromDateOfService, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                rawData = rawData.Where(x => DateOnly.FromDateTime(x.AcceptedDate ?? DateTime.Now) >= DateOnly.FromDateTime(fromDate));
            }

            if (SearchFilter.ToDateOfService != null)
            {
                DateTime toDate = DateTime.ParseExact(SearchFilter.ToDateOfService, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                rawData = rawData.Where(x => DateOnly.FromDateTime(x.AcceptedDate ?? DateTime.Now) <= DateOnly.FromDateTime(toDate));
            }

            if (SearchFilter.ProviderName != null)
            {
                rawData = rawData.Where(x => EF.Functions.ILike(x.Physician != null ? x.Physician.FirstName : "", "%" + SearchFilter.ProviderName + "%"));
            }

            if (SearchFilter.Email != null)
            {
                rawData = rawData.Where(x => x.RequestClients.FirstOrDefault() != null ? EF.Functions.ILike(x.RequestClients.FirstOrDefault().Email ?? "", "%" + SearchFilter.Email + "%") : false
                );
            }

            if (SearchFilter.PhoneNumber != null)
            {
                rawData = rawData.Where(x => x.RequestClients.FirstOrDefault() != null ? EF.Functions.ILike(x.RequestClients.FirstOrDefault().PhoneNumber, "%" + SearchFilter.PhoneNumber + "%") : false);
            }

            rawData = rawData.OrderByDescending(x => x.CreatedDate);

            return rawData;
        }
             
        public PaginatedListViewModel<RecordRowViewModel> GetRecordsList(SearchRecordsViewModel SearchFilter)
        {

            var rawData = FilterRequests(SearchFilter);

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

            PaginatedListViewModel<RecordRowViewModel> PaginatedList = new PaginatedListViewModel<RecordRowViewModel>();
            PaginatedList.PagerData = PagerData;
            PaginatedList.DataRows = RecordsList;

            return PaginatedList;
        }

        public byte[] ExportToExcel(SearchRecordsViewModel SearchFilter)
        {
            var rawData = FilterRequests(SearchFilter);

            var data = rawData.Select(request => new
            {
                request,
                requestClient = request.RequestClients.FirstOrDefault(),
                physician = request.Physician,
                closeCaseLog = request.RequestStatusLogs.FirstOrDefault(x => x.Status == (int)RequestStatus.Unpaid),
                requestNote = request.RequestNotes.FirstOrDefault(),
                cancelledLog = request.RequestStatusLogs.FirstOrDefault(x => x.Status == (int)RequestStatus.Cancelled),
            }).ToList();

            DataTable dataTable = new DataTable("Requests");

            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("RequestId"),
                new DataColumn("Patient Name"),
                new DataColumn("Date of Service"),
                new DataColumn("Close Case Date"),
                new DataColumn("Email"),
                new DataColumn("Phone Number"),
                new DataColumn("Address"),
                new DataColumn("Zipcode"),
                new DataColumn("Request Status"),
                new DataColumn("Physician"),
                new DataColumn("Physician Note"),
                new DataColumn("Physician Cancellation Note"),
                new DataColumn("Admin Note"),
                new DataColumn("Patient Note"),

            });

            foreach(var item in data)
            {
                dataTable.Rows.Add(
                    item.request.RequestId,
                    item.requestClient?.FirstName + item.requestClient?.LastName,
                    item.request.AcceptedDate,
                    item.closeCaseLog?.CreatedDate,
                    item.requestClient?.Email,
                    item.requestClient?.PhoneNumber,
                    item.requestClient?.Address,
                    item.requestClient?.ZipCode,
                    ((RequestStatus)item.request.Status).ToString(),
                    item.physician?.FirstName + " " + item.physician?.LastName,
                    item.cancelledLog?.PhysicianId != null ? item.cancelledLog.Notes?.Substring(item.cancelledLog.Notes.LastIndexOf(':')) : "",
                    item.requestNote?.AdminNotes,
                    item.requestClient?.Notes
                );
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    return stream.ToArray();
                }
            }
        }

        public async Task<bool> DeleteRecord(int requestId, int adminId)
        {
            var request = await _requestRepository.GetRequestByRequestId(requestId);
            if(request == null) return false;

            request.IsDeleted = true;
            await _requestRepository.UpdateRequest(request);

            RequestStatusLog log = new RequestStatusLog();
            log.RequestId = request.RequestId;
            log.Status = request.Status;
            log.AdminId = adminId;
            log.Notes = "Admin deleted the request on " + DateOnly.FromDateTime(DateTime.Now) + " at " + DateTime.Now.ToLongTimeString();
            log.CreatedDate = DateTime.Now;

            await _notesAndLogsRepository.AddRequestStatusLog(log);

            return true;
        }

        #endregion

        #region PATIENT HISTORY

        public PaginatedListViewModel<PatientRowViewModel> GetPatientList(SearchRecordsViewModel SearchFilter)
        {
            var data = _userRepository.GetIQueryableUsers();

            if (SearchFilter.FirstName != null)
            {
                data = data.Where(x => EF.Functions.ILike(x.FirstName, "%" + SearchFilter.FirstName + "%"));
            }
            
            if (SearchFilter.LastName != null)
            {
                data = data.Where(x => EF.Functions.ILike(x.LastName ?? "", "%" + SearchFilter.LastName + "%"));
            }

            if (SearchFilter.Email != null)
            {
                data = data.Where(x => EF.Functions.ILike(x.Email, "%" + SearchFilter.Email + "%"));
            }

            if (SearchFilter.PhoneNumber != null)
            {
                data = data.Where(x => EF.Functions.ILike(x.Mobile ?? "", "%" + SearchFilter.PhoneNumber + "%"));
            }

            data = data.OrderByDescending(x => x.CreatedDate);

            //pagination data initialize

            int requestCount = data.Count();
            int pageNumber = SearchFilter.PageNumber;
            int pageSize = 5;

            PagerViewModel PagerData = new PagerViewModel(requestCount, pageNumber, pageSize);

            //pagination query for db 

            int rowsToSkip = (pageNumber - 1) * pageSize;

            data = data.Skip(rowsToSkip).Take(pageSize);

            List<PatientRowViewModel> PatientList = data.Select(x => new PatientRowViewModel
            {
                UserId = x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.Mobile,
                Address = x.Street + ", " + x.City + ", " + x.State + ", " + x.ZipCode,

            }).ToList();

            PatientList = PatientList.OrderBy(x => x.UserId).ToList();

            PaginatedListViewModel<PatientRowViewModel> PaginatedList = new PaginatedListViewModel<PatientRowViewModel>();
            PaginatedList.PagerData = PagerData;
            PaginatedList.DataRows = PatientList;

            return PaginatedList;
        }

        public PaginatedListViewModel<PatientRowViewModel> GetPatientRecordList(int userId, int pageNumber)
        {
            var rawData = _requestRepository.GetIQueryableRequests().Where(x => x.UserId  == userId);

            rawData = rawData.OrderByDescending(x => x.CreatedDate);

            int requestCount = rawData.Count();
            int pageSize = 5;

            PagerViewModel PagerData = new PagerViewModel(requestCount, pageNumber, pageSize);

            //pagination query for db 

            int rowsToSkip = (pageNumber - 1) * pageSize;

            rawData = rawData.Skip(rowsToSkip).Take(pageSize);

            var requests = rawData.Select(x => new
            {
                request = x,
                requestClient = x.RequestClients.FirstOrDefault(),
                physician = x.Physician,
                encounterFormId = x.EncounterForms.FirstOrDefault() != null ? x.EncounterForms.FirstOrDefault().EncounterFormId : 0,

            }).ToList();

            List<PatientRowViewModel> PatientRecordList = new List<PatientRowViewModel>();

            foreach (var item in requests)
            {
                PatientRecordList.Add(new PatientRowViewModel
                {
                    UserId = item.request.UserId ?? 0,
                    RequestId = item.request.RequestId,
                    EncounterFormId = item.encounterFormId,
                    FirstName = item.requestClient?.FirstName,
                    LastName = item.requestClient?.LastName,
                    Email = item.requestClient?.Email,
                    PhoneNumber = item.requestClient?.PhoneNumber,
                    Address = item.requestClient?.Address,
                    CreatedDate = DateOnly.FromDateTime(item.request.CreatedDate).ToString("MMMM dd, yyyy"),
                    ConfirmationNumber = item.request.ConfirmationNumber,
                    ProviderName = item.physician?.FirstName + " " + item.physician?.LastName,
                    //concluded date
                    ConcludedDate = DateOnly.FromDateTime(DateTime.Now).ToString("MMMM dd, yyyy"),
                    Status = ((RequestStatus)item.request.Status).ToString(),

                });
            }

            PaginatedListViewModel<PatientRowViewModel> PaginatedList = new PaginatedListViewModel<PatientRowViewModel>();
            PaginatedList.PagerData = PagerData;
            PaginatedList.DataRows = PatientRecordList;

            return PaginatedList;
        }

        #endregion

        #region BLOCKED HISTORY

        public PaginatedListViewModel<PatientRowViewModel> GetBlockedList(SearchRecordsViewModel SearchFilter)
        {
            var data = _requestRepository.GetIQueryableBlockedRequests().Where(x => x.IsActive == true);

            if (SearchFilter.PatientName != null)
            {
                data = data.Where(x => 
                EF.Functions.ILike(x.Request.RequestClients.FirstOrDefault().FirstName, "%" + SearchFilter.PatientName + "%") ||
                EF.Functions.ILike(x.Request.RequestClients.FirstOrDefault().LastName?? "", "%" + SearchFilter.PatientName + "%"));
            }

            if (SearchFilter.CreatedDate != null)
            {
                DateTime createdDate = DateTime.ParseExact(SearchFilter.CreatedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                data = data.Where(x => DateOnly.FromDateTime(x.CreatedDate ?? DateTime.Now) == DateOnly.FromDateTime(createdDate));
            }

            if (SearchFilter.Email != null)
            {
                data = data.Where(x => EF.Functions.ILike(x.Email, "%" + SearchFilter.Email + "%"));
            }

            if (SearchFilter.PhoneNumber != null)
            {
                data = data.Where(x => EF.Functions.ILike(x.PhoneNumber ?? "", "%" + SearchFilter.PhoneNumber + "%"));
            }

            data = data.OrderByDescending(x => x.CreatedDate);

            //pagination data initialize

            int requestCount = data.Count();
            int pageNumber = SearchFilter.PageNumber;
            int pageSize = 5;

            PagerViewModel PagerData = new PagerViewModel(requestCount, pageNumber, pageSize);

            //pagination query for db 

            int rowsToSkip = (pageNumber - 1) * pageSize;

            data = data.Skip(rowsToSkip).Take(pageSize);

            var blockedData = data.Select(x => new
            {
                blockRequest = x,
                firstName = x.Request.RequestClients.FirstOrDefault().FirstName,
                lastName = x.Request.RequestClients.FirstOrDefault().LastName

            }).ToList();

            List<PatientRowViewModel> BlockedList = new List<PatientRowViewModel>();

            foreach (var row in blockedData)
            {
                BlockedList.Add(new PatientRowViewModel
                {
                    BlockedRequestId = row.blockRequest.BlockRequestId,
                    FirstName =  row.firstName,
                    LastName = row.lastName,
                    PhoneNumber = row.blockRequest.PhoneNumber,
                    Email = row.blockRequest.Email,
                    CreatedDate = DateOnly.FromDateTime(row.blockRequest.CreatedDate ?? DateTime.Now).ToString("MMMM dd, yyyy"),
                    BlockedReason = row.blockRequest.Reason,
                    IsActive = row.blockRequest.IsActive
                });
            }

            BlockedList = BlockedList.OrderBy(x => x.UserId).ToList();

            PaginatedListViewModel<PatientRowViewModel> PaginatedList = new PaginatedListViewModel<PatientRowViewModel>();
            PaginatedList.PagerData = PagerData;
            PaginatedList.DataRows = BlockedList;

            return PaginatedList;
        }

        public async Task<bool> UnblockRequest(int blockRequestId, int adminId)
        {
            BlockRequest blockRequest = _requestRepository.GetBlockRequestById(blockRequestId);
            Request request = await _requestRepository.GetRequestByRequestId(blockRequest.RequestId);
            
            if(blockRequest.BlockRequestId == 0 || request.RequestId == 0)
            {
                return false;
            }

            blockRequest.IsActive = false;
            await _requestRepository.UpdateBlockRequest(blockRequest);

            request.Status = (int)RequestStatus.Unassigned;
            await _requestRepository.UpdateRequest(request);

            RequestStatusLog log = new RequestStatusLog();
            log.RequestId = request.RequestId;
            log.Status = request.Status;
            log.AdminId = adminId;
            log.Notes = "Admin unblocked the case on " + DateOnly.FromDateTime(DateTime.Now) + " at " + DateTime.Now.ToLongTimeString();
            log.CreatedDate = DateTime.Now;

            await _notesAndLogsRepository.AddRequestStatusLog(log);

            return true;
        }

        #endregion

        #region EMAIL-SMS LOGS

        public PaginatedListViewModel<LogRowViewModel> GetEmailLogs(SearchRecordsViewModel SearchFilter)
        {
            var emailLogs = _emailSMSLogRepository.GetIqueryableEmailLogs();

            if(SearchFilter.AccountType != null)
            {
                emailLogs = emailLogs.Where(x => x.RoleId == SearchFilter.AccountType);
            }

            if(SearchFilter.ReceiverName != null)
            {
                emailLogs = emailLogs.Where(x => EF.Functions.ILike(x.RecipientName, "%" + SearchFilter.ReceiverName + "%"));
            }

            if(SearchFilter.Email != null)
            {
                emailLogs = emailLogs.Where(x => EF.Functions.ILike(x.EmailId, "%" + SearchFilter.Email + "%"));
            }

            if(SearchFilter.CreatedDate != null)
            {
                DateTime createdDate = DateTime.ParseExact(SearchFilter.CreatedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                emailLogs = emailLogs.Where(x => DateOnly.FromDateTime(x.CreatedDate) == DateOnly.FromDateTime(createdDate));
            }
            
            if(SearchFilter.SentDate != null)
            {
                DateTime sentDate = DateTime.ParseExact(SearchFilter.SentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                emailLogs = emailLogs.Where(x => DateOnly.FromDateTime(x.SentDate ?? DateTime.Now) == DateOnly.FromDateTime(sentDate));
            }

            emailLogs = emailLogs.OrderByDescending(x => x.CreatedDate);

            //pagination data initialize

            int requestCount = emailLogs.Count();
            int pageNumber = SearchFilter.PageNumber;
            int pageSize = 5;

            PagerViewModel PagerData = new PagerViewModel(requestCount, pageNumber, pageSize);

            //pagination query for db 

            int rowsToSkip = (pageNumber - 1) * pageSize;

            emailLogs = emailLogs.Skip(rowsToSkip).Take(pageSize);

            List<LogRowViewModel> EmailLogList = emailLogs.Select(x => new LogRowViewModel
            {
                EmailLogId = x.EmailLogId,
                RecipientName = x.RecipientName,
                Action = ((ActionEnum)(x.Action ?? 0)).ToString(),
                RoleName = ((AccountType)(x.RoleId ?? 0)).ToString(),
                Email = x.EmailId,
                CreatedDate = x.CreatedDate.ToString(),
                SentDate = x.SentDate != null ? x.SentDate.ToString() : null,
                IsSent = x.IsEmailSent ?? false,
                SentTries = x.SentTries,
                ConfirmationNumber = x.Request != null ? x.Request.ConfirmationNumber : "",
                IsEmailLog = true,
                IsSMSLog = false,
            }).ToList();



            PaginatedListViewModel<LogRowViewModel> PaginatedList = new PaginatedListViewModel<LogRowViewModel>();
            PaginatedList.PagerData = PagerData;
            PaginatedList.DataRows = EmailLogList;

            return PaginatedList;
        }

        public PaginatedListViewModel<LogRowViewModel> GetSMSLogs(SearchRecordsViewModel SearchFilter)
        {
            var smsLogs = _emailSMSLogRepository.GetIqueryableSmsLogs();

            if (SearchFilter.AccountType != null)
            {
                smsLogs = smsLogs.Where(x => x.RoleId == SearchFilter.AccountType);
            }

            if (SearchFilter.ReceiverName != null)
            {
                smsLogs = smsLogs.Where(x => EF.Functions.ILike(x.RecipientName ?? "", "%" + SearchFilter.ReceiverName + "%"));
            }

            if (SearchFilter.PhoneNumber != null)
            {
                smsLogs = smsLogs.Where(x => EF.Functions.ILike(x.MobileNumber, "%" + SearchFilter.PhoneNumber + "%"));
            }

            if (SearchFilter.CreatedDate != null)
            {
                DateTime createdDate = DateTime.ParseExact(SearchFilter.CreatedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                smsLogs = smsLogs.Where(x => DateOnly.FromDateTime(x.CreatedDate) == DateOnly.FromDateTime(createdDate));
            }

            if (SearchFilter.SentDate != null)
            {
                DateTime sentDate = DateTime.ParseExact(SearchFilter.SentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                smsLogs = smsLogs.Where(x => DateOnly.FromDateTime(x.SentDate ?? DateTime.Now) == DateOnly.FromDateTime(sentDate));
            }

            smsLogs = smsLogs.OrderByDescending(x => x.CreatedDate);

            //pagination data initialize

            int requestCount = smsLogs.Count();
            int pageNumber = SearchFilter.PageNumber;
            int pageSize = 5;

            PagerViewModel PagerData = new PagerViewModel(requestCount, pageNumber, pageSize);

            //pagination query for db 

            int rowsToSkip = (pageNumber - 1) * pageSize;

            smsLogs = smsLogs.Skip(rowsToSkip).Take(pageSize);

            List<LogRowViewModel> EmailLogList = smsLogs.Select(x => new LogRowViewModel
            {
                SMSLogId = x.SmsLogId,
                RecipientName = x.RecipientName,
                Action = ((ActionEnum)(x.Action ?? 0)).ToString(),
                RoleName = ((AccountType)(x.RoleId ?? 0)).ToString(),
                PhoneNumber = x.MobileNumber,
                CreatedDate = x.CreatedDate.ToString(),
                SentDate = x.SentDate != null ? x.SentDate.ToString() : null,
                IsSent = x.IsSmsSent ?? false,
                SentTries = x.SentTries,
                ConfirmationNumber = x.Request != null ? x.Request.ConfirmationNumber : "",
                IsEmailLog = false,
                IsSMSLog = true,
            }).ToList();



            PaginatedListViewModel<LogRowViewModel> PaginatedList = new PaginatedListViewModel<LogRowViewModel>();
            PaginatedList.PagerData = PagerData;
            PaginatedList.DataRows = EmailLogList;

            return PaginatedList;
        }

        #endregion
    }
}
