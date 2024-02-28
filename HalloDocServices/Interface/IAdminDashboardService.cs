using HalloDocServices.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetViewModelData(int requestStatus);

        List<RequestRowViewModel> GetViewModelData(int requestStatus, int? requestType, string? searchPattern, int? searchRegion);
    }
}
