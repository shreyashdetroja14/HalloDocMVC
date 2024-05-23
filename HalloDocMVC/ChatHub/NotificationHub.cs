using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.SignalR;

namespace HalloDocMVC.ChatHub
{
    public class NotificationHub : Hub
    {
        private readonly IJwtService _jwtService;
        private readonly IMessageService _messageService;
        public static Dictionary<string, string> NotificationConnectionsStorage = new Dictionary<string, string>();
        public NotificationHub(IJwtService jwtService, IMessageService messageService)
        {
            _jwtService = jwtService;
            _messageService = messageService;
        }

        public override async Task OnConnectedAsync()
        {
            ClaimsData claimsData = _jwtService.GetClaimValues();
            string connectionId = Context.ConnectionId;
            if (NotificationConnectionsStorage.Where(x => x.Key == claimsData.AspNetUserId).Any())
            {
                NotificationConnectionsStorage.Remove(claimsData.AspNetUserId ?? "");
            }
            NotificationConnectionsStorage.Add(claimsData.AspNetUserId ?? "", connectionId);
            //Groups.AddToGroupAsync("testname", connectionId);

            await base.OnConnectedAsync();
        }

        public async Task SendNotification(MessageViewModel MessageDetails)
        {
            string receiverConnectionId = NotificationConnectionsStorage.FirstOrDefault(x => x.Key == MessageDetails.ReceiverId).Value;
            if (receiverConnectionId != null)
            {
                await Clients.Client(receiverConnectionId).SendAsync("SendNotification", MessageDetails);
            }
        }

    }
}