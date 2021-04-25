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
        /// Process a packet sent by user.
        /// </summary>
        /// <param name="sender">Sender user</param>
        /// <param name="command">Packet</param>
        public void ProcessUserPacket(OnlineUser sender, Packet packet)
        {
            User user = Users[sender.Username];
            string command = packet["command"];
            if (command == "keep-alive")
            {
                sender.KeepAlive();
            }
            else
            {
                ServerMessage("server", $"\\crInvalid command \"{command}\"", sender);
            }
        }
    }
}
