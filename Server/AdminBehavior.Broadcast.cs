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
        /// Admin broadcast message
        /// </summary>
        /// <param name="user">Sender</param>
        /// <param name="message">Message content</param>
        void AdminBroadcast(OnlineUser user, CommandPacket command)
        {
            User backendUser = Users[user.Username];
            var arguments = command.arguments;
            //  Permission check
            if (backendUser.permissionLevel < 1)
            {
                PermissionDeniedMessage(user);
                return;
            }
            //  Syntax check
            if (arguments.Count != 1)
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_arguments_error"]
                    .Replace("$COUNT$", "1")
                );
                return;
            }
            else if (backendUser.muteTime > DateTime.Now)
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["user_muted"]
                );
                return;
            }
            backendUser.lastChatTime = DateTime.Now;
            Broadcast(backendUser.nickname, arguments[0]);
        }

    }
}
