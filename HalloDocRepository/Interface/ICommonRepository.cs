﻿using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface ICommonRepository
    {
        List<CaseTag> GetAllCaseTags();

        List<Region> GetAllRegions();
    }
}