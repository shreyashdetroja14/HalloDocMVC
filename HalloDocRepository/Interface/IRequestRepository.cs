using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Repository.Interface
{
    public interface IRequestRepository
    {
        // Request Client 

        Task<RequestClient> GetLastRequestClient(string email);

        Task<List<RequestClient>> GetRequestsClientsByEmail(string email);

        Task<RequestClient> CreateRequestClient(RequestClient requestClient);


        // Request 

        Task<List<Request>> GetAllRequests();

        Task<bool> UpdateRequests(List<Request> requests);

        Task<Request> CreateRequest(Request request);

        Task<Request> GetRequestByConciergeEmail(string email);

        Task<List<Request>> GetAllRequestsByUserId(int userId);

        Task<List<Request>> GetRequestsWithFileCount(int userId);


        // Request Wise File

        Task<List<RequestWiseFile>> CreateRequestWiseFiles(List<RequestWiseFile> requestWiseFiles);

        Task<List<RequestWiseFile>> GetAllRequestWiseFiles();


        // Request Business

        Task<RequestBusiness> CreateRequestBusiness(RequestBusiness requestBusiness);
    }
}
