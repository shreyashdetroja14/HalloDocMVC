using System.Globalization;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels.AdminViewModels;

namespace HalloDocServices.Implementation
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<bool> CreateMessageDetail(MessageViewModel Message)
        {
            MessageDetail messageDetail = new MessageDetail
            {
                SenderId = Message.SenderId,
                ReceiverId = Message.ReceiverId,
                MessageText = Message.Message,
                SentTime = DateTime.ParseExact(Message.SentTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                IsRead = Message.IsRead,
            };

            await _messageRepository.CreateMessageDetail(messageDetail);

            return true;
        }

        public async Task<bool> UpdateMessageReadStatus(string senderId, string receiverId, bool isRead)
        {
            List<MessageDetail> messageDetails = _messageRepository.GetMessageDetailListBySender(senderId, receiverId);
            foreach (var messageDetail in messageDetails)
            {
                messageDetail.IsRead = isRead;
            }

            await _messageRepository.UpdateMessageDetails(messageDetails);

            return true;
        }
    }
}