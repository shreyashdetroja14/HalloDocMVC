using HalloDocEntities.Data;
using HalloDocEntities.Models;
using HalloDocRepository.Interface;

namespace HalloDocRepository.Implementation
{
    public class MessageRepository : IMessageRepository
    {
        private readonly HalloDocContext _context;
        public MessageRepository(HalloDocContext context)
        {
            _context = context;
        }
        public async Task<MessageDetail> CreateMessageDetail(MessageDetail messageDetail)
        {
            _context.MessageDetails.Add(messageDetail);
            await _context.SaveChangesAsync();

            return messageDetail;
        }

        public List<MessageDetail> GetMessageDetailList(string senderId, string receiverId)
        {
            return _context.MessageDetails.Where(x => (x.SenderId == senderId && x.ReceiverId == receiverId) || (x.SenderId == receiverId && x.ReceiverId == senderId)).OrderBy(x => x.MessageId).ToList();
        }

        public List<MessageDetail> GetMessageDetailListBySender(string senderId, string receiverId)
        {
            return _context.MessageDetails.Where(x => x.SenderId == senderId && x.ReceiverId == receiverId).OrderBy(x => x.MessageId).ToList();
        }

        public async Task<List<MessageDetail>> UpdateMessageDetails(List<MessageDetail> messageDetails)
        {
            _context.MessageDetails.UpdateRange(messageDetails);
            await _context.SaveChangesAsync();

            return messageDetails;
        }
    }
}