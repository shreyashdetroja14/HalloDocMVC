namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class TimesheetDetailViewModel
    {
        public int TimesheetDetailId { get; set; }

        public string? TimesheetDetailDate { get; set; }

        public int ShiftCount { get; set; }

        public int OnCallHours { get; set; }

        public int TotalHours { get; set; }

        public bool IsNightWeekend { get; set; }

        public int? HousecallsCount { get; set; }

        public int? PhoneconsultCount { get; set; }

    }
}
