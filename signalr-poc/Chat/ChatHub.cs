using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace signalr_poc.Chat
{
    [HubName("Chat")]
    public class ChatHub: Hub
    {
        public void Say(Message message)
        {
            Clients.All.AcceptNewMessage(message);
        }
    }
}