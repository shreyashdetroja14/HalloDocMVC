using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using Microsoft.EntityFrameworkCore;
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

        public async Task<AspNetUser> GetAspNetUserById(string id)
        {
            var aspnetuser = await _context.AspNetUsers.FindAsync(id);
            return aspnetuser;
        }
    }
}
