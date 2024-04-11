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

        IQueryable<AspNetUser> GetIQueryableAspNetUserByEmail(string email);

        Task<AspNetUser> GetAspNetUserByIdAsync(string id);

        AspNetUser GetAspNetUserById(string id);

        IQueryable<AspNetUser> GetIQueryableAspNetUserById(string id);

        Task<AspNetUser> CreateAspNetUser(AspNetUser aspnetuser);

        Task<bool> UpdateAspNetUser(AspNetUser aspnetuser);

        IQueryable<AspNetUser> GetIQueryableAspNetUsers();

        int GetMatchingUserNameCount(string username);


        // AspNetUserRoles

        Task<AspNetUserRole> CreateAspNetUserRole(AspNetUserRole aspnetuserrole);


        // User

        Task<User> CreateUser(User user);

        Task<User> GetUserByAspNetUserId(string? aspnetuserId);

        Task<User> GetUserByEmail(string? email);

        Task<User> GetUserByUserId(int userId);

        Task<User> UpdateUser(User user);

        IQueryable<User> GetIQueryableUsers();


        // Concierge

        Task<Concierge> CreateConcierge(Concierge concierge);


        // Business

        Task<Business> GetBusinessByPhoneAndName(string? phone, string name);

        Task<Business> CreateBusiness(Business business);
    }
}
