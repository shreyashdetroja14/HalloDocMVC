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
        // AspNetUser

        Task<AspNetUser> GetAspNetUserByEmail(string email);

        Task<AspNetUser> GetAspNetUserById(string id);

        Task<AspNetUser> CreateAspNetUser(AspNetUser aspnetuser);

        Task<bool> UpdateAspNetUser(AspNetUser aspnetuser);


        // User

        Task<User> CreateUser(User user);

        Task<User> GetUserByAspNetUserId(string? aspnetuserId);

        Task<User> GetUserByEmail(string? email);

        Task<User> GetUserByUserId(int userId);


        // Concierge

        Task<Concierge> CreateConcierge(Concierge concierge);


        // Business

        Task<Business> GetBusinessByPhoneAndName(string? phone, string name);

        Task<Business> CreateBusiness(Business business);
    }
}
