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
        /// Admin ban user
        /// </summary>
        void AdminSetAdmin(OnlineUser user, CommandPacket command)
        {
            User backendUser = Users[user.Username];
            var arguments = command.arguments;
            //  Permission check
            if (backendUser.permissionLevel < 2)
            {
                PermissionDeniedMessage(user);
                return;
            }
            //  Syntax check
            if (arguments.Count != 1)
            {
                ServerMessage("server", user, "\\crCommand only supports 1 arguments.");
                return;
            }
            //  Check if the user is exist
            if (!Users.ContainsKey(arguments[0]))
            {
                ServerMessage("server", user, $"\\crUser \"{arguments[0]}\" does not exist");
                return;
            }
            //  Check permission level
            User target = Users[arguments[0]];
            if (target.permissionLevel == 1)
            {
                ServerMessage("server", user, $"\\crUser \"{arguments[0]}\" is already a administrator.");
                return;
            }
            //  Check permission level
            if (target.permissionLevel == 2)
            {
                ServerMessage("server", user, $"\\crUser \"{arguments[0]}\"'s permission level is not lower than you.");
                return;
            }
            Users[arguments[0]].permissionLevel = 1;
            Broadcast("server", $"\\brUser {arguments[0]}\\br is now an administrator.");
            if (OnlineUsers.ContainsKey(arguments[0]))
                ServerMessage("server", OnlineUsers[arguments[0]], "\\brYou are now an administrator.");
        }

        void AdminUnsetAdmin(OnlineUser user, CommandPacket command)
        {
            User backendUser = Users[user.Username];
            var arguments = command.arguments;
            //  Permission check
            if (backendUser.permissionLevel < 2)
            {
                PermissionDeniedMessage(user);
                return;
            }
            //  Syntax check
            if (arguments.Count != 1)
            {
                ServerMessage("server", user, "\\crCommand only supports 1 arguments.");
                return;
            }
            //  Check if the user is exist
            if (!Users.ContainsKey(arguments[0]))
            {
                ServerMessage("server", user, $"\\crUser \"{arguments[0]}\" does not exist");
                return;
            }
            //  Check permission level
            User target = Users[arguments[0]];
            if (target.permissionLevel == 0)
            {
                ServerMessage("server", user, $"\\crUser \"{arguments[0]}\" is not an administrator.");
                return;
            }
            //  Check permission level
            if (target.permissionLevel == 2)
            {
                ServerMessage("server", user, $"\\crUser \"{arguments[0]}\"'s permission level is not lower than you.");
                return;
            }
            Users[arguments[0]].permissionLevel = 0;
            Broadcast("server", $"\\crUser {arguments[0]}\\cr is no longer an administrator.");
            if (OnlineUsers.ContainsKey(arguments[0]))
                ServerMessage("server", OnlineUsers[arguments[0]], "\\crYou are no longer an administrator.");
        }
    }
}