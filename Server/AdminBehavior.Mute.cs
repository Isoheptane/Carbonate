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
                ServerMessage("server", user, "\\crCommand only supports 2 arguments.");
                return;
            }
            //  Check if the user is exist
            if (!Users.ContainsKey(arguments[0]))
            {
                ServerMessage("server", user, $"\\crUser {arguments[0]}\\cr does not exist.");
                return;
            }
            //  Check permission level
            User target = Users[arguments[0]];
            if (target.permissionLevel >= backendUser.permissionLevel)
            {
                ServerMessage("server", user, $"\\cr{target.nickname}\\rr({target.username}\\rr)\\cr's permission level is not lower than you.");
                return;
            }
            //  Convert time
            DateTime time;
            if (!TypeConvert.TryGetTime(arguments[1], out time))
            {
                ServerMessage("server", user, $"Time format is invalid.");
                return;
            }
            //  Calculate time
            DateTime untilTime = DateTime.Now.AddTicks(time.Ticks);

            Users[arguments[0]].muteTime = untilTime;
            Broadcast("server", $"\\cr{target.nickname}\\rr({target.username}\\rr)\\cr is muted until {untilTime.ToString("yyyy/MM/dd HH:mm:ss")}.");
            if (OnlineUsers.ContainsKey(arguments[0]))
                ServerMessage("server", OnlineUsers[arguments[0]], $"\\crYou are muted until {untilTime.ToString("yyyy/MM/dd HH:mm:ss")}.");
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
                ServerMessage("server", user, "\\crCommand only supports 1 arguments.");
                return;
            }
            //  Check if the user is exist
            if (!Users.ContainsKey(arguments[0]))
            {
                ServerMessage("server", user, $"\\crUser {arguments[0]}\\cr does not exist.");
                return;
            }
            //  Check permission level
            User target = Users[arguments[0]];
            if (target.permissionLevel >= backendUser.permissionLevel)
            {
                ServerMessage("server", user, $"\\cr{target.nickname}\\rr({target.username}\\rr)\\cr's permission level is not lower than you.");
                return;
            }
            //  Check if is unmutable
            if (target.muteTime < DateTime.Now)
            {
                ServerMessage("server", user, $"\\cr{target.nickname}\\rr({target.username}\\rr)\\cr is not muted now.");
                return;
            }

            Users[arguments[0]].muteTime = DateTime.MinValue;

            Broadcast("server", $"\\ar{target.nickname}\\rr({target.username}\\rr)\\ar is unmuted.");
            if (OnlineUsers.ContainsKey(arguments[0]))
                ServerMessage("server", OnlineUsers[arguments[0]], $"\\arYou are unmuted now.");
        }
    }
}