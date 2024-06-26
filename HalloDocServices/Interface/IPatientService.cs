﻿using HalloDocEntities.Models;
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IPatientService
    {
        List<SelectListItem> GetRegionList();

        Task<int> CheckUser(string id);

        Task<List<DashboardRequestViewModel>> GetRequestList(int userId);

        Task<ViewDocumentsViewModel> GetRequestFiles(int requestId);

        Task UploadFiles(IEnumerable<IFormFile> MultipleFiles, int requestId);

        byte[] GetFilesAsZip(List<int> fileIds, int requestId);

        Task<int> GetUserIdByRequestId(int requestId);

        Task<string> GetAspNetUserIdByUserId(int userId);

        Task<RequestWiseFile> RequestFileData(int fileId);

        Task<ProfileViewModel> GetProfileDetails(int userId);

        Task<bool> EditProfile(ProfileViewModel ProfileDetails);

        Task<PatientRequestViewModel> GetPatientInfo(int userId);

        Task<bool> CreatePatientRequest(FamilyRequestViewModel frvm);

        Task<bool> CreateFamilyRequest(FamilyRequestViewModel frvm, int userId);

        Task<AgreementViewModel> GetAgreementViewModelData(AgreementViewModel AgreementInfo);

        Task<bool> AcceptAgreement(AgreementViewModel AgreementInfo);

        Task<bool> CancelAgreement(AgreementViewModel CancelAgreementInfo);

        bool CheckValidRequest(int requestId, int userId);
    }
}
