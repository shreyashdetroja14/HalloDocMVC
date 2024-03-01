﻿using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;

namespace HalloDocServices.Implementation
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly INotesAndLogsRepository _notesAndLogsRepository;
        private readonly ICommonRepository _commonRepository;

        public AdminDashboardService(IUserRepository userRepository, IRequestRepository requestRepository, IPhysicianRepository physicianRepository, INotesAndLogsRepository notesAndLogsRepository, ICommonRepository commonRepository)
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _physicianRepository = physicianRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
            _commonRepository = commonRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
         }

        public async Task<AdminDashboardViewModel> GetViewModelData(int requestStatus)
        {
            AdminDashboardViewModel viewModel = new AdminDashboardViewModel();
            viewModel.RequestStatus = requestStatus;
            viewModel.NewRequestCount = await _requestRepository.GetNewRequestCount();
            viewModel.PendingRequestCount = await _requestRepository.GetPendingRequestCount();
            viewModel.ActiveRequestCount = await _requestRepository.GetActiveRequestCount();
            viewModel.ConcludeRequestCount = await _requestRepository.GetConcludeRequestCount();
            viewModel.ToCloseRequestCount = await _requestRepository.GetToCloseRequestCount();
            viewModel.UnpaidRequestCount = await _requestRepository.GetUnpaidRequestCount();
            return viewModel;
        }

        public List<RequestRowViewModel> GetViewModelData(int requestStatus, int? requestType, string? searchPattern, int? searchRegion)
        {
            List<RequestRowViewModel> requestRows = new List<RequestRowViewModel>();
            var requests = _requestRepository.GetAllIEnumerableRequests().AsQueryable();

            

            int[] myarray = new int[3];
            switch (requestStatus)
            {
                case 1:
                    myarray[0] = 1;
                    break;

                case 2:
                    myarray[0] = 2;
                    break;

                case 3:
                    myarray[0] = 4;
                    myarray[1] = 5;
                    break;

                case 4:
                    myarray[0] = 6;
                    break;

                case 5:
                    myarray[0] = 3;
                    myarray[1] = 7;
                    myarray[2] = 8;
                    break;

                case 6:
                    myarray[0] = 9;
                    break;
            }

            requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Include(x => x.RequestStatusLogs).Where(x => myarray.Contains(x.Status));

            /*switch(requestStatus)
            {
                case 1: 
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 1);
                    break;

                case 2:
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 2);
                    break;

                case 3:
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 4 || x.Status == 5);
                    break;

                case 4:
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 6);
                    break;

                case 5:
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 3 || x.Status == 7 || x.Status == 8);
                    break;

                case 6:
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 9);
                    break;
            }*/

            if (requestType != null)
            {
                requests = requests.Where(x => x.RequestTypeId == requestType);
            }

            

            if(searchPattern != null)
            {
                /*x.RequestClients.FirstOrDefault() != null ? (x.RequestClients.FirstOrDefault().FirstName != null ? x.RequestClients.FirstOrDefault().FirstName : "") : ""*/
                //requests = requests.Where(x => x.FirstName.Contains(searchPattern));
                requests = requests.Where(x => EF.Functions.Like(x.RequestClients.FirstOrDefault().FirstName, "%" + searchPattern + "%"));
            }

            if(searchRegion != null)
            {
                requests = requests.Where(x => x.RequestClients.FirstOrDefault().RegionId == searchRegion);
            }

            foreach(var request in requests)
            {
                RequestClient? requestClient = request.RequestClients.FirstOrDefault();
                int date = requestClient?.IntDate??0;
                int year = requestClient?.IntYear ?? 0;
                string month = requestClient?.StrMonth ?? "";

                List<string> notes = new List<string>();
                foreach(var log in request.RequestStatusLogs)
                {
                    if(log.Notes != null)
                    {
                        notes.Add(log.Notes);
                    }
                }

                requestRows.Add(new()
                {
                    DashboardRequestStatus = requestStatus,
                    RequestStatus = request.Status,
                    RequestId = request.RequestId,
                    RequestType = request.RequestTypeId,
                    PatientFullName = requestClient?.FirstName + " " + requestClient?.LastName,
                    PatientEmail = requestClient?.Email,
                    DateOfBirth = month + " " + date + "," + year,
                    RequestorName = request.FirstName + " " + request.LastName,
                    PhysicianName = request.Physician?.FirstName + " " + request.Physician?.LastName,
                    //DateOfService
                    RequestedDate = request.CreatedDate.ToLongDateString(),
                    PatientPhoneNumber = requestClient?.PhoneNumber,
                    SecondPhoneNumber = request.PhoneNumber,
                    Address = requestClient?.Address,
                    Region = requestClient?.RegionId,
                    //Notes
                    Notes = notes,

                }) ;
            }

            return requestRows;
        }

        public ViewCaseViewModel GetViewCaseViewModelData(int requestId)
        {
            ViewCaseViewModel CaseInfo = new ViewCaseViewModel();

            var requestFetched = _requestRepository.GetIQueryableRequestByRequestId(requestId);

            var request = requestFetched.Include(x => x.RequestClients).Include(x => x.RequestBusinesses).Include(x => x.RequestBusinesses).ThenInclude(x => x.Business).FirstOrDefault();

            var requestClient = request?.RequestClients.FirstOrDefault();
            var businessName = request?.RequestBusinesses.FirstOrDefault()?.Business.Name;
            //var request = await _requestRepository.GetRequestByRequestId(requestId);
            //var requestClient = await _requestRepository.GetRequestClientByRequestId(requestId);

            CaseInfo.RequestId = request?.RequestId;
            CaseInfo.RequestType = request?.RequestTypeId;
            CaseInfo.ConfirmationNumber = request?.ConfirmationNumber;
            
            if(requestClient != null)
            {
                CaseInfo.Symptoms = requestClient?.Notes;
                CaseInfo.FirstName = requestClient?.FirstName;
                CaseInfo.LastName = requestClient?.LastName;

                if (requestClient.IntDate.HasValue && requestClient.IntYear.HasValue && requestClient.StrMonth != null)
                {
                    DateTime monthDateTime = DateTime.ParseExact(requestClient.StrMonth, "MMMM", CultureInfo.InvariantCulture);
                    int month = monthDateTime.Month;
                    DateOnly date = new DateOnly((int)requestClient.IntYear, month, requestClient.IntDate.Value);
                    CaseInfo.DOB = date.ToString("yyyy-MM-dd");
                }
                CaseInfo.Email = requestClient?.Email;
                CaseInfo.PhoneNumber = requestClient?.PhoneNumber;
                CaseInfo.Region = requestClient?.State;
                if (businessName != null)
                {
                    CaseInfo.BusinessNameOrAddress = businessName;
                }
                else
                {
                    CaseInfo.BusinessNameOrAddress = requestClient?.Address;
                }
                CaseInfo.Room = requestClient?.Location;
            }

            return CaseInfo;
        }

        public async Task<bool> UpdateViewCaseInfo(ViewCaseViewModel CaseInfo)
        {
            var requestClient = await _requestRepository.GetRequestClientByRequestId(CaseInfo.RequestId ?? 0);
            if(requestClient == null) 
            {
                return false;
            }
            else
            {
                requestClient.Email = CaseInfo.Email;
                requestClient.PhoneNumber = CaseInfo.PhoneNumber;
                requestClient.NotiMobile = CaseInfo.PhoneNumber;

                await _requestRepository.UpdateRequestClient(requestClient);

                return true;
            }
        }

        public async Task<ViewNotesViewModel> GetViewNotesViewModelData(int requestId)
        {
            ViewNotesViewModel ViewNotes = new ViewNotesViewModel();

            var requestNotes = await _notesAndLogsRepository.GetNoteByRequestId(requestId);
            var requestStatusLogs = _notesAndLogsRepository.GetStatusLogsByRequestId(requestId).ToList();

            ViewNotes.RequestId = requestId;
            ViewNotes.AdminNotes = requestNotes?.AdminNotes;
            ViewNotes.PhysicianNotes = requestNotes?.PhysicianNotes;
            List<string> transfernotes = new List<string>();
            foreach(var log in requestStatusLogs)
            {
                if(log.Notes != null)
                {
                    if (log.Status == 3)
                    {
                        if (log.PhysicianId != null)
                        {
                            ViewNotes.PhysicianCancellationNotes = log.Notes;
                        }
                        else if (log.AdminId != null)
                        {
                            ViewNotes.AdminCancellationNotes = log.Notes;
                        }
                    }
                    else if (log.Status == 7)
                    {
                        ViewNotes.PatientCancellationNotes = log.Notes;
                    }
                    else
                    {
                        transfernotes.Add(log.Notes);
                    }
                }
            }
            ViewNotes.TransferNotes = transfernotes;

            return ViewNotes;
        }

        public async Task<bool> AddAdminNote(int requestId, string AdminNotesInput)
        {
            var requestNote = await _notesAndLogsRepository.GetNoteByRequestId(requestId);
            if (requestNote == null)
            {
                RequestNote note = new RequestNote();

                note.AdminNotes = AdminNotesInput;
                note.RequestId = requestId;
                note.CreatedBy = "sd";
                note.CreatedDate = DateTime.Now;

                await _notesAndLogsRepository.AddRequestNote(note);

                return true;
            }
            else
            {
                requestNote.AdminNotes = AdminNotesInput;

                await _notesAndLogsRepository.UpdateRequestNote(requestNote);

                return true;
            }
        }

        public CancelCaseViewModel GetCancelCaseViewModelData(CancelCaseViewModel CancelCase)
        {
            CancelCase.CaseTags = new List<string>();
            CancelCase.CaseTagIds = new List<int>();

            var requestFetched = _requestRepository.GetIQueryableRequestByRequestId(CancelCase.RequestId);
            var request = requestFetched.Include(x => x.RequestClients).ToList();


            
            var CaseTags = _commonRepository.GetAllCaseTags();


            foreach(var tag in CaseTags)
            {
                if(tag == null) continue;
                if(tag.Name != null)
                {
                    CancelCase.CaseTags.Add(tag.Name);
                }
                CancelCase.CaseTagIds.Add(tag.CaseTagId);
            }

            RequestClient? requestClient = request[0].RequestClients?.FirstOrDefault();
            CancelCase.PatientFullName = requestClient?.FirstName + " " + requestClient?.LastName;

            return CancelCase;
        }

        public async Task<bool> CancelCase(CancelCaseViewModel CancelCase)
        {
            var requestFetched = await _requestRepository.GetRequestByRequestId(CancelCase.RequestId);
            if(requestFetched != null)
            {
                requestFetched.Status = 3;
                requestFetched.CaseTag = CancelCase.CaseTagId.ToString();

                await _requestRepository.UpdateRequest(requestFetched);

                RequestStatusLog log = new RequestStatusLog();
                log.RequestId = CancelCase.RequestId;
                log.Status = requestFetched.Status;
                log.AdminId = CancelCase.AdminId??1;
                log.Notes = "Admin cancelled the case on " + DateOnly.FromDateTime(DateTime.Now) + " at " + DateTime.Now.ToShortTimeString() + ": " + CancelCase.AdminCancellationNote;
                log.CreatedDate = DateTime.Now;

                await _notesAndLogsRepository.AddRequestStatusLog(log);
            }
            
            return true;
        }
    }
}
