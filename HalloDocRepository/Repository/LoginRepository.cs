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
using static System.Net.Mime.MediaTypeNames;

//G:\test\hallodoc3tier\HalloDocMVC\HalloDocMVC.sln

namespace HalloDocRepository.Repository
{
    public class LoginRepository : ILoginRepository
    {
        private readonly HalloDocContext _context;

        public LoginRepository(HalloDocContext context)
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
            var aspuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == Credentials.Email);
            if (aspuserFetched != null)
            {
                return "user exists";
            }

            var requestClientFetched = await _context.RequestClients.OrderBy(x => x.RequestClientId).LastOrDefaultAsync(m => m.Email == Credentials.Email);
            if (requestClientFetched == null)
            {
                return "not eligible";
            }
            else
            {
                var aspnetuserNew = new AspNetUser();
                var userNew = new User();
                var requests = _context.Requests.Where(x => x.RequestId == requestClientFetched.RequestId).ToList();

                aspnetuserNew.Id = Guid.NewGuid().ToString();
                aspnetuserNew.UserName = Credentials.Email;
                aspnetuserNew.Email = Credentials.Email;
                aspnetuserNew.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Credentials.Password);
                aspnetuserNew.CreatedDate = DateTime.Now;

                _context.AspNetUsers.Add(aspnetuserNew);
                await _context.SaveChangesAsync();

                userNew.AspNetUserId = aspnetuserNew.Id;
                userNew.Email = aspnetuserNew.Email;
                userNew.FirstName = requestClientFetched.FirstName;
                userNew.LastName = requestClientFetched?.LastName;
                userNew.Mobile = requestClientFetched?.PhoneNumber;
                userNew.Street = requestClientFetched?.Street;
                userNew.City = requestClientFetched?.City;
                userNew.State = requestClientFetched?.State;
                userNew.ZipCode = requestClientFetched?.ZipCode;
                userNew.StrMonth = requestClientFetched?.StrMonth;
                userNew.IntDate = requestClientFetched?.IntDate;
                userNew.IntYear = requestClientFetched?.IntYear;
                userNew.CreatedDate = DateTime.Now;

                _context.Users.Add(userNew);
                await _context.SaveChangesAsync();


                foreach (var request in requests)
                {
                    request.UserId = userNew.UserId;
                    request.PatientAccountId = aspnetuserNew.Id.ToString();
                    request.ModifiedDate = DateTime.Now;
                    _context.Update(request);
                }

                await _context.SaveChangesAsync();

                return "account created";
            }
        }

        public async Task<bool> ResetPassword(CreateAccountViewModel Credentials)
        {
            var aspnetuserFetched = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == Credentials.Email);
            if (aspnetuserFetched == null)
            {
                return false;
            }
            aspnetuserFetched.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Credentials.Password);
            aspnetuserFetched.ModifiedDate = DateTime.Now;

            _context.AspNetUsers.Update(aspnetuserFetched);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
