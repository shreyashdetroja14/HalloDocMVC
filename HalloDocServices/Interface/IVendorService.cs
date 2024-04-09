using HalloDocServices.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IVendorService
    {
        VendorsViewModel GetVendorsViewModel(VendorsViewModel VendorsViewData);

        List<VendorRowViewModel> GetVendorsList(int professionId, string searchPattern);

        CreateVendorViewModel GetCreateVendorViewModel(CreateVendorViewModel CreateVendorData);

        Task<bool> CreateVendor(CreateVendorViewModel CreateVendorData);

        Task<bool> EditVendor(CreateVendorViewModel EditVendorData);

        Task<bool> DeleteVendor(int vendorId);
    }
}
