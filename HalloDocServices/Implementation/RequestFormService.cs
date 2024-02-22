using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocRepository.Repository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HalloDocServices.Implementation
{
    public class RequestFormService : IRequestFormService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        public RequestFormService(IUserRepository userRepository, IRequestRepository requestRepository) 
        { 
            _userRepository = userRepository;
            _requestRepository = requestRepository;
        }

        #region LocalMethods

        public AspNetUser CreateAspnetuser(PatientRequestViewModel prvm)
        {
            var aspnetuserNew = new AspNetUser();

            aspnetuserNew.Id = Guid.NewGuid().ToString();

            aspnetuserNew.UserName = prvm.Email;
            aspnetuserNew.PasswordHash = BCrypt.Net.BCrypt.HashPassword(prvm.Password);
            aspnetuserNew.Email = prvm.Email;
            aspnetuserNew.PhoneNumber = prvm.PhoneNumber;
            aspnetuserNew.CreatedDate = DateTime.Now;

            return aspnetuserNew;
        }

        public User CreateUser(PatientRequestViewModel prvm, AspNetUser aspnetuserNew)
        {
            var userNew = new User();

            userNew.AspNetUserId = aspnetuserNew.Id;
            userNew.FirstName = prvm.FirstName;
            userNew.LastName = prvm.LastName;
            userNew.Email = prvm.Email;
            userNew.Mobile = prvm.PhoneNumber;
            userNew.Street = prvm.Street;
            userNew.City = prvm.City;
            userNew.State = prvm.State;
            userNew.ZipCode = prvm.ZipCode;

            if (prvm.DOB != null)
            {
                DateTime dateTime = DateTime.ParseExact(prvm.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                userNew.IntYear = dateTime.Year;
                userNew.StrMonth = dateTime.ToString("MMMM");
                userNew.IntDate = dateTime.Day;
            }

            userNew.CreatedBy = "admin";
            userNew.CreatedDate = DateTime.Now;

            return userNew;
        }

        public RequestClient CreateRequestClient(PatientRequestViewModel PatientInfo, Request requestNew)
        {
            var requestClientNew = new RequestClient();
            requestClientNew.RequestId = requestNew.RequestId;
            requestClientNew.FirstName = PatientInfo.FirstName;
            requestClientNew.LastName = PatientInfo.LastName;
            requestClientNew.PhoneNumber = PatientInfo.PhoneNumber;
            requestClientNew.Location = PatientInfo.Room;
            requestClientNew.Address = PatientInfo.Street + ", " + PatientInfo.City + ", " + PatientInfo.State + ", " + PatientInfo.ZipCode;
            requestClientNew.NotiMobile = PatientInfo.PhoneNumber;
            requestClientNew.NotiEmail = PatientInfo.Email;
            requestClientNew.Notes = PatientInfo.Symptoms;
            requestClientNew.Email = PatientInfo.Email;

            if (PatientInfo.DOB != null)
            {
                DateTime dateTime = DateTime.ParseExact(PatientInfo.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                requestClientNew.IntYear = dateTime.Year;
                requestClientNew.StrMonth = dateTime.ToString("MMMM");
                requestClientNew.IntDate = dateTime.Day;
            }

            requestClientNew.Street = PatientInfo.Street;
            requestClientNew.City = PatientInfo.City;
            requestClientNew.State = PatientInfo.State;
            requestClientNew.ZipCode = PatientInfo.ZipCode;
            return requestClientNew;
        }

        public List<string> UploadFilesToServer(IEnumerable<IFormFile> MultipleFiles, int requestid)
        {
            List<string> files = new List<string>();
            foreach (var UploadFile in MultipleFiles)
            {
                string FilePath = "wwwroot\\Upload\\" + requestid;
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";

                string fileNameWithPath = Path.Combine(path, newfilename);
                files.Add(FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + newfilename);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
            }
            return files;
        }

        public static string? encrypt(string email)
        {
            if (email == null)
            {
                return null;
            }
            else
            {
                byte[] storePass = ASCIIEncoding.ASCII.GetBytes(email);
                string emailtoken = Convert.ToBase64String(storePass);
                return emailtoken;
            }

        }
        #endregion

        public async Task<bool> CheckUser(string email)
        {
            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(email);
            if (aspnetuserFetched == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> SendMail(string receiver, string subject, string message)
        {
            var mail = "tatva.dotnet.shreyashdetroja@outlook.com";
            var password = "Dotnet_tatvasoft@14";

            //var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(receiver);

            string? encryptedEmail = encrypt(receiver);
            string mailbody = message + "?emailtoken=" + encryptedEmail;

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
            var aspuserFetched = await _userRepository.GetAspNetUserById(token);
            if (aspuserFetched == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CreatePatientRequest(PatientRequestViewModel prvm)
        {
            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(prvm.Email);
            if (aspnetuserFetched == null)
            {
                var aspnetuserNew = CreateAspnetuser(prvm);
                aspnetuserNew = await _userRepository.CreateAspNetUser(aspnetuserNew);

                var userNew = CreateUser(prvm, aspnetuserNew);
                await _userRepository.CreateUser(userNew);
            }

            aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(prvm.Email);
            var userFetched = await _userRepository.GetUserByAspNetUserId(aspnetuserFetched.Id);
            if (userFetched != null && aspnetuserFetched != null)
            {
                var requestNew = new Request();

                requestNew.RequestTypeId = 2;
                requestNew.UserId = userFetched.UserId;
                requestNew.FirstName = prvm.FirstName;
                requestNew.LastName = prvm.LastName;
                requestNew.PhoneNumber = prvm.PhoneNumber;
                requestNew.Email = prvm.Email;
                requestNew.Status = 1;
                requestNew.CreatedDate = DateTime.Now;
                requestNew.IsUrgentEmailSent = false;
                requestNew.PatientAccountId = aspnetuserFetched?.Id;
                requestNew.CreatedUserId = userFetched.UserId;

                requestNew = await _requestRepository.CreateRequest(requestNew);

                var requestClientNew = CreateRequestClient(prvm, requestNew);

                await _requestRepository.CreateRequestClient(requestClientNew);

                if (prvm.MultipleFiles != null)
                {
                    List<string> files = UploadFilesToServer(prvm.MultipleFiles, requestNew.RequestId);
                    List<RequestWiseFile> requestWiseFiles = new List<RequestWiseFile>();
                    foreach (string file in files)
                    {
                        var reqwisefileNew = new RequestWiseFile();
                        reqwisefileNew.RequestId = requestNew.RequestId;
                        reqwisefileNew.FileName = file;
                        reqwisefileNew.CreatedDate = DateTime.Now;

                        requestWiseFiles.Add(reqwisefileNew);
                    }
                    await _requestRepository.CreateRequestWiseFiles(requestWiseFiles);
                }
            }
            return true;
        }

        public async Task<bool> CreateFamilyRequest(FamilyRequestViewModel frvm)
        {
            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(frvm.PatientInfo.Email);
            var userFetched = await _userRepository.GetUserByEmail(frvm.PatientInfo.Email);
            var requestorUser = await _userRepository.GetUserByEmail(frvm.FamilyEmail);

            var requestNew = new Request();

            requestNew.RequestTypeId = 3;
            requestNew.UserId = userFetched?.UserId;
            requestNew.FirstName = frvm.FamilyFirstName;
            requestNew.LastName = frvm.FamilyLastName;
            requestNew.PhoneNumber = frvm.FamilyPhoneNumber;
            requestNew.Email = frvm.FamilyEmail;
            requestNew.Status = 1;
            requestNew.CreatedDate = DateTime.Now;
            requestNew.IsUrgentEmailSent = false;
            requestNew.RelationName = frvm.FamilyRelation;
            requestNew.PatientAccountId = aspnetuserFetched?.Id;
            requestNew.CreatedUserId = requestorUser?.UserId;

            requestNew = await _requestRepository.CreateRequest(requestNew);

            var requestClientNew = CreateRequestClient(frvm.PatientInfo, requestNew);

            await _requestRepository.CreateRequestClient(requestClientNew);

            if (frvm.PatientInfo.MultipleFiles != null)
            {
                List<string> files = UploadFilesToServer(frvm.PatientInfo.MultipleFiles, requestNew.RequestId);
                List<RequestWiseFile> requestWiseFiles = new List<RequestWiseFile>();
                foreach (string file in files)
                {
                    var reqwisefileNew = new RequestWiseFile();
                    reqwisefileNew.RequestId = requestNew.RequestId;
                    reqwisefileNew.FileName = file;
                    reqwisefileNew.CreatedDate = DateTime.Now;

                    requestWiseFiles.Add(reqwisefileNew);
                }
                await _requestRepository.CreateRequestWiseFiles(requestWiseFiles);
            }

            return true;
        }

        public async Task<bool> CreateConciergeRequest(ConciergeRequestViewModel crvm)
        {
            var conciergeFetched = await _requestRepository.GetRequestByConciergeEmail(crvm.ConciergeEmail);
            if (conciergeFetched == null)
            {
                var conciergeNew = new Concierge();
                conciergeNew.ConciergeName = crvm.ConciergePropertyName;
                conciergeNew.Address = crvm.ConciergeStreet + ", " + crvm.ConciergeCity + ", " + crvm.ConciergeState + ", " + crvm.ConciergeZipCode;
                conciergeNew.Street = crvm.ConciergeStreet;
                conciergeNew.City = crvm.ConciergeCity;
                conciergeNew.State = crvm.ConciergeState;
                conciergeNew.ZipCode = crvm.ConciergeZipCode;
                conciergeNew.CreatedDate = DateTime.Now;
                conciergeNew.RegionId = 1;

                await _userRepository.CreateConcierge(conciergeNew);
            }
            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(crvm.PatientInfo.Email);
            var userFetched = await _userRepository.GetUserByEmail(crvm.PatientInfo.Email);
            var requestorUser = await _userRepository.GetUserByEmail(crvm.ConciergeEmail);

            var requestNew = new Request();

            requestNew.RequestTypeId = 4;
            requestNew.UserId = userFetched?.UserId;
            requestNew.FirstName = crvm.ConciergeFirstName;
            requestNew.LastName = crvm.ConciergeLastName;
            requestNew.PhoneNumber = crvm.ConciergePhoneNumber;
            requestNew.Email = crvm.ConciergeEmail;
            requestNew.Status = 1;
            requestNew.CreatedDate = DateTime.Now;
            requestNew.IsUrgentEmailSent = false;
            requestNew.PatientAccountId = aspnetuserFetched?.Id;
            requestNew.CreatedUserId = requestorUser?.UserId;

            requestNew = await _requestRepository.CreateRequest(requestNew);

            var requestClientNew = CreateRequestClient(crvm.PatientInfo, requestNew);

            await _requestRepository.CreateRequestClient(requestClientNew);

            return true;
        }

        public async Task<bool> CreateBusinessRequest(BusinessRequestViewModel brvm)
        {
            var businessFetched = await _userRepository.GetBusinessByPhoneAndName(brvm.BusinessPhoneNumber, brvm.BusinessName);
            if (businessFetched == null)
            {
                var businessNew = new Business();
                businessNew.Name = brvm.BusinessName;
                businessNew.PhoneNumber = brvm.BusinessPhoneNumber;
                businessNew.CreatedBy = "admin";
                businessNew.CreatedDate = DateTime.Now;

                await _userRepository.CreateBusiness(businessNew);
            }

            var aspnetuserFetched = await _userRepository.GetAspNetUserByEmail(brvm.PatientInfo.Email);
            var userFetched = await _userRepository.GetUserByEmail(brvm.PatientInfo.Email);
            var requestorUser = await _userRepository.GetUserByEmail(brvm.BusinessEmail);

            var requestNew = new Request();

            requestNew.RequestTypeId = 1;
            requestNew.UserId = userFetched?.UserId;
            requestNew.FirstName = brvm.BusinessFirstName;
            requestNew.LastName = brvm.BusinessLastName;
            requestNew.PhoneNumber = brvm.BusinessPhoneNumber;
            requestNew.Email = brvm.BusinessEmail;
            requestNew.Status = 1;
            requestNew.CreatedDate = DateTime.Now;
            requestNew.IsUrgentEmailSent = false;
            requestNew.PatientAccountId = aspnetuserFetched?.Id;
            requestNew.CreatedUserId = requestorUser?.UserId;

            requestNew = await _requestRepository.CreateRequest(requestNew);

            var requestClientNew = CreateRequestClient(brvm.PatientInfo, requestNew);

            await _requestRepository.CreateRequestClient(requestClientNew);


            businessFetched = await _userRepository.GetBusinessByPhoneAndName(brvm.BusinessPhoneNumber, brvm.BusinessName);

            var requestBusinessNew = new RequestBusiness();
            requestBusinessNew.RequestId = requestNew.RequestId;
            if (businessFetched != null)
            {
                requestBusinessNew.BusinessId = businessFetched.BusinessId;
            }

            await _requestRepository.CreateRequestBusiness(requestBusinessNew);

            return true;
        }
    }
}
