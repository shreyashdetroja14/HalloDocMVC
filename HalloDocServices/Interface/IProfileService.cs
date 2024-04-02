using HalloDocServices.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IProfileService
    {
        AdminProfileViewModel GetAdminProfileViewModelData(string aspnetuserId);

        Task<bool> ResetPassword(AdminProfileViewModel AdminProfileDetails);

        Task<bool> UpdateAccountInfo(AdminProfileViewModel AdminProfileDetails);

        Task<bool> UpdateBillingInfo(AdminProfileViewModel AdminProfileDetails);

        Task<bool> UpdateAdminInfo(AdminProfileViewModel AdminProfileDetails);
    }
}
