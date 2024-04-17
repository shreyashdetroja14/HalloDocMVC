﻿using HalloDocMVC.Auth;
using HalloDocServices.Constants;
using HalloDocServices.Implementation;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDocMVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IJwtService _jwtService;
        private readonly IProfileService _profileService;
        private readonly IProvidersService _providersService;

        public ProfileController(IJwtService jwtService, IProfileService profileService, IProvidersService providersService)
        {
            _jwtService = jwtService;
            _profileService = profileService;
            _providersService = providersService;
        }

        [Route("Profile", Name = "Profile")]
        [CustomAuthorize("admin", "physician")]
        [RoleAuthorize("AdminProfile")]
        public IActionResult Index()
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            if(claimsData.AspNetUserRole == "admin")
            {
                AdminProfileViewModel AdminProfileDetails = _profileService.GetAdminProfileViewModelData(claimsData.AspNetUserId??"");
                AdminProfileDetails.IsEditAdmin = false;
                return View(AdminProfileDetails);
            }
            else
            {
                EditProviderViewModel PhysicianProfileDetails = new EditProviderViewModel();
                PhysicianProfileDetails.ProviderId = claimsData.Id;
                PhysicianProfileDetails.IsAccessProvider = false;
                PhysicianProfileDetails = _providersService.GetEditProviderViewModel(PhysicianProfileDetails);

                return View("~/Views/Profile/PhysicianProfile.cshtml", PhysicianProfileDetails);

            }
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> ResetPassword(AdminProfileViewModel AdminProfileDetails)
        {

            if(AdminProfileDetails.Password == null)
            {
                TempData["ErrorMessage"] = "Unable to reset password";
                return RedirectToAction("Index");
            }

            bool isPasswordReset = await _profileService.ResetPassword(AdminProfileDetails);
            if(isPasswordReset)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to reset password";

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> EditAccountInfo(AdminProfileViewModel AdminProfileDetails)
        {

            bool isAdminInfoUpdated = await _profileService.UpdateAccountInfo(AdminProfileDetails);
            if (isAdminInfoUpdated)
            {
                TempData["SuccessMessage"] = "Account Info Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable To Update Account Info";

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> EditAdminInfo(AdminProfileViewModel AdminProfileDetails)
        {
            bool isAdminInfoUpdated = await _profileService.UpdateAdminInfo(AdminProfileDetails);
            if (isAdminInfoUpdated)
            {
                TempData["SuccessMessage"] = "Admin Info Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable To Update Info";

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthorize("admin")]
        public async Task<IActionResult> EditBilling(AdminProfileViewModel AdminProfileDetails)
        {
            bool isBillingInfoUpdated = await _profileService.UpdateBillingInfo(AdminProfileDetails);
            if (isBillingInfoUpdated)
            {
                TempData["SuccessMessage"] = "Mailing & Billing Info Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable To Update Info";

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Physician/ResetPassword", Name ="ResetPasswordPhysician")]
        [CustomAuthorize("physician")]
        public async Task<IActionResult> ResetPassword(EditProviderViewModel AccountInfo)
        {

            if (AccountInfo.Password == null)
            {
                TempData["ErrorMessage"] = "Unable to reset password";
                return RedirectToRoute("Profile");
            }

            bool isPasswordReset = await _providersService.ResetPassword(AccountInfo);
            if (isPasswordReset)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to reset password";

            }

            return RedirectToRoute("Profile");
        }

        [HttpPost]
        [Route("Physician/ProfileInfo", Name = "EditProfileInfoPhysician")]
        [CustomAuthorize("physician")]
        public async Task<IActionResult> EditProfileInfo(EditProviderViewModel ProfileInfo)
        {
            bool isInfoUpdated = await _providersService.EditProfileInfo(ProfileInfo);
            if (isInfoUpdated)
            {
                TempData["SuccessMessage"] = "Profile Info Updated Successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Update Profile Info.";
            }
            return RedirectToRoute("Profile");
        }

        [HttpPost]
        [CustomAuthorize("physician")]
        public async Task<IActionResult> RequestAdmin(EditProviderViewModel MailDetails)
        {
            bool isMailSent = await _profileService.SendMailToAdmin(MailDetails);
            if (isMailSent)
            {
                TempData["SuccessMessage"] = "Mail Sent Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Send Mail. Try Again.";
            }
            return RedirectToRoute("Profile");
        }
    }
}
