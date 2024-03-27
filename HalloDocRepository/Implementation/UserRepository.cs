using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly HalloDocContext _context;
        public UserRepository(HalloDocContext context)
        {
            _context = context;
        }
        public async Task<AspNetUser> GetAspNetUserByEmail(string email)
        {
            var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == email);
            return aspnetuserFetched;
        }

        public IQueryable<AspNetUser> GetIQueryableAspNetUserByEmail(string email)
        {
            var aspnetuserFetched = _context.AspNetUsers.AsQueryable().Include(x => x.Users).Include(x => x.AdminAspNetUsers).Include(x => x.PhysicianAspNetUsers).Include(x => x.AspNetUserRoles).ThenInclude(x => x.Role).Where(m => m.Email == email);
            return aspnetuserFetched;
        }

        public async Task<AspNetUser> CreateAspNetUser(AspNetUser aspnetuser)
        {
            _context.AspNetUsers.Add(aspnetuser);
            await _context.SaveChangesAsync();

            return aspnetuser;
        }

        public async Task<User> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UpdateAspNetUser(AspNetUser aspnetuser)
        {
            _context.AspNetUsers.Update(aspnetuser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<AspNetUser> GetAspNetUserByIdAsync(string id)
        {
            var aspnetuser = await _context.AspNetUsers.FindAsync(id);
            return aspnetuser;
        }

        public AspNetUser GetAspNetUserById(string id)
        {
            var aspnetuser = _context.AspNetUsers.Find(id);
            return aspnetuser ?? new AspNetUser();
        }

        public IQueryable<AspNetUser> GetIQueryableAspNetUserById(string id)
        {
            var aspnetuser = _context.AspNetUsers.AsQueryable().Where(x => x.Id == id);
            return aspnetuser;
        }

        public async Task<User> GetUserByAspNetUserId(string? aspnetuserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.AspNetUserId == aspnetuserId);
            return user;
        }

        public async Task<User> GetUserByEmail(string? email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user;
        }

        public async Task<Concierge> CreateConcierge(Concierge concierge)
        {
            _context.Add(concierge);
            await _context.SaveChangesAsync();

            return concierge;
        }

        public async Task<Business> GetBusinessByPhoneAndName(string? phone, string name)
        {
            var business = await _context.Businesses.FirstOrDefaultAsync(m => m.PhoneNumber == phone && m.Name == name);
            return business;
        }

        public async Task<Business> CreateBusiness(Business business)
        {
            _context.Add(business);
            await _context.SaveChangesAsync();

            return business;
        }

        public async Task<User> GetUserByUserId(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<AspNetUserRole> CreateAspNetUserRole(AspNetUserRole aspnetuserrole)
        {
            _context.Add(aspnetuserrole);
            await _context.SaveChangesAsync();

            return aspnetuserrole;
        }
    }
}
