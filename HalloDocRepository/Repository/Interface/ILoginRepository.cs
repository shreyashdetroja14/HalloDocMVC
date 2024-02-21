using HalloDocEntities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Repository.Interface
{
    public interface ILoginRepository
    {
        Task<String> CheckLogin(LoginViewModel LoginInfo);

        Task<string> CreateAccount(CreateAccountViewModel Credentials);

        Task<bool> ResetPassword(CreateAccountViewModel Credentials);
    }
}
