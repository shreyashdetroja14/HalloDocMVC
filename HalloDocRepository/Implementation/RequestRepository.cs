using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Implementation
{
    public class RequestRepository : IRequestRepository
    {
        private readonly HalloDocContext _context;
        public RequestRepository(HalloDocContext context)
        {
            _context = context;
        }
        public async Task<RequestClient> GetLastRequestClient(string email)
        {
            var requestClientFetched = await _context.RequestClients.OrderBy(x => x.RequestClientId).LastOrDefaultAsync(m => m.Email == email);
            return requestClientFetched;
        }

        public async Task<List<RequestClient>> GetRequestsClientsByEmail(string email)
        {
            var requestclientsFetched = await _context.RequestClients.Where(x => x.Email == email).ToListAsync();

            return requestclientsFetched;
        }

        public async Task<List<Request>> GetAllRequests()
        {
            var requests = await _context.Requests.ToListAsync();

            return requests;
        }

        public IEnumerable<Request> GetAllIEnumerableRequests()
        {
            var requests =  _context.Requests.AsEnumerable();

            return requests;
        }

        public async Task<bool> UpdateRequests(List<Request> requests)
        {
            _context.Requests.UpdateRange(requests);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Request> CreateRequest(Request request)
        {
            _context.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<RequestClient> CreateRequestClient(RequestClient requestClient)
        {
            _context.Add(requestClient);
            await _context.SaveChangesAsync();

            return requestClient;
        }

        public async Task<List<RequestWiseFile>> CreateRequestWiseFiles(List<RequestWiseFile> requestWiseFiles)
        {
            _context.RequestWiseFiles.AddRange(requestWiseFiles);
            await _context.SaveChangesAsync();

            return requestWiseFiles;
        }

        public async Task<Request> GetRequestByConciergeEmail(string email)
        {
            var conciergeRequest = await _context.Requests.FirstOrDefaultAsync(m => m.Email == email && m.RequestTypeId == 4);
            return conciergeRequest;
        }

        public async Task<RequestBusiness> CreateRequestBusiness(RequestBusiness requestBusiness)
        {
            _context.Add(requestBusiness);
            await _context.SaveChangesAsync();

            return requestBusiness;
        }

        public async Task<List<RequestWiseFile>> GetAllRequestWiseFiles()
        {
            List<RequestWiseFile> requestWiseFiles = await _context.RequestWiseFiles.ToListAsync();
            return requestWiseFiles;
        }

        public async Task<List<Request>> GetAllRequestsByUserId(int userId)
        {
            List<Request> requests = await _context.Requests.Where(r => r.UserId == userId).ToListAsync();
            return requests;
        }

        public async Task<List<Request>> GetRequestsWithFileCount(int userId)
        {
            return await _context.Requests.Include(x => x.RequestWiseFiles).Include(x => x.Physician).Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedDate).ToListAsync();
        }

        public async Task<List<RequestWiseFile>> GetRequestWiseFilesByRequestId(int requestId)
        {
            var requestwisefiles = await _context.RequestWiseFiles.Where(x => x.RequestId == requestId).ToListAsync();
            return requestwisefiles;
        }

        public async Task<Request> GetRequestByRequestId(int requestId)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(x => x.RequestId == requestId);
            return request;
        }

        public async Task<List<Request>> GetRequestByRequestIdAsList(int requestId)
        {
            var requestAsList = await _context.Requests.Where(x => x.RequestId == requestId).ToListAsync();
            return requestAsList;
        }

        public IQueryable<Request> GetIQueryableRequestByRequestId(int requestId)
        {
            var request = _context.Requests.AsQueryable().Where(x => x.RequestId == requestId);
            return request;
        }

        public async Task<RequestWiseFile> GetRequestWiseFileByFileId(int fileId)
        {
            var requestwisefile = await _context.RequestWiseFiles.FindAsync(fileId);
            return requestwisefile;
        }

        public async Task<List<Request>> GetRequestsByEmail(string email)
        {
            List<Request> requests = new List<Request>();
            requests = await _context.Requests.Where(m => m.Email == email).ToListAsync();

            return requests;
        }

        public async Task<bool> UpdateRequestClient(RequestClient requestClient)
        {
            _context.RequestClients.Update(requestClient);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateRequestClients(List<RequestClient> requestClients)
        {
            _context.RequestClients.UpdateRange(requestClients);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetNewRequestCount()
        {
            int count = await _context.Requests.Where(x => x.Status == 1).CountAsync();
            return count;
        }

        public async Task<int> GetPendingRequestCount()
        {
            int count = await _context.Requests.Where(x => x.Status == 2).CountAsync();
            return count;
        }

        public async Task<int> GetActiveRequestCount()
        {
            int count = await _context.Requests.Where(x => x.Status == 4 || x.Status == 5).CountAsync();
            return count;
        }

        public async Task<int> GetConcludeRequestCount()
        {
            int count = await _context.Requests.Where(x => x.Status == 6).CountAsync();
            return count;
        }

        public async Task<int> GetToCloseRequestCount()
        {
            int count = await _context.Requests.Where(x => x.Status == 3 || x.Status == 7 || x.Status == 8).CountAsync();
            return count;
        }

        public async Task<int> GetUnpaidRequestCount()
        {
            int count = await _context.Requests.Where(x => x.Status == 9).CountAsync();
            return count;
        }

        public async Task<int> GetClearedRequestCount()
        {
            int count = await _context.Requests.Where(x => x.Status == 10).CountAsync();
            return count;
        }

        public async Task<RequestClient> GetRequestClientByRequestId(int requestId)
        {
            var requestClient = await _context.RequestClients.FirstOrDefaultAsync(x => x.RequestId == requestId);
            return requestClient;
        }
    }
}
