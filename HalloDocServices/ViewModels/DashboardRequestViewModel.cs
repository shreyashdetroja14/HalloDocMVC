namespace HalloDocServices.ViewModels
{

    public class DashboardRequestViewModel
    {

        public int RequestId { get; set; }

        public DateOnly CreateDate { get; set; }

        public short Status { get; set; }

        public int? Count { get; set; }

        public string PhysicianName { get; set; } = null!;

        public string PhysicianAspNetUserId { get; set; } = string.Empty;

        public string AdminAspNetUserId { get; set; } = string.Empty;
    }
}
