namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class TimesheetViewModel
    {
        public int TimesheetId { get; set; }

        public int PhysicianId { get; set; }

        public string TimesheetStartDate { get; set; } = string.Empty;

        public string TimesheetEndDate { get; set; } = string.Empty;

        public string? CreatedBy { get; set; }

        public List<TimesheetDetailViewModel> TimesheetDetails { get; set; } = new List<TimesheetDetailViewModel>();
    }
}
