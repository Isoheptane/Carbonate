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
        void AdminBan(OnlineUser user, CommandPacket command)
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
            if (arguments.Count != 2)
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_arguments_error"]
                    .Replace("$COUNT$", "2")
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
            if (target.permissionLevel >= backendUser.permissionLevel)
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
            //  Convert time
            DateTime time;
            if (!TypeConvert.TryGetTime(arguments[1], out time))
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_invalidTimeFormat"]
                );
                return;
            }
            
            DateTime untilTime = DateTime.Now.AddTicks(time.Ticks);

            Users[arguments[0]].banTime = untilTime;
            Info(
                $"{backendUser.nickname}\\rr({backendUser.username}\\rr)" +
                $" banned {target.nickname}\\rr({target.username}\\rr)."
            );
            Broadcast(
                "server", 
                langFile["command_ban_broadcast"]
                .Replace("$NICK$", target.nickname)
                .Replace("$NAME$", target.username)
                .Replace("$TIME$", untilTime.ToString("yyyy/MM/dd HH:mm:ss"))
            );
            if (OnlineUsers.ContainsKey(arguments[0]))
            {
                DisconnectMessage(
                    OnlineUsers[arguments[0]], 
                    user.Username, 
                    langFile["command_ban_message"]
                    .Replace("$TIME$", untilTime.ToString("yyyy/MM/dd HH:mm:ss"))
                );
                Disconnect(arguments[0]);
            }
        }

        void AdminUnban(OnlineUser user, CommandPacket command)
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
            if (target.permissionLevel >= backendUser.permissionLevel)
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
            //  Check if is unmutable
            if (target.banTime < DateTime.Now)
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_unban_notBanned"]
                    .Replace("$NICK$", target.nickname)
                    .Replace("$NAME$", target.username)
                );
                return;
            }

            Users[arguments[0]].banTime = DateTime.MinValue;
            Info(
                $"{backendUser.nickname}\\rr({backendUser.username}\\rr)" +
                $" unbanned {target.nickname}\\rr({target.username}\\rr)."
            );
            Broadcast(
                "server", 
                langFile["command_unban_broadcast"]
                .Replace("$NICK$", target.nickname)
                .Replace("$NAME$", target.username)
            );
        }
    }
}