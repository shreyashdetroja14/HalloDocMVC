using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Implementation
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ICommonRepository _commonRepository;

        public VendorService(IVendorRepository vendorRepository, ICommonRepository commonRepository)
        {
            _vendorRepository = vendorRepository;
            _commonRepository = commonRepository;
        }

        public VendorsViewModel GetVendorsViewModel(VendorsViewModel VendorsViewData)
        {
            var healthProfessionTypes = _vendorRepository.GetAllHealthProfessionTypes();

            foreach (var type in healthProfessionTypes)
            {
                VendorsViewData.ProfessionList.Add(new SelectListItem
                {
                    Value = type.HealthProfessionId.ToString(),
                    Text = type.ProfessionName
                });
            }

            return VendorsViewData;
        }

        public List<VendorRowViewModel> GetVendorsList(int professionId, string searchPattern)
        {
            var vendors = _vendorRepository.GetIQueryableVendors().Where(x => x.IsDeleted != true && (professionId == 0 || x.ProfessionId == professionId) && (searchPattern == "" || EF.Functions.ILike(x.VendorName, "%" + searchPattern + "%")));

            List<VendorRowViewModel> VendorsList = vendors.Select(x => new VendorRowViewModel
            {
                VendorId = x.VendorId,
                VendorName = x.VendorName,
                ProfessionId = x.ProfessionId ?? 0,
                ProfessionName = x.Profession != null ? x.Profession.ProfessionName : string.Empty,
                Email = x.Email,
                FaxNumber = x.FaxNumber,
                PhoneNumber = x.PhoneNumber,
                BusinessContact = x.BusinessContact
            }).ToList();

            return VendorsList;
        }

        public CreateVendorViewModel GetCreateVendorViewModel(CreateVendorViewModel CreateVendorData)
        {
            var healthProfessionTypes = _vendorRepository.GetAllHealthProfessionTypes();

            foreach (var type in healthProfessionTypes)
            {
                CreateVendorData.ProfessionList.Add(new SelectListItem
                {
                    Value = type.HealthProfessionId.ToString(),
                    Text = type.ProfessionName
                });
            }

            var regions = _commonRepository.GetAllRegions();

            foreach (var region in regions)
            {
                CreateVendorData.RegionList.Add(new SelectListItem
                {
                    Value = region.RegionId.ToString(),
                    Text = region.Name
                });
            }

            HealthProfessional vendor = _vendorRepository.GetVendorById(CreateVendorData.VendorId);
            if(vendor.VendorId != 0)
            {
                CreateVendorData.VendorId = vendor.VendorId;
                CreateVendorData.VendorName = vendor.VendorName;
                CreateVendorData.ProfessionId = vendor.ProfessionId ?? 0;
                CreateVendorData.FaxNumber = vendor.FaxNumber;
                CreateVendorData.Email = vendor.Email;
                CreateVendorData.PhoneNumber = vendor.PhoneNumber;
                CreateVendorData.BusinessContact = vendor.BusinessContact;
                CreateVendorData.Street = vendor.Address;
                CreateVendorData.City = vendor.City;
                CreateVendorData.RegionId = vendor.RegionId ?? 0;
                CreateVendorData.ZipCode = vendor.ZipCode;
            }

            return CreateVendorData;
        }

        public async Task<bool> CreateVendor(CreateVendorViewModel CreateVendorData)
        {
            HealthProfessional vendor = new HealthProfessional();

            vendor.VendorName = CreateVendorData.VendorName;
            vendor.ProfessionId = CreateVendorData.ProfessionId;
            vendor.FaxNumber = CreateVendorData.FaxNumber;
            vendor.Email = CreateVendorData.Email;
            vendor.PhoneNumber = CreateVendorData.PhoneNumber;
            vendor.BusinessContact = CreateVendorData.BusinessContact;
            vendor.City = CreateVendorData.City;
            vendor.State = _commonRepository.GetRegionById(CreateVendorData.RegionId).Name;
            vendor.RegionId = CreateVendorData.RegionId;
            vendor.ZipCode = CreateVendorData.ZipCode;
            vendor.Address = CreateVendorData.Street;
            //vendor.Address = GetAddress(CreateVendorData.Street, vendor.City, vendor.State, vendor.ZipCode);
            vendor.CreatedDate = DateTime.Now;
            vendor.IsDeleted = false;

            await _vendorRepository.CreateHealthProfessional(vendor);

            return true;
        }

        string GetAddress(string? street, string? city, string? state, string? zipCode)
        {
            string address = string.Empty;

            if (street != null)
            {
                address += street + ",";
            }
            if (city != null)
            {
                address += city + ",";
            }
            if(state != null)
            {
                address += state + ",";
            }
            if (zipCode != null)
            {
                address += zipCode + ",";
            }

            address = address.Remove(address.Length - 1, 1);

            return address;
        }

        public async Task<bool> EditVendor(CreateVendorViewModel EditVendorData)
        {
            HealthProfessional vendor = _vendorRepository.GetVendorById(EditVendorData.VendorId);

            if(vendor.VendorId == 0)
            {
                return false;
            }

            vendor.VendorName = EditVendorData.VendorName;
            vendor.ProfessionId = EditVendorData.ProfessionId;
            vendor.FaxNumber = EditVendorData.FaxNumber;
            vendor.Email = EditVendorData.Email;
            vendor.PhoneNumber = EditVendorData.PhoneNumber;
            vendor.BusinessContact = EditVendorData.BusinessContact;
            vendor.City = EditVendorData.City;
            vendor.State = _commonRepository.GetRegionById(EditVendorData.RegionId).Name;
            vendor.RegionId = EditVendorData.RegionId;
            vendor.ZipCode = EditVendorData.ZipCode;
            vendor.Address = EditVendorData.Street;
            //vendor.Address = GetAddress(CreateVendorData.Street, vendor.City, vendor.State, vendor.ZipCode);
            vendor.ModifiedDate = DateTime.Now;
            vendor.IsDeleted = false;

            await _vendorRepository.UpdateHealthProfessional(vendor);

            return true;
        }

        public async Task<bool> DeleteVendor(int vendorId)
        {
            HealthProfessional vendor = _vendorRepository.GetVendorById(vendorId);

            if (vendor.VendorId == 0)
            {
                return false;
            }

            vendor.IsDeleted = true;

            await _vendorRepository.UpdateHealthProfessional(vendor);

            return true;
        }
    }
}
