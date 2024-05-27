namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class GroupChatBoxViewModel
    {
        public string GroupName { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public List<MessageViewModel> MessageList { get; set; } = new List<MessageViewModel>();
    }
}