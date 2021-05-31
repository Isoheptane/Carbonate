using System;
using System.Threading;
using Carbonate.Standard;

namespace Carbonate.Client
{
    public partial class Client
    {
        public struct Events
        {
            //  SendPacketFailedEvent
            public delegate void ErrorEventHandler(Thread sender, string message);
            public event ErrorEventHandler ErrorEvent;
            public void RaiseErrorEvent(Thread sender, string message) 
            => ErrorEvent(sender, message);
            //  OnMessageEvent
            public delegate void OnMessageEventHandler(Packet packet);
            public event OnMessageEventHandler OnMessageEvent;
            public void RaiseOnMessageEvent(Packet packet) 
            => OnMessageEvent(packet);
            //  ServerDisconnectEvent
            public delegate void ServerDisconnectEventHandler(Packet packet);
            public ServerDisconnectEventHandler ServerDisconnectEvent;
            public void RaiseServerDisconnectEvent(Packet packet)
            => ServerDisconnectEvent(packet);
        }
        
        public Events events;
    }
}