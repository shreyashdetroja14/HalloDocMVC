﻿using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocRepository.Interface
{
    public interface IPhysicianRepository
    {
        Task<List<Physician>> GetAllPhysicians();
    }
}
