using HalloDocEntities.Models;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetViewModelData(int requestStatus, int? physicianId = null);

        PaginatedListViewModel<RequestRowViewModel> GetViewModelData(int requestStatus, int? requestType, string? searchPattern, int? searchRegion, int pageNumber, int? physicianId = null);

        bool CheckValidRequest(int requestId, int physicianId);

        Task<bool> AcceptCase(int requestId);

        ViewCaseViewModel GetViewCaseViewModelData(ViewCaseViewModel CaseInfo);

        Task<bool> UpdateViewCaseInfo(ViewCaseViewModel CaseInfo);

        Task<ViewNotesViewModel> GetViewNotesViewModelData(int requestId);

        Task<bool> AddAdminNote(int requestId, string AdminNotesInput, string createdBy);

        Task<bool> AddNote(ViewNotesViewModel vnvm);

        CancelCaseViewModel GetCancelCaseViewModelData(CancelCaseViewModel CancelCase);

        Task<bool> CancelCase(CancelCaseViewModel CancelCase);

        AssignCaseViewModel GetAssignCaseViewModelData(AssignCaseViewModel AssignCase);

        List<SelectListItem> GetPhysiciansByRegion(int regionId);

        Task<bool> AssignCase(AssignCaseViewModel AssignCase);

        Task<bool> TransferRequest(AssignCaseViewModel TransferRequest);

        //TransferToAdminViewModel GetTransferToAdminData(int requestId);

        Task<bool> TransferToAdmin(TransferToAdminViewModel TransferData);

        Task<BlockRequestViewModel> GetBlockRequestViewModelData(BlockRequestViewModel BlockRequest);

        Task<bool> BlockRequest(BlockRequestViewModel BlockRequest);

        ViewDocumentsViewModel GetViewUploadsViewModelData(ViewDocumentsViewModel ViewUploads);

        Task UploadFiles(IEnumerable<IFormFile> MultipleFiles, int requestId);

        Task<DownloadedFile> DownloadFile(int fileId);

        byte[] GetFilesAsZip(List<int> fileIds, int requestId);

        Task<RequestWiseFile> DeleteFile(int fileId);

        Task<bool> DeleteSelectedFiles(List<int> fileIds, int requestId);

        Task<bool> SendMailWithAttachments(DownloadRequest requestData);

        OrdersViewModel GetOrdersViewModel(int requestId);

        string GetProfessionListOptions();

        string GetVendorListOptions(int professionId);

        OrdersViewModel GetVendorDetails(int vendorId);

        Task<bool> SendOrder(OrdersViewModel Order);

        Task<bool> ClearCase(ClearCaseViewModel ClearCase);

        Task<SendAgreementViewModel> GetSendAgreementViewModelData(SendAgreementViewModel SendAgreementInfo);

        Task<bool> SendAgreementViaMail(SendAgreementViewModel SendAgreementInfo);

        Task<bool> CloseCase(int requestId, int adminId);

        Task<bool> SelectCareType(CareTypeViewModel CareTypeData);

        EncounterFormViewModel GetEncounterFormViewModelData(EncounterFormViewModel EncounterFormDetails);

        Task<bool> UpdateEncounterForm(EncounterFormViewModel EncounterFormDetails);

        Task<bool> Finalize(EncounterFormViewModel EncounterFormDetails);

        Task<bool> HouseCall(int requestId, int physicianId);

        ConcludeCareViewModel GetConcludeCareViewModel(ConcludeCareViewModel ConcludeCareData);

        Task<bool> ConcludeCare(ConcludeCareViewModel ConcludeCareData);

        Task<bool> SendLink(SendLinkViewModel SendLinkData);

        byte[] ExportToExcel(int requestStatus, int? requestType, string? searchPattern, int? searchRegion, int? pageNumber);

        List<SelectListItem> GetRegionList();

        Task<bool> CreateRequest(PatientRequestViewModel PatientInfo);

        Task<bool> RequestSupport(AdminDashboardViewModel SupportMessageData);

        Task<byte[]> GenerateEncounterPdf(DownloadFormViewModel DownloadFormData);

    }
}
