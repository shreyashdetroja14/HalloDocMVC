using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
                (rwf, req) => new {rwf, req}
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

        public async Task<int> GetUserInfoByRequestId(int requestId)
        {
            var requestFetched = await _requestRepository.GetRequestByRequestId(requestId);
            return requestFetched.UserId??0;
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
    }
}
