using HalloDocServices.ViewModels.AdminViewModels;

namespace HalloDocServices.Interface
{
    public interface IMessageService
    {
        Task<bool> CreateMessageDetail(MessageViewModel Message);

        Task<bool> UpdateMessageReadStatus(string senderId, string receiverId, bool isRead);
    }
}