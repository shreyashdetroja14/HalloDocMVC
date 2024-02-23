using HalloDocRepository.Interface;
using HalloDocRepository.Repository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDocServices.Implementation
{
    public class PatientService : IPatientService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IPhysicianRepository _physicianRepository;

        public PatientService(IUserRepository userRepository, IRequestRepository requestRepository, IPhysicianRepository physicianRepository)
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _physicianRepository = physicianRepository;
        }

        public async Task<int> CheckUser(string id)
        {
            var userFetched = await _userRepository.GetUserByAspNetUserId(id);
            if(userFetched == null)
            {
                return 0;
            }
            return userFetched.UserId;
        }

        public async Task<List<DashboardRequestViewModel>> GetRequestList(int userId)
        {
            var requestWiseFiles = await _requestRepository.GetAllRequestWiseFiles();

            var requestsFetched = await _requestRepository.GetRequestsWithFileCount(userId);

            var physiciansFetched = await _physicianRepository.GetAllPhysicians();

            var result = requestsFetched.Select(x => new
            {
                count = x.RequestWiseFiles.Count,
                request = x,
                files = x.RequestWiseFiles
            });

            List<DashboardRequestViewModel> requestlist = new List<DashboardRequestViewModel>();
            foreach (var r in result)
            {
                //Debug.Print(($@"""{r.RequestId}"" ""{r.CreatedDate}"" ""{r.FileCount}"" "));
                requestlist.Add(new DashboardRequestViewModel
                {
                    RequestId = r.request.RequestId,
                    CreateDate = DateOnly.FromDateTime(r.request.CreatedDate),
                    Status = r.request.Status,
                    Count = r.count,
                    PhysicianId = r.request.PhysicianId
                });
            }

            return requestlist;
        }
    }
}
