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

    public enum WeekDays
    {
        Sunday = 1,
        Monday = 2,
        Tuesday = 3,
        Wednesday = 4,
        Thursday = 5,
        Friday = 6,
        Saturday = 7,
    }

    public enum Months
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12,
    }

    public enum ShiftStatus
    {
        Unapproved = 0,
        Approved = 1,
        Deleted = 2,
    }
}
