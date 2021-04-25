using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{
    public partial class Server
    {
        public void ServerBroadcast(string sender, string message)
        {
            Packet messagePacket = new Packet();
            messagePacket["messageType"]    = "broadcast";
            messagePacket["sender"]         = sender;
            messagePacket["message"]        = message;
            foreach (var user in OnlineUsers)
            {
                Task.Run(() => 
                {
                    try
                    {
                        user.Value.Send(messagePacket);
                    }
                    catch (Exception ex)
                    {
                        Error($"Error occured while sending message to user \"{user.Value.Username}\": {ex.Message}");
                    }
                });
            }
        }

        public void ServerMessage(string sender, string message, OnlineUser user)
        {
            Packet messagePacket = new Packet();
            messagePacket["messageType"]    = "server";
            messagePacket["sender"]         = sender;
            messagePacket["message"]        = message;
            Task.Run(() => 
            { 
                try {
                    user.Send(messagePacket); 
                }
                catch (Exception ex)
                {
                    Error($"Error occured while sending message to user \"{user.Username}\": {ex.Message}");
                }
            });
        }
    }
}
