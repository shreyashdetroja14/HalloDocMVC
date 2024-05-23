namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class ChatBoxViewModel
    {
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string? ReceiverName { get; set; }
        public List<MessageViewModel> MessageList { get; set; } = new List<MessageViewModel>();
    }
}