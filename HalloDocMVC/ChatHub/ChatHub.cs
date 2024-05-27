using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.SignalR;

namespace HalloDocMVC.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly IJwtService _jwtService;
        private readonly IMessageService _messageService;
        public static Dictionary<string, string> ConnectionsStorage = new Dictionary<string, string>();
        public ChatHub(IJwtService jwtService, IMessageService messageService)
        {
            _jwtService = jwtService;
            _messageService = messageService;
        }
        public override async Task OnConnectedAsync()
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            string connectionId = Context.ConnectionId;

            ConnectionsStorage.Add(claimsData.AspNetUserId ?? "", connectionId);
            //Groups.AddToGroupAsync("testname", connectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            ConnectionsStorage.Remove(claimsData.AspNetUserId ?? "");
        }

        public async Task SendMessage(string receiverId, string receiverName, string message, string sentTime)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            string connectionId = ConnectionsStorage.FirstOrDefault(x => x.Key == receiverId).Value;

            MessageViewModel MessageDetails = new MessageViewModel();
            MessageDetails.SenderId = claimsData.AspNetUserId ?? "";
            MessageDetails.SenderName = claimsData.Username;
            MessageDetails.ReceiverId = receiverId;
            MessageDetails.ReceiverName = receiverName;
            MessageDetails.Message = message;
            MessageDetails.SentTime = sentTime;
            MessageDetails.IsRead = false;

            await _messageService.CreateMessageDetail(MessageDetails);

            if (connectionId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", MessageDetails);
            }
            else
            {
                string senderConnectionId = ConnectionsStorage.FirstOrDefault(x => x.Key == MessageDetails.SenderId).Value;
                await Clients.Client(senderConnectionId).SendAsync("SendNotification", MessageDetails);
            }
            //await Clients.Group(connectionId).SendAsync("ReceiveMessage", MessageDetails);
        }

        public async Task CheckReadStatus(string receiverId)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            string senderId = claimsData.AspNetUserId ?? "";
            string receiverConnectionId = ConnectionsStorage.FirstOrDefault(x => x.Key == receiverId).Value;
            string senderConnectionId = ConnectionsStorage.FirstOrDefault(x => x.Key == senderId).Value;
            if (receiverConnectionId == null)
            {
                //await Clients.Client(senderConnectionId).SendAsync("UpdateReadStatus", false);
                await UpdateReadStatus(senderId, receiverId, false);
            }
            else
            {
                await Clients.Client(receiverConnectionId).SendAsync("CheckReadStatus", senderId);
            }
        }

        public async Task UpdateReadStatus(string senderId, string receiverId, bool isRead)
        {
            string senderConnectionId = ConnectionsStorage.FirstOrDefault(x => x.Key == senderId).Value;
            if (isRead)
            {
                await _messageService.UpdateMessageReadStatus(senderId, receiverId, isRead);
            }
            if (senderConnectionId != null)
            {
                await Clients.Client(senderConnectionId).SendAsync("UpdateReadStatus", isRead);
            }
        }

        public async Task AddToGroup(string groupname)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupname);

            ClaimsData claimsData = _jwtService.GetClaimValues();
            await Clients.Group(groupname).SendAsync("Announcement", $"{claimsData.Username} has joined the group {groupname}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Announcement", $"{Context.ConnectionId} has left the group {groupName}.");
        }

        public async Task SendGroupMessage(string groupname, string message)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();

            MessageViewModel MessageDetails = new MessageViewModel();
            MessageDetails.SenderId = claimsData.AspNetUserId ?? "";
            MessageDetails.SenderName = claimsData.Username;
            MessageDetails.GroupName = groupname;
            MessageDetails.Message = message;
            MessageDetails.MessageDateTime = DateTime.Now;
            MessageDetails.SentTime = MessageDetails.MessageDateTime.ToString();
            MessageDetails.MessageDate = MessageDetails.MessageDateTime.ToShortDateString();
            MessageDetails.MessageTime = MessageDetails.MessageDateTime.ToShortTimeString();

            await Clients.Group(groupname).SendAsync("ReceiveGroupMessage", MessageDetails);
        }
    }
}