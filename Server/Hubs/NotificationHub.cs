using Blazor.Twilio.Video.Shared;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Blazor.Twilio.Video.Server.Hubs
{
    public class NotificationHub : Hub
    {
        public Task RoomsUpdated(string room) =>
            Clients.All.SendAsync(HubEndpoints.RoomsUpdated, room);
    }
}