using HalloDocEntities.Models;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
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

        Task<ViewDocumentsViewModel> GetViewUploadsViewModelData(ViewDocumentsViewModel ViewUploads);

        Task UploadFiles(IEnumerable<IFormFile> MultipleFiles, int requestId);

        Task<DownloadedFile> DownloadFile(int fileId);

        byte[] GetFilesAsZip(List<int> fileIds, int requestId);

        Task<RequestWiseFile> DeleteFile(int fileId);

        Task<bool> DeleteSelectedFiles(List<int> fileIds, int requestId);

        Task<bool> SendMailWithAttachments(DownloadRequest requestData);
    }
}
