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

    public enum OnCallStatus
    {
        Unavailable = 0,
        Available = 1
    }

    public enum RequestTypes
    {
        Business = 1,
        Patient = 2,
        Family = 3,
        Concierge = 4
    }

    public enum DashboardRequestStatus
    {
        New = 1,
        Pending = 2,
        Active = 3,
        Conclude = 4,
        ToClose = 5,
        Unpaid = 6,
        Clear = 7,
        Blocked = 8,
    }

    public enum RequestStatus
    {
        Unassigned = 1,
        Accepted = 2,
        Cancelled = 3,
        MDEnRoute = 4,
        MDONSite = 5,
        Conclude = 6,
        CancelledByPatient = 7,
        Closed = 8,
        Unpaid = 9,
        Clear = 10,
        Blocked = 11,

    }

    public enum CareType
    {
        HouseCall = 1,
        Consult = 2,
        Virtual = 3
    }
 
    public enum UserType
    {
        admin = 1, 
        physician = 2
    }

    public enum ActionEnum
    {
        Anonymous = 0,
        NotMentioned = 1,
        CreateAccount = 2,
        ChangePassword = 3,
        ReviewAgreement = 4,
        Attachments = 5,
        NewRequest = 6,
        AdminMessage = 7,
        ResetPassword = 8,
        ProfileUpdateMessage = 9,
        SendLink = 10,
    }
}
