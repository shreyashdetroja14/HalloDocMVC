using HalloDocEntities.Models;

namespace HalloDocRepository.Interface
{
    public interface IMessageRepository
    {
        Task<MessageDetail> CreateMessageDetail(MessageDetail messageDetail);

        List<MessageDetail> GetMessageDetailList(string senderId, string receiverId);

        List<MessageDetail> GetMessageDetailListBySender(string senderId, string receiverId);

        Task<List<MessageDetail>> UpdateMessageDetails(List<MessageDetail> messageDetails);
    }
}