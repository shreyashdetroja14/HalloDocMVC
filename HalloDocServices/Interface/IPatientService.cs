using HalloDocEntities.Models;
using HalloDocServices.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IPatientService
    {
        Task<int> CheckUser(string id);

        Task<List<DashboardRequestViewModel>> GetRequestList(int userId);
    }
}
