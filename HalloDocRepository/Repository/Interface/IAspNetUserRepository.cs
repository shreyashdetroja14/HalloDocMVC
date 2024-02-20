using HalloDocEntities.Models;
using HalloDocEntities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Repository.Interface
{
    public interface IAspNetUserRepository
    {
        Task<AspNetUser> GetAspNetUserByEmail(string email);

        AspNetUser CreateAspNetUser(CreateAccountViewModel Credentials);
    }
}
