using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{
    public partial class Server
    {
        /// <summary>
        /// Process a packet sent by user.
        /// </summary>
        /// <param name="sender">Sender user</param>
        /// <param name="command">Packet</param>
        public void ProcessUserPacket(OnlineUser sender, Packet packet)
        {
            User user = Users[sender.Username];
            CommandPacket command = new CommandPacket(packet);
            switch (command.command.ToLower())
            {
                case "say":             //< User chat
                    UserChat(sender, command);
                    break;

                case "me":              //< User action
                    UserAction(sender, command);
                    break;

                case "tell":            //< User whisper
                    UserWhisper(sender, command);
                    break;

                case "changename":      //< User change name
                    UserChangeName(sender, command);
                    break;

                case "manual":          //< User gets server manual
                    UserGetManual(sender, command);
                    break;

                case "list":            //< User list all messages
                    UserGetOnlineList(sender, command);
                    break;

                case "keep-alive":      //< Keep-Alive packet
                    sender.KeepAlive();
                    break;

                case "disconnect":      //< User disconnect
                    DisconnectMessage(sender, "server", "User disconnected.");
                    Disconnect(sender.Username);
                    break;

                case "broadcast":
                    AdminBroadcast(sender, command);
                    break;
                
                case "mute":
                    AdminMute(sender, command);
                    break;
                
                case "unmute":
                    AdminUnmute(sender, command);
                    break;

                case "ban":
                    AdminBan(sender, command);
                    break;
                
                case "unban":
                    AdminUnban(sender, command);
                    break;

                default:                //< Invalid command
                    ServerMessage("server", sender, $"\\crInvalid command \"{command.command}\"");
                    break;
            }
        }
    }
}
