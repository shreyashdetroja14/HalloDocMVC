using HalloDocMVC.Auth;
using HalloDocServices.Implementation;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers
{
    [CustomAuthorize("admin")]
    public class VendorsController : Controller
    {
        private readonly IVendorService _vendorService;

        public VendorsController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        [RoleAuthorize("Vendors")]
        public IActionResult Index()
        {
            VendorsViewModel VendorsViewData = new VendorsViewModel();
            VendorsViewData = _vendorService.GetVendorsViewModel(VendorsViewData);

            return View(VendorsViewData);
        }

        public IActionResult GetVendorsList(int professionId, string searchPattern)
        {
            List<VendorRowViewModel> VendorsList = _vendorService.GetVendorsList(professionId, searchPattern);
            return PartialView("_VendorsListPartial", VendorsList);
        }

        public IActionResult CreateVendor()
        {
            CreateVendorViewModel CreateVendorData = new CreateVendorViewModel();
            CreateVendorData.IsEditVendor = false;
            CreateVendorData = _vendorService.GetCreateVendorViewModel(CreateVendorData);

            return View("~/Views/Vendors/CreateEditVendor.cshtml", CreateVendorData);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVendor(CreateVendorViewModel CreateVendorData)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Bad Request. Enter Proper Details";

                var selectlists = _vendorService.GetCreateVendorViewModel(CreateVendorData);
                CreateVendorData.ProfessionList = selectlists.ProfessionList;
                CreateVendorData.RegionList = selectlists.RegionList;

                return View("~/Views/Vendors/CreateEditVendor.cshtml", CreateVendorData);
            }

            bool isVendorCreated = await _vendorService.CreateVendor(CreateVendorData);
            if (isVendorCreated)
            {
                TempData["SuccessMessage"] = "Vendor Created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Create Vendor";
                return RedirectToAction("CreateVendor");
            }
        }

        public IActionResult EditVendor(int vendorId)
        {
            CreateVendorViewModel EditVendorData = new CreateVendorViewModel();
            EditVendorData.VendorId = vendorId;
            EditVendorData.IsEditVendor = true;
            EditVendorData = _vendorService.GetCreateVendorViewModel(EditVendorData);

            return View("~/Views/Vendors/CreateEditVendor.cshtml", EditVendorData);
        }

        [HttpPost]
        public async Task<IActionResult> EditVendor(CreateVendorViewModel EditVendorData)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Bad Request. Enter Proper Details";

                var selectlists = _vendorService.GetCreateVendorViewModel(EditVendorData);
                EditVendorData.ProfessionList = selectlists.ProfessionList;
                EditVendorData.RegionList = selectlists.RegionList;

                return View("~/Views/Vendors/CreateEditVendor.cshtml", EditVendorData);
            }

            bool isVendorEdited = await _vendorService.EditVendor(EditVendorData);
            if (isVendorEdited)
            {
                TempData["SuccessMessage"] = "Vendor Edited successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Edit Vendor";
                return RedirectToAction("EditVendor", new { vendorId = EditVendorData.VendorId });
            }
        }

        public async Task<IActionResult> DeleteVendor(int vendorId)
        {
            bool isVendorDeleted = await _vendorService.DeleteVendor(vendorId);
            if (isVendorDeleted)
            {
                TempData["SuccessMessage"] = "Vendor Deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to Delete Vendor";
            }
                return RedirectToAction("Index");
        }
    }
}
