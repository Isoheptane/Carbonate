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
                ServerMessage("server", user, langFile["command_arguments_error"].Replace("$COUNT$", "1"));
                return;
            }
            //  Check if the user is exist
            if (!Users.ContainsKey(arguments[0]))
            {
                ServerMessage("server", user, langFile["command_userNotExist"].Replace("$NAME$", arguments[0]));
                return;
            }
            //  Check permission level
            User target = Users[arguments[0]];
            if (target.permissionLevel == 1)
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_setadmin_alreadyAdmin"]
                    .Replace("$NICK$", target.nickname)
                    .Replace("$NAME$", target.username)
                );
                return;
            }
            //  Check permission level
            if (target.permissionLevel == 2)
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_higherPermission"]
                    .Replace("$NICK$", target.nickname)
                    .Replace("$NAME$", target.username)
                );
                return;
            }
            Users[arguments[0]].permissionLevel = 1;
            Broadcast(
                "server", 
                langFile["command_setadmin_broadcast"]
                .Replace("$NICK$", target.nickname)
                .Replace("$NAME$", target.username)
            );
            if (OnlineUsers.ContainsKey(arguments[0]))
                ServerMessage(
                    "server", 
                    OnlineUsers[arguments[0]], 
                    langFile["command_setadmin_message"]
                );
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
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_arguments_error"]
                    .Replace("$COUNT$", "1")
                );
                return;
            }
            //  Check if the user is exist
            if (!Users.ContainsKey(arguments[0]))
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_userNotExist"]
                    .Replace("$NAME$", arguments[0])
                );
                return;
            }
            //  Check permission level
            User target = Users[arguments[0]];
            if (target.permissionLevel == 0)
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_unsetadmin_notAdmin"]
                    .Replace("$NICK$", target.nickname)
                    .Replace("$NAME$", target.username)
                );
                return;
            }
            //  Check permission level
            if (target.permissionLevel == 2)
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_higherPermission"]
                    .Replace("$NICK$", target.nickname)
                    .Replace("$NAME$", target.username)
                );
                return;
            }
            Users[arguments[0]].permissionLevel = 0;
            Broadcast(
                "server", 
                langFile["command_unsetadmin_broadcast"]
                .Replace("$NICK$", target.nickname)
                .Replace("$NAME$", target.username)
            );
            if (OnlineUsers.ContainsKey(arguments[0]))
                ServerMessage(
                    "server", 
                    OnlineUsers[arguments[0]], 
                    langFile["command_unsetadmin_message"]
                );
        }
    }
}