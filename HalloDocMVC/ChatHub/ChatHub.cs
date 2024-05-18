using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.SignalR;

namespace HalloDocMVC.ChatHub
{
    public class ChatHub : Hub
    {
        private readonly IJwtService _jwtService;
        public static Dictionary<string, string> ConnectionsStorage = new Dictionary<string, string>();
        public ChatHub(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }
        public override async Task OnConnectedAsync()
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            string connectionId = Context.ConnectionId;

            ConnectionsStorage.Add(claimsData.AspNetUserId ?? "", connectionId);

            await base.OnConnectedAsync();
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



            await Clients.Client(connectionId).SendAsync("ReceiveMessage", MessageDetails);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            ConnectionsStorage.Remove(claimsData.AspNetUserId ?? "");
        }
    }
}