namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class MessageViewModel
    {
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string? SenderRole { get; set; }
        public string? SenderName { get; set; }
        public string? ReceiverRole { get; set; }
        public string? ReceiverName { get; set; }
        public string? Message { get; set; }
        public string SentTime { get; set; } = string.Empty;
        public bool IsRead { get; set; }

    }
}