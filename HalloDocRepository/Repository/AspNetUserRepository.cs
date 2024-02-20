using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocEntities.ViewModels;
using HalloDocRepository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Repository
{
    public class AspNetUserRepository : IAspNetUserRepository
    {
        private readonly HalloDocContext _context;
        public AspNetUserRepository(HalloDocContext context)
        {
            _context = context;
        }
        public async Task<AspNetUser> GetAspNetUserByEmail(string email)
        {
            var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == email);
            return aspnetuserFetched;
        }

        public AspNetUser CreateAspNetUser(CreateAccountViewModel Credentials)
        {
            var aspnetuserNew = new AspNetUser();
            aspnetuserNew.Id = Guid.NewGuid().ToString();
            aspnetuserNew.UserName = Credentials.Email;
            aspnetuserNew.Email = Credentials.Email;
            aspnetuserNew.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Credentials.Password);
            aspnetuserNew.CreatedDate = DateTime.Now;

            _context.AspNetUsers.Add(aspnetuserNew);

            return aspnetuserNew;
        }
    }
}
