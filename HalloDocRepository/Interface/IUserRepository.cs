using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface IUserRepository
    {
        Task<AspNetUser> GetAspNetUserByEmail(string email);

        Task<AspNetUser> GetAspNetUserById(string id);

        Task<AspNetUser> CreateAspNetUser(AspNetUser aspnetuser);

        Task<bool> UpdateAspNetUser(AspNetUser aspnetuser);

        Task<User> CreateUser(User user);

    }
}
