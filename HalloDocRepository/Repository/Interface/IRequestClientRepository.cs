using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Repository.Interface
{
    public interface IRequestClientRepository
    {
        Task<RequestClient> GetLastRequestClient(string email);
    }
}
