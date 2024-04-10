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

        PaginatedListViewModel<RequestRowViewModel> GetViewModelData(int requestStatus, int? requestType, string? searchPattern, int? searchRegion, int pageNumber);

        ViewCaseViewModel GetViewCaseViewModelData(ViewCaseViewModel CaseInfo);

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

        ViewDocumentsViewModel GetViewUploadsViewModelData(ViewDocumentsViewModel ViewUploads);

        Task UploadFiles(IEnumerable<IFormFile> MultipleFiles, int requestId);

        Task<DownloadedFile> DownloadFile(int fileId);

        byte[] GetFilesAsZip(List<int> fileIds, int requestId);

        Task<RequestWiseFile> DeleteFile(int fileId);

        Task<bool> DeleteSelectedFiles(List<int> fileIds, int requestId);

        Task<bool> SendMailWithAttachments(DownloadRequest requestData);

        string GetProfessionListOptions();

        string GetVendorListOptions(int professionId);

        OrdersViewModel GetVendorDetails(int vendorId);

        Task<bool> SendOrder(OrdersViewModel Order);

        Task<bool> ClearCase(ClearCaseViewModel ClearCase);

        Task<SendAgreementViewModel> GetSendAgreementViewModelData(SendAgreementViewModel SendAgreementInfo);

        Task<bool> SendAgreementViaMail(SendAgreementViewModel SendAgreementInfo);

        Task<bool> CloseCase(int requestId, int adminId);

        EncounterFormViewModel GetEncounterFormViewModelData(EncounterFormViewModel EncounterFormDetails);

        Task<bool> UpdateEncounterForm(EncounterFormViewModel EncounterFormDetails);

        Task<bool> SendLink(string receiverEmail);
    }
}
