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

        Task<bool> UpdateRequestClients(List<RequestClient> requestClients);



        // Request 

        Task<List<Request>> GetAllRequests();

        Task<bool> UpdateRequests(List<Request> requests);

        Task<Request> CreateRequest(Request request);

        Task<Request> GetRequestByConciergeEmail(string email);

        Task<List<Request>> GetAllRequestsByUserId(int userId);

        Task<List<Request>> GetRequestsWithFileCount(int userId);

        Task<Request> GetRequestByRequestId(int requestId);

        Task<List<Request>> GetRequestByRequestIdAsList(int requestId);

        Task<List<Request>> GetRequestsByEmail(string email);


        // Request Wise File

        Task<List<RequestWiseFile>> CreateRequestWiseFiles(List<RequestWiseFile> requestWiseFiles);

        Task<List<RequestWiseFile>> GetAllRequestWiseFiles();

        Task<List<RequestWiseFile>> GetRequestWiseFilesByRequestId(int requestId);

        Task<RequestWiseFile> GetRequestWiseFileByFileId(int fileId);


        // Request Business

        Task<RequestBusiness> CreateRequestBusiness(RequestBusiness requestBusiness);
    }
}
