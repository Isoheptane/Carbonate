using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{
    public partial class Server
    {

        ConcurrentDictionary<string, OnlineUser> onlineUsers = new ConcurrentDictionary<string, OnlineUser>();

        /// <value>Online users on the server.</value>
        public ConcurrentDictionary<string, OnlineUser> OnlineUsers
        {
            get { return onlineUsers; }
        }

        public void Connect(string username, TcpClient client)
        {
            onlineUsers.TryAdd(username, new OnlineUser(username, client));
            Packet messagePacket = new Packet();
            messagePacket["type"]       = "broadcast";
            messagePacket["sender"]     = "server";
            messagePacket["message"]    = $"\\br{username} \\rr Joined in the server.";
            foreach(var users in onlineUsers)
            {
                try
                {
                    users.Value.Send(messagePacket);
                }
                catch (Exception ex)
                {
                    Error($"Error occured while sending message to user \"{users.Value.Username}\": {ex.Message}");
                }
            }
        }
    }
}
