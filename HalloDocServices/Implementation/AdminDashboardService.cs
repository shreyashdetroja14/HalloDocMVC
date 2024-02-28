﻿using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Implementation
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IPhysicianRepository _physicianRepository;
        public AdminDashboardService(IUserRepository userRepository, IRequestRepository requestRepository, IPhysicianRepository physicianRepository) 
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _physicianRepository = physicianRepository;
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
            List<RequestRowViewModel> viewModels = new List<RequestRowViewModel>();
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

            requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => myarray.Contains(x.Status));

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
                viewModels.Add(new()
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
                    Region = requestClient?.RegionId
                    //Notes
                });
            }

            return viewModels;
        }
    }
}
