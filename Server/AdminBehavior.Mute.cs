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
        /// Admin mute user
        /// </summary>
        void AdminMute(OnlineUser user, CommandPacket command)
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
            //  Calculate time
            DateTime untilTime = DateTime.Now.AddTicks(time.Ticks);

            Users[arguments[0]].muteTime = untilTime;
            Info(
                $"{backendUser.nickname}\\rr({backendUser.username}\\rr)" +
                $" muted {target.nickname}\\rr({target.username}\\rr)."
            );
            Broadcast(
                "server", 
                langFile["command_mute_broadcast"]
                .Replace("$NICK$", target.nickname)
                .Replace("$NAME$", target.username)
                .Replace("$TIME$", untilTime.ToString("yyyy/MM/dd HH:mm:ss"))
            );
            if (OnlineUsers.ContainsKey(arguments[0]))
                ServerMessage(
                    "server", 
                    OnlineUsers[arguments[0]], 
                    langFile["command_mute_message"]
                    .Replace("$TIME$", untilTime.ToString("yyyy/MM/dd HH:mm:ss"))
                );
        }

        void AdminUnmute(OnlineUser user, CommandPacket command)
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
            if (target.muteTime < DateTime.Now)
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_unmute_notMuted"]
                    .Replace("$NICK$", target.nickname)
                    .Replace("$NAME$", target.username)
                );
                return;
            }

            Users[arguments[0]].muteTime = DateTime.MinValue;
            Info(
                $"{backendUser.nickname}\\rr({backendUser.username}\\rr)" +
                $" unmuted {target.nickname}\\rr({target.username}\\rr)."
            );
            Broadcast(
                "server", 
                langFile["command_unmute_broadcast"]
                .Replace("$NICK$", target.nickname)
                .Replace("$NAME$", target.username)
            );
            if (OnlineUsers.ContainsKey(arguments[0]))
                ServerMessage(
                    "server", 
                    OnlineUsers[arguments[0]], 
                    langFile["command_unmute_message"]
                );
        }
    }
}