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
        Task<RequestClient> GetLastRequestClient(string email);

        Task<List<RequestClient>> GetRequestsClientsByEmail(string email);

        Task<List<Request>> GetAllRequests();

        Task<bool> UpdateRequests(List<Request> requests);
    }
}
