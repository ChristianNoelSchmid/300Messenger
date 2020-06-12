using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messages
{
    public class SessionHub : Hub
    {
        public async Task Join(int sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId.ToString());
        }
        public async Task UpdateMessage(int sessionId)
        {
            await Clients.Group(sessionId.ToString()).SendAsync("UpdateSessions");
        }
    }
}
