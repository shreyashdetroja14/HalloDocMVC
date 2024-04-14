using HalloDocEntities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface IRequestRepository
    {
        // Request Client 

        Task<RequestClient> GetLastRequestClient(string email);

        Task<List<RequestClient>> GetRequestsClientsByEmail(string email);

        Task<RequestClient> CreateRequestClient(RequestClient requestClient);

        Task<bool> UpdateRequestClient(RequestClient requestClient);

        Task<bool> UpdateRequestClients(List<RequestClient> requestClients);

        Task<RequestClient> GetRequestClientByRequestId(int requestId);

        // Request 

        Task<List<Request>> GetAllRequests();

        IQueryable<Request> GetIQueryableRequests();

        IEnumerable<Request> GetAllIEnumerableRequests();

        Task<bool> UpdateRequests(List<Request> requests);

        Task<bool> UpdateRequest(Request request);

        Task<Request> CreateRequest(Request request);

        Task<Request> GetRequestByConciergeEmail(string email);

        Task<List<Request>> GetAllRequestsByUserId(int userId);

        Task<List<Request>> GetRequestsWithFileCount(int userId);

        Task<Request> GetRequestByRequestId(int requestId);

        Task<List<Request>> GetRequestByRequestIdAsList(int requestId);

        IQueryable<Request> GetIQueryableRequestByRequestId(int requestId);

        Task<List<Request>> GetRequestsByEmail(string email);

        Task<int> GetNewRequestCount(int? physicianId = null);

        Task<int> GetPendingRequestCount(int? physicianId = null);

        Task<int> GetActiveRequestCount(int? physicianId = null);

        Task<int> GetConcludeRequestCount(int? physicianId = null);

        Task<int> GetToCloseRequestCount();

        Task<int> GetUnpaidRequestCount();

        Task<int> GetClearedRequestCount();

        int GetTotalRequestCountByDate(DateOnly date);


        // Request Wise File

        Task<List<RequestWiseFile>> CreateRequestWiseFiles(List<RequestWiseFile> requestWiseFiles);

        Task<List<RequestWiseFile>> GetAllRequestWiseFiles();

        Task<List<RequestWiseFile>> GetRequestWiseFilesByRequestId(int requestId);

        Task<RequestWiseFile> GetRequestWiseFileByFileId(int fileId);

        List<RequestWiseFile> GetRequestWiseFilesByFileIds(List<int> fileIds);

        Task<RequestWiseFile> UpdateRequestWiseFile(RequestWiseFile requestWiseFile);

        Task UpdateRequestWiseFiles(List<RequestWiseFile> requestWiseFiles);


        // Request Business

        Task<RequestBusiness> CreateRequestBusiness(RequestBusiness requestBusiness);


        // Block Requests

        Task<BlockRequest> CreateBlockRequest(BlockRequest blockRequest);

        IQueryable<BlockRequest> GetIQueryableBlockedRequests();

        BlockRequest GetBlockRequestById(int blockRequestId);

        Task<BlockRequest> UpdateBlockRequest(BlockRequest blockRequest);
    }
}
