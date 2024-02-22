using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Repository.Interface;
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

        public async Task<bool> UpdateRequests(List<Request> requests)
        {
            _context.Requests.UpdateRange(requests);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
