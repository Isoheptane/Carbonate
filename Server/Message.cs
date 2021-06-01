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
        /// <summary>
        /// Send a packet to the indicated user.
        /// </summary>
        /// <param name="user">Indicated user</param>
        /// <param name="packet">The packet to be sent</param>
        void SendPacket(OnlineUser user, Packet packet)
        {
            try
            {
                if(user.Connected)
                    user.Send(packet);
            }
            catch (Exception ex)
            {
                Error($"Error occured while sending message to user \"{user.Username}\": {Unescape(ex.ToString())}");
            }
        }

        /// <summary>
        /// Send a packet to the indicated user using a individual thread.
        /// </summary>
        void AsyncSendPacket(OnlineUser user, Packet packet)
        {
            Task.Run(() => { SendPacket(user, packet); });
        }

        /// <summary>
        /// Send a packet to all connected user.
        /// </summary>
        void BroadcastPacket(Packet packet)
        {
            foreach (var user in OnlineUsers)
            {
                SendPacket(user.Value, packet);
            }
        }

        /// <summary>
        /// Send a packet to all connected user using a individual thread.
        /// </summary>
        void AsyncBroadcastPacket(Packet packet)
        {
            Task.Run(() => { BroadcastPacket(packet); });
        }

        /// <summary>
        /// Generate a message packet using indicated informations.
        /// </summary>
        public static Packet GenerateMessagePacket(string type, string sender, string message)
        {
            Packet messagePacket = new Packet();
            messagePacket["messageType"]    = type;
            messagePacket["sender"]         = sender;
            messagePacket["message"]        = message;
            return messagePacket;
        }

        /// <summary>
        /// Generate a message packet using indicated informations.
        /// </summary>
        public static Packet GenerateMessagePacket(string type, string sender, string senderNickname, string message)
        {
            Packet messagePacket = new Packet();
            messagePacket["messageType"]    = type;
            messagePacket["sender"]         = sender;
            messagePacket["senderNickname"] = senderNickname;
            messagePacket["message"]        = message;
            return messagePacket;
        }

        /// <summary>
        /// Broadcast message to all users
        /// </summary>
        public void Broadcast(string sender, string message)
        {
            Packet packet = GenerateMessagePacket("broadcast", sender, message);
            AsyncBroadcastPacket(packet);
            EnqueueHistory(packet);
            Info($"\\arBroadcast:\\9r{sender}\\ar> \\rr{message}");
        }

        /// <summary>
        /// Send message to a single user
        /// </summary>
        public void ServerMessage(string sender, OnlineUser user, string message)
        {
            SendPacket(
                user, 
                GenerateMessagePacket("server", sender, message)
            );
        }

        /// <summary>
        /// Send message to a single user.
        /// </summary>
        public void AsyncServerMessage(string sender, OnlineUser user, string message)
        {
            Task.Run(() => { ServerMessage(sender, user, message); });
        }
        
        /// <summary>
        /// Broadcast chat message.
        /// </summary>
        public void ChatMessage(OnlineUser sender, string message)
        {
            Packet packet = GenerateMessagePacket(
                "chat",
                 sender.Username,
                 Users[sender.Username].nickname,
                 message
            );
            AsyncBroadcastPacket(packet);
            EnqueueHistory(packet);
        }

        /// <summary>
        /// Send whisper chat message
        /// </summary>
        public void WhisperMessage(OnlineUser sender, OnlineUser user, string message)
        {
            Packet packet = GenerateMessagePacket(
                "whisper",
                 sender.Username,
                 Users[sender.Username].nickname,
                 message
            );
            SendPacket(user, packet);
        }

        /// <summary>
        /// Send whisper chat message
        /// </summary>
        public void AsyncWhisperMessage(OnlineUser sender, OnlineUser user, string message)
        {
            Task.Run(() => { WhisperMessage(sender, user, message); });
        }

        public void ActionMessage(OnlineUser sender, string message)
        {
            Packet packet = GenerateMessagePacket(
                "action",
                 sender.Username,
                 Users[sender.Username].nickname,
                 message
            );
            AsyncBroadcastPacket(packet);
            EnqueueHistory(packet);
        }

        /// <summary>
        /// Send force disconnect message
        /// </summary>
        public void DisconnectMessage(OnlineUser receiver, string sender, string message)
        {
            Packet messagePacket = GenerateMessagePacket(
                "disconnect",
                sender,
                message
            );
            SendPacket(receiver, messagePacket);
        }

    }
}
