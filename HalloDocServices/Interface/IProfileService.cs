﻿using HalloDocServices.ViewModels.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IProfileService
    {
        AdminProfileViewModel GetAdminProfileViewModelData(string aspnetuserId);
    }
}