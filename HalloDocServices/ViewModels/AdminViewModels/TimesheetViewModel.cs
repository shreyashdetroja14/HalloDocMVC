namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class TimesheetViewModel
    {
        public int TimesheetId { get; set; }

        public int PhysicianId { get; set; }

        public string TimesheetStartDate { get; set; } = string.Empty;

        public string TimesheetEndDate { get; set; } = string.Empty;

        public bool IsFinalized { get; set; }

        public bool IsApproved { get; set; }

        public int BonusAmount { get; set; }

        public string? AdminDescription { get; set; }

        public string? CreatedBy { get; set; }

        public string? AspNetUserRole { get; set; }

        public string SelectedDatePeriod { get; set; } = string.Empty;

        public List<TimesheetDetailViewModel> TimesheetDetails { get; set; } = new List<TimesheetDetailViewModel>();

        public List<TimesheetReceiptViewModel> TimesheetReceipts { get; set; } = new List<TimesheetReceiptViewModel>();

        public Dictionary<int, int> PayrateTotals { get; set; } = new Dictionary<int, int>();
    }
}
