namespace HalloDocServices.ViewModels
{
    
    public class DashboardRequestViewModel
    {

        public int RequestId { get; set; }

        public DateOnly CreateDate { get; set;}

        public short Status { get; set; } 

        public int? Count { get; set; }

        public int? PhysicianId { get; set; }
    }
}
