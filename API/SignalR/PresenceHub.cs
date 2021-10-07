using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        public PresenceTracker Tracker { get; }
        public PresenceHub(PresenceTracker tracker)
        {
            this.Tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            await Tracker.UserConnected(Context.User.getUserName(),Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User.getUserName());

            var CurUsers = Tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", CurUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Tracker.UserDisconnected(Context.User.getUserName(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User.getUserName());

            await base.OnDisconnectedAsync(exception);
        }
    }
}