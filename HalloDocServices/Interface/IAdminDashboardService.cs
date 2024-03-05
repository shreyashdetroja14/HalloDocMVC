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

        ViewCaseViewModel GetViewCaseViewModelData(int requestId);

        Task<bool> UpdateViewCaseInfo(ViewCaseViewModel CaseInfo);

        Task<ViewNotesViewModel> GetViewNotesViewModelData(int requestId);

        Task<bool> AddAdminNote(int requestId, string AdminNotesInput);

        CancelCaseViewModel GetCancelCaseViewModelData(CancelCaseViewModel CancelCase);

        Task<bool> CancelCase(CancelCaseViewModel CancelCase);

        AssignCaseViewModel GetAssignCaseViewModelData(AssignCaseViewModel AssignCase);

        Task<bool> AssignCase(AssignCaseViewModel AssignCase);

        Task<bool> TransferRequest(AssignCaseViewModel TransferRequest);

        Task<BlockRequestViewModel> GetBlockRequestViewModelData(BlockRequestViewModel BlockRequest);

        Task<bool> BlockRequest(BlockRequestViewModel BlockRequest);

    }
}
