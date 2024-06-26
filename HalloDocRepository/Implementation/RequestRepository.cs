﻿using HalloDocEntities.Data;
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

        public IQueryable<Request> GetIQueryableRequests()
        {
            return _context.Requests.AsQueryable();
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

        public async Task<bool> UpdateRequest(Request request)
        {
            _context.Requests.Update(request);
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

        public Request GetRequest(int requestId)
        {
            return _context.Requests.FirstOrDefault(x => x.RequestId == requestId) ?? new Request();
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

        public async Task<int> GetNewRequestCount(int? physicianId = null)
        {
            int count = await _context.Requests.Where(x => (physicianId == null || x.PhysicianId == physicianId) && x.IsDeleted != true && (x.Status == 1)).CountAsync();
            return count;
        }

        public async Task<int> GetPendingRequestCount(int? physicianId = null)
        {
            int count = await _context.Requests.Where(x => (physicianId == null || x.PhysicianId == physicianId) && x.IsDeleted != true && (x.Status == 2)).CountAsync();
            return count;
        }

        public async Task<int> GetActiveRequestCount(int? physicianId = null)
        {
            int count = await _context.Requests.Where(x => (physicianId == null || x.PhysicianId == physicianId) && x.IsDeleted != true && (x.Status == 4 || x.Status == 5)).CountAsync();
            return count;
        }

        public async Task<int> GetConcludeRequestCount(int? physicianId = null)
        {
            int count = await _context.Requests.Where(x => (physicianId == null || x.PhysicianId == physicianId) && x.IsDeleted != true && (x.Status == 6)).CountAsync();
            return count;
        }

        public async Task<int> GetToCloseRequestCount()
        {
            int count = await _context.Requests.Where(x => x.IsDeleted != true && (x.Status == 3 || x.Status == 7 || x.Status == 8)).CountAsync();
            return count;
        }

        public async Task<int> GetUnpaidRequestCount()
        {
            int count = await _context.Requests.Where(x => x.IsDeleted != true && x.Status == 9).CountAsync();
            return count;
        }

        public async Task<int> GetClearedRequestCount()
        {
            int count = await _context.Requests.Where(x => x.IsDeleted != true && x.Status == 10).CountAsync();
            return count;
        }

        public int GetTotalRequestCountByDate(DateOnly date)
        {
            int count = _context.Requests.Where(x => DateOnly.FromDateTime(x.CreatedDate) == date).Count();
            return count;
        }

        public async Task<RequestClient> GetRequestClientByRequestId(int requestId)
        {
            var requestClient = await _context.RequestClients.FirstOrDefaultAsync(x => x.RequestId == requestId);
            return requestClient;
        }

        public async Task<BlockRequest> CreateBlockRequest(BlockRequest blockRequest)
        {
            _context.Add(blockRequest);
            await _context.SaveChangesAsync();

            return blockRequest;
        }



        public List<RequestWiseFile> GetRequestWiseFilesByFileIds(List<int> fileIds)
        {
            var requestwisefiles = _context.RequestWiseFiles.Where(x => fileIds.Contains(x.RequestWiseFileId)).ToList();
            return requestwisefiles;
        }

        public async Task<RequestWiseFile> UpdateRequestWiseFile(RequestWiseFile requestWiseFile)
        {
            _context.Update(requestWiseFile);
            await _context.SaveChangesAsync();

            return requestWiseFile;
        }

        public async Task UpdateRequestWiseFiles(List<RequestWiseFile> requestWiseFiles)
        {
            _context.UpdateRange(requestWiseFiles);
            await _context.SaveChangesAsync();
        }

        public IQueryable<BlockRequest> GetIQueryableBlockedRequests()
        {
            return _context.BlockRequests.AsQueryable();


        }

        public BlockRequest GetBlockRequestById(int blockRequestId)
        {
            return _context.BlockRequests.FirstOrDefault(x => x.BlockRequestId == blockRequestId) ?? new BlockRequest();
        }
        
        public BlockRequest GetBlockRequestByEmail(string email)
        {
            return _context.BlockRequests.FirstOrDefault(x => x.Email == email && x.IsActive == true) ?? new BlockRequest();
        }

        public async Task<BlockRequest> UpdateBlockRequest(BlockRequest blockRequest)
        {
            _context.BlockRequests.Update(blockRequest);
            await _context.SaveChangesAsync();

            return blockRequest;
        }

        
    }
}
