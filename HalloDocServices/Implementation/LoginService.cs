using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

//G:\test\hallodoc3tier\HalloDocMVC\HalloDocMVC.sln

namespace HalloDocServices.Implementation
{
    public class LoginService : ILoginService
    {
        private readonly HalloDocContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IPhysicianRepository _physicianRepository;

        public LoginService(HalloDocContext context, IUserRepository userRepository, IRequestRepository requestRepository, IPhysicianRepository physicianRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _physicianRepository = physicianRepository;
        }
        public async Task<AspNetUser> CheckLogin(LoginViewModel LoginInfo)
        {
            //string id = "";

            //var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(LoginInfo.Email);
            var aspnetuserIQ = _userRepository.GetIQueryableAspNetUserByEmail(LoginInfo.Email);
            var aspnetuserFetched = aspnetuserIQ.FirstOrDefault();

            if (aspnetuserFetched != null)
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(LoginInfo.Password);
                if (BCrypt.Net.BCrypt.Verify(LoginInfo.Password, aspnetuserFetched.PasswordHash))
                {
                    if (aspnetuserFetched.PhysicianAspNetUsers.Any())
                    {
                        var physician = aspnetuserFetched.PhysicianAspNetUsers.FirstOrDefault();
                        var location = _physicianRepository.GetPhysicianLocationByPhysicianId(physician?.PhysicianId ?? 0);
                        if(location.LocationId != 0)
                        {
                            location.Latitude = LoginInfo.Latitude; 
                            location.Longitude = LoginInfo.Longitude;

                            await _physicianRepository.UpdatePhysicianLocation(location);
                        }
                    }
                    

                    return aspnetuserFetched;
                }
            }
            return new AspNetUser();
        }

        public async Task<string> CreateAccount(CreateAccountViewModel Credentials)
        {
            var aspuserFetched = await _userRepository.GetAspNetUserByEmail(Credentials.Email);
            if (aspuserFetched != null)
            {
                return "user exists";
            }

            var requestClientFetched = await _requestRepository.GetLastRequestClient(Credentials.Email);
            if (requestClientFetched == null)
            {
                return "not eligible";
            }
            else
            {
                var aspnetuserNew = new AspNetUser();
                var userNew = new User();
                var requestClientsFetched = await _requestRepository.GetRequestsClientsByEmail(Credentials.Email);
                var requestsFetched = await _requestRepository.GetAllRequests();

                List<Request> requestsForClient = new List<Request>();
                if (requestsFetched != null)
                {
                    requestClientsFetched.ForEach(item =>
                    {
                        var request = requestsFetched.FirstOrDefault(x => x.RequestId == item.RequestId);
                        if (request != null)
                        {
                            requestsForClient.Add(request);

                        }
                    });
                }

                aspnetuserNew.Id = Guid.NewGuid().ToString();
                aspnetuserNew.UserName = Credentials.Email;
                aspnetuserNew.Email = Credentials.Email;
                aspnetuserNew.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Credentials.Password);
                aspnetuserNew.CreatedDate = DateTime.Now;

                await _userRepository.CreateAspNetUser(aspnetuserNew);

                var aspnetuserRoleNew = new AspNetUserRole();
                aspnetuserRoleNew.AspNetUserId = aspnetuserNew.Id;
                aspnetuserRoleNew.RoleId = "c5169b0d-e9f6-4c6f-8f98-a7b8d4a95158";

                await _userRepository.CreateAspNetUserRole(aspnetuserRoleNew);

                userNew.AspNetUserId = aspnetuserNew.Id;
                userNew.Email = aspnetuserNew.Email;
                userNew.FirstName = requestClientFetched.FirstName;
                userNew.LastName = requestClientFetched?.LastName;
                userNew.Mobile = requestClientFetched?.PhoneNumber;
                userNew.Street = requestClientFetched?.Street;
                userNew.City = requestClientFetched?.City;
                userNew.State = requestClientFetched?.State;
                userNew.ZipCode = requestClientFetched?.ZipCode;
                userNew.RegionId = requestClientFetched?.RegionId;
                userNew.StrMonth = requestClientFetched?.StrMonth;
                userNew.IntDate = requestClientFetched?.IntDate;
                userNew.IntYear = requestClientFetched?.IntYear;
                userNew.CreatedDate = DateTime.Now;

                userNew = await _userRepository.CreateUser(userNew);

                requestsForClient.ForEach(request =>
                {
                    request.UserId = userNew.UserId;
                    request.PatientAccountId = aspnetuserNew.Id.ToString();
                    request.ModifiedDate = DateTime.Now;
                });

                bool status = await _requestRepository.UpdateRequests(requestsForClient);
                

                return "account created";
            }
        }

        public async Task<bool> ResetPassword(CreateAccountViewModel Credentials)
        {
            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(Credentials.Email);
            if (aspnetuserFetched == null)
            {
                return false;
            }
            aspnetuserFetched.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Credentials.Password);
            aspnetuserFetched.ModifiedDate = DateTime.Now;

            await _userRepository.UpdateAspNetUser(aspnetuserFetched);

            return true;
        }

        public async Task<bool> CheckUser(string email)
        {
            var aspnetuser = await _userRepository.GetAspNetUserByEmail(email);
            if (aspnetuser == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SendMail(string receiver, string subject, string message)
        {
            var mail = "tatva.dotnet.shreyashdetroja@outlook.com";
            var password = "Dotnet_tatvasoft@14";

            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(receiver);

            string mailbody = message + "?email=" + receiver + "&token=" + aspnetuserFetched.Id;

            var client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            await client.SendMailAsync(new MailMessage(from: mail, to: receiver, subject, mailbody));

            return true;
        }

        public async Task<bool> ValidateToken(string token)
        {
            var aspuserFetched = await _userRepository.GetAspNetUserByIdAsync(token);
            if(aspuserFetched == null)
            {
                return false;
            }
            return true;
        }
    }
}
