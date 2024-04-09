using HalloDocEntities.Models;
using HalloDocServices.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface ILoginService
    {
        Task<AspNetUser> CheckLogin(LoginViewModel LoginInfo);

        Task<string> CreateAccount(CreateAccountViewModel Credentials);

        Task<bool> ResetPassword(CreateAccountViewModel Credentials);

        Task<bool> CheckUser(string email);

        Task<bool> SendMail(string receiver, string subject, string message);

        Task<bool> ValidateToken(string token);
    }
}
