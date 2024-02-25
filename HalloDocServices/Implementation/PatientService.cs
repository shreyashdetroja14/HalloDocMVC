﻿using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
            if (userFetched == null)
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
                physicianName = x.Physician?.FirstName + " " + x.Physician?.LastName
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
                    PhysicianName = r.physicianName
                });
            }

            return requestlist;
        }

        public async Task<List<RequestFileViewModel>> GetRequestFiles(int requestId)
        {
            var requestwisefilesFetched = await _requestRepository.GetRequestWiseFilesByRequestId(requestId);
            var requestFetched = await _requestRepository.GetRequestByRequestIdAsList(requestId);


            var data = requestwisefilesFetched.Join(requestFetched,
                rwf => rwf.RequestId,
                req => req.RequestId,
                (rwf, req) => new { rwf, req }
                );

            /*var data = from requests in _context.Requests.ToList()
                       join files in requestwisefilesFetched
                       on requests.RequestId equals files.RequestId
                       select new
                       {
                           requests,
                           files
                       };*/

            List<RequestFileViewModel> requestfilelist = new List<RequestFileViewModel>();
            foreach (var row in data)
            {
                requestfilelist.Add(new RequestFileViewModel
                {
                    FileId = row.rwf.RequestWiseFileId,
                    FileName = Path.GetFileName(row.rwf.FileName),
                    Uploader = row.req.FirstName + " " + row.req.LastName,
                    UploadDate = DateOnly.FromDateTime(row.rwf.CreatedDate),
                    FilePath = row.rwf.FileName
                });
            }

            return requestfilelist;
        }

        public async Task<int> GetUserIdByRequestId(int requestId)
        {
            var requestFetched = await _requestRepository.GetRequestByRequestId(requestId);
            return requestFetched.UserId ?? 0;
        }

        public async Task<string> GetAspNetUserIdByUserId(int userId)
        {
            var userFetched = await _userRepository.GetUserByUserId(userId);
            return userFetched.AspNetUserId?? "";
        }

        public async Task UploadFiles(IEnumerable<IFormFile> MultipleFiles, int requestId)
        {
            List<string> files = new List<string>();
            foreach (var UploadFile in MultipleFiles)
            {
                string FilePath = "wwwroot\\Upload\\" + requestId;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";

                string fileNameWithPath = Path.Combine(path, newfilename);
                files.Add(FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + newfilename);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
            }
            List<RequestWiseFile> requestWiseFiles = new List<RequestWiseFile>();
            foreach (string file in files)
            {
                var reqwisefileNew = new RequestWiseFile();
                reqwisefileNew.RequestId = requestId;
                reqwisefileNew.FileName = file;
                reqwisefileNew.CreatedDate = DateTime.Now;

                requestWiseFiles.Add(reqwisefileNew);
            }
            await _requestRepository.CreateRequestWiseFiles(requestWiseFiles);
        }

        public async Task<RequestWiseFile> RequestFileData(int fileId)
        {
            var requestwisefileFetched = await _requestRepository.GetRequestWiseFileByFileId(fileId);
            return requestwisefileFetched;
        }

        public async Task<ProfileViewModel> GetProfileDetails(int userId)
        {
            var userFetched = await _userRepository.GetUserByUserId(userId);

            ProfileViewModel ProfileDetails = new ProfileViewModel();
            ProfileDetails.UserId = userId;
            ProfileDetails.FirstName = userFetched.FirstName;
            ProfileDetails.LastName = userFetched.LastName;
            if (userFetched.IntDate.HasValue && userFetched.IntYear.HasValue && userFetched.StrMonth != null)
            {
                DateTime monthDateTime = DateTime.ParseExact(userFetched.StrMonth, "MMMM", CultureInfo.InvariantCulture);
                int month = monthDateTime.Month;
                DateOnly date = new DateOnly((int)userFetched.IntYear, month, userFetched.IntDate.Value);
                ProfileDetails.DOB = date.ToString("yyyy-MM-dd");
            }
            ProfileDetails.PhoneNumber = userFetched.Mobile;
            ProfileDetails.Email = userFetched.Email;
            ProfileDetails.Street = userFetched.Street;
            ProfileDetails.City = userFetched.City;
            ProfileDetails.State = userFetched.State;
            ProfileDetails.ZipCode = userFetched.ZipCode;

            return ProfileDetails;
        }

        public async Task EditProfile(ProfileViewModel ProfileDetails)
        {
            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(ProfileDetails.Email);
            if (aspnetuserFetched != null)
            {
                aspnetuserFetched.PhoneNumber = ProfileDetails.PhoneNumber;

                await _userRepository.UpdateAspNetUser(aspnetuserFetched);
            }
            var userFetched = await _userRepository.GetUserByEmail(ProfileDetails.Email);
            if (userFetched != null)
            {
                userFetched.FirstName = ProfileDetails.FirstName;
                userFetched.LastName = ProfileDetails.LastName;

                DateTime dateTime = DateTime.ParseExact(ProfileDetails.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                userFetched.IntYear = dateTime.Year;
                userFetched.StrMonth = dateTime.ToString("MMMM");
                userFetched.IntDate = dateTime.Day;

                userFetched.Mobile = ProfileDetails.PhoneNumber;
                userFetched.Email = ProfileDetails.Email;
                userFetched.Street = ProfileDetails.Street;
                userFetched.City = ProfileDetails.City;
                userFetched.State = ProfileDetails.State;
                userFetched.ZipCode = ProfileDetails.ZipCode;

                await _userRepository.UpdateUser(userFetched);
            }

            var requestsFetched = await _requestRepository.GetRequestsByEmail(ProfileDetails.Email);
            if (requestsFetched.Count > 0)
            {
                foreach (var request in requestsFetched)
                {
                    request.FirstName = ProfileDetails.FirstName;
                    request.LastName = ProfileDetails.LastName;
                    request.PhoneNumber = ProfileDetails.PhoneNumber;
                }

                await _requestRepository.UpdateRequests(requestsFetched);
            }

            var requestClientsFetched = await _requestRepository.GetRequestsClientsByEmail(ProfileDetails.Email);
            if (requestClientsFetched.Count > 0)
            {
                foreach (var requestClient in requestClientsFetched)
                {
                    requestClient.FirstName = ProfileDetails.FirstName;
                    requestClient.LastName = ProfileDetails.LastName;
                    requestClient.PhoneNumber = ProfileDetails.PhoneNumber;
                    requestClient.Email = ProfileDetails.Email;

                    DateTime dateTime = DateTime.ParseExact(ProfileDetails.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    requestClient.IntYear = dateTime.Year;
                    requestClient.StrMonth = dateTime.ToString("MMMM");
                    requestClient.IntDate = dateTime.Day;


                }
                await _requestRepository.UpdateRequestClients(requestClientsFetched);
            }
        }

        public async Task<PatientRequestViewModel> GetPatientInfo(int userId)
        {
            var PatientInfo = new PatientRequestViewModel();
            var userFetched = await _userRepository.GetUserByUserId(userId);
            if (userFetched != null)
            {
                PatientInfo.FirstName = userFetched.FirstName;
                PatientInfo.LastName = userFetched.LastName;
                PatientInfo.Email = userFetched.Email;
                PatientInfo.PhoneNumber = userFetched.Mobile;
                if (userFetched.IntDate.HasValue && userFetched.IntYear.HasValue && userFetched.StrMonth != null)
                {
                    DateTime monthDateTime = DateTime.ParseExact(userFetched.StrMonth, "MMMM", CultureInfo.InvariantCulture);
                    int month = monthDateTime.Month;
                    DateOnly date = new DateOnly((int)userFetched.IntYear, month, userFetched.IntDate.Value);
                    PatientInfo.DOB = date.ToString("yyyy-MM-dd");
                }
            }

            return PatientInfo;
        }


        #region LocalMethodsForRequests

        public AspNetUser CreateAspnetuser(PatientRequestViewModel prvm)
        {
            var aspnetuserNew = new AspNetUser();

            aspnetuserNew.Id = Guid.NewGuid().ToString();

            aspnetuserNew.UserName = prvm.Email;
            aspnetuserNew.PasswordHash = prvm.Password;
            aspnetuserNew.Email = prvm.Email;
            aspnetuserNew.PhoneNumber = prvm.PhoneNumber;
            aspnetuserNew.CreatedDate = DateTime.Now;

            return aspnetuserNew;
        }
        public User CreateUser(PatientRequestViewModel prvm, AspNetUser aspnetuserNew)
        {
            var userNew = new User();

            userNew.AspNetUserId = aspnetuserNew.Id;
            userNew.FirstName = prvm.FirstName;
            userNew.LastName = prvm.LastName;
            userNew.Email = prvm.Email;
            userNew.Mobile = prvm.PhoneNumber;
            userNew.Street = prvm.Street;
            userNew.City = prvm.City;
            userNew.State = prvm.State;
            userNew.ZipCode = prvm.ZipCode;

            if (prvm.DOB != null)
            {
                DateTime dateTime = DateTime.ParseExact(prvm.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                userNew.IntYear = dateTime.Year;
                userNew.StrMonth = dateTime.ToString("MMMM");
                userNew.IntDate = dateTime.Day;
            }

            userNew.CreatedBy = "admin";
            userNew.CreatedDate = DateTime.Now;

            return userNew;
        }
        public RequestClient CreateRequestClient(PatientRequestViewModel PatientInfo, Request requestNew)
        {
            var requestClientNew = new RequestClient();
            requestClientNew.RequestId = requestNew.RequestId;
            requestClientNew.FirstName = PatientInfo.FirstName;
            requestClientNew.LastName = PatientInfo.LastName;
            requestClientNew.PhoneNumber = PatientInfo.PhoneNumber;
            requestClientNew.Location = PatientInfo.Room;
            requestClientNew.Address = PatientInfo.Street + ", " + PatientInfo.City + ", " + PatientInfo.State + ", " + PatientInfo.ZipCode;
            requestClientNew.NotiMobile = PatientInfo.PhoneNumber;
            requestClientNew.NotiEmail = PatientInfo.Email;
            requestClientNew.Notes = PatientInfo.Symptoms;
            requestClientNew.Email = PatientInfo.Email;

            if (PatientInfo.DOB != null)
            {
                DateTime dateTime = DateTime.ParseExact(PatientInfo.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                requestClientNew.IntYear = dateTime.Year;
                requestClientNew.StrMonth = dateTime.ToString("MMMM");
                requestClientNew.IntDate = dateTime.Day;
            }

            requestClientNew.Street = PatientInfo.Street;
            requestClientNew.City = PatientInfo.City;
            requestClientNew.State = PatientInfo.State;
            requestClientNew.ZipCode = PatientInfo.ZipCode;
            return requestClientNew;
        }

        public List<string> UploadFilesToServer(IEnumerable<IFormFile> MultipleFiles, int requestid)
        {
            List<string> files = new List<string>();
            foreach (var UploadFile in MultipleFiles)
            {
                string FilePath = "wwwroot\\Upload\\" + requestid;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";

                string fileNameWithPath = Path.Combine(path, newfilename);
                files.Add(FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + newfilename);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
            }
            return files;
        }


        #endregion

        public async Task<int> CreatePatientRequest(FamilyRequestViewModel frvm)
        {
            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(frvm.PatientInfo.Email);
            var userFetched = await _userRepository.GetUserByEmail(frvm.PatientInfo.Email);
            if (userFetched != null && aspnetuserFetched != null)
            {
                var requestNew = new Request();

                requestNew.RequestTypeId = 2;
                requestNew.UserId = userFetched.UserId;
                requestNew.FirstName = frvm.PatientInfo.FirstName;
                requestNew.LastName = frvm.PatientInfo.LastName;
                requestNew.PhoneNumber = frvm.PatientInfo.PhoneNumber;
                requestNew.Email = frvm.PatientInfo.Email;
                requestNew.Status = 1;
                requestNew.CreatedDate = DateTime.Now;
                requestNew.IsUrgentEmailSent = false;
                requestNew.PatientAccountId = aspnetuserFetched?.Id;
                requestNew.CreatedUserId = userFetched.UserId;

                await _requestRepository.CreateRequest(requestNew);

                var requestClientNew = CreateRequestClient(frvm.PatientInfo, requestNew);

                await _requestRepository.CreateRequestClient(requestClientNew);

                if (frvm.PatientInfo.MultipleFiles != null)
                {
                    List<string> files = UploadFilesToServer(frvm.PatientInfo.MultipleFiles, requestNew.RequestId);
                    List<RequestWiseFile> requestWiseFiles = new List<RequestWiseFile>();
                    foreach (string file in files)
                    {
                        var reqwisefileNew = new RequestWiseFile();
                        reqwisefileNew.RequestId = requestNew.RequestId;
                        reqwisefileNew.FileName = file;
                        reqwisefileNew.CreatedDate = DateTime.Now;

                        requestWiseFiles.Add(reqwisefileNew);
                    }
                    await _requestRepository.CreateRequestWiseFiles(requestWiseFiles);
                }
            }

            return userFetched?.UserId ?? 0;
        }

        public async Task CreateFamilyRequest(FamilyRequestViewModel frvm, int userId)
        {
            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(frvm.PatientInfo.Email);
            var userFetched = await _userRepository.GetUserByEmail(frvm.PatientInfo.Email);
            var requestorUser = await _userRepository.GetUserByUserId(userId);
            if (requestorUser != null)
            {
                var requestNew = new Request();

                requestNew.RequestTypeId = 3;
                requestNew.UserId = userFetched?.UserId;
                requestNew.FirstName = requestorUser?.FirstName;
                requestNew.LastName = requestorUser?.LastName;
                requestNew.PhoneNumber = requestorUser?.Mobile;
                requestNew.Email = requestorUser?.Email;
                requestNew.Status = 1;
                requestNew.CreatedDate = DateTime.Now;
                requestNew.IsUrgentEmailSent = false;
                requestNew.PatientAccountId = aspnetuserFetched?.Id;
                requestNew.CreatedUserId = requestorUser?.UserId;
                requestNew.RelationName = frvm.FamilyRelation;

                await _requestRepository.CreateRequest(requestNew);

                var requestClientNew = CreateRequestClient(frvm.PatientInfo, requestNew);

                await _requestRepository.CreateRequestClient(requestClientNew);

                if (frvm.PatientInfo.MultipleFiles != null)
                {
                    List<string> files = UploadFilesToServer(frvm.PatientInfo.MultipleFiles, requestNew.RequestId);
                    List<RequestWiseFile> requestWiseFiles = new List<RequestWiseFile>();
                    foreach (string file in files)
                    {
                        var reqwisefileNew = new RequestWiseFile();
                        reqwisefileNew.RequestId = requestNew.RequestId;
                        reqwisefileNew.FileName = file;
                        reqwisefileNew.CreatedDate = DateTime.Now;

                        requestWiseFiles.Add(reqwisefileNew);
                    }
                    await _requestRepository.CreateRequestWiseFiles(requestWiseFiles);
                }

            }
        }
    }

}

