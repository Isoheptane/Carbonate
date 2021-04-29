using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{
    public partial class Server
    {
        /// <summary>
        /// Change user's own name
        /// </summary>
        void UserChangeName(OnlineUser user, CommandPacket command)
        {
            User backendUser = Users[user.Username];
            var arguments = command.arguments;
            if (arguments.Count != 1)
            {
                ServerMessage("server", user, "\\crCommand only supports 1 arguments.");
                return;
            }
            else if (!IsLegalNickname(arguments[0]))
            {
                ServerMessage("server", user, "\\crNickname contains invalid character.");
                return;
            }
            if (!(backendUser.muteTime > DateTime.Now))
                Broadcast("server", $"{backendUser.nickname}\\rr changed name to {arguments[0]}");
            backendUser.nickname = arguments[0];
        }

        /// <summary>
        /// User gets the current online list
        /// </summary>
        void UserGetOnlineList(OnlineUser user, CommandPacket command)
        {
            StringBuilder message = new StringBuilder();
            message.Append($"\n\\erThere are \\cr{OnlineUsers.Count} \\erusers online:\n");
            int counter = 0;
            foreach(var onlineUser in OnlineUsers)
            {
                counter++;
                string username = onlineUser.Value.Username;
                message.Append($"{Users[username].nickname}\\rr({username})" + (counter == OnlineUsers.Count ? "" : ", "));
            }
            ServerMessage("server", user, message.ToString());
        }
    }
}
