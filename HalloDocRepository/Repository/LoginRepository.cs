using HalloDocEntities.Data;
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
    public class LoginRepository : ILoginRepository
    {
        private readonly HalloDocContext _context;

        public LoginRepository(HalloDocContext context, IAspNetUserRepository aspnetuserRepository, IRequestClientRepository requestClientRepository)
        {
            _context = context;
            
        }
        public async Task<string> CheckLogin(LoginViewModel LoginInfo)
        {
            string id = "";
            
            var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == LoginInfo.Email);
            if (aspnetuserFetched != null)
            {
                if (BCrypt.Net.BCrypt.Verify(LoginInfo.Password, aspnetuserFetched.PasswordHash))
                {
                    id = aspnetuserFetched.Id;
                }
            }
            return id;
        }

        public async Task<string> CreateAccount(CreateAccountViewModel Credentials)
        {
            var aspuserFetched = await _aspnetuserRepository.GetAspNetUserByEmail(Credentials.Email);
            if(aspuserFetched != null)
            {
                return "user exists";
            }
            var aspnetuserNew = _aspnetuserRepository.CreateAspNetUser(Credentials);
            var lastRequestClient = _requestClientRepository.GetLastRequestClient(Credentials.Email);
            if(lastRequestClient != null)
            {

            }
            else
            {

            }
            return "account created";
        }
    }
}
