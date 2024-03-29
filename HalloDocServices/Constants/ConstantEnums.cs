using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Constants
{
    public enum Status
    {
        Active = 1,
        Inactive = 2,
        Pending = 3
    }

    public enum AccountType
    {
        Admin = 1,
        Physician = 2,
        Patient = 3,
    }
}
