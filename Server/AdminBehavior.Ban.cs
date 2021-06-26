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
                ServerMessage("server", user, "\\crCommand only supports 2 arguments.");
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
            if (target.permissionLevel >= backendUser.permissionLevel)
            {
                ServerMessage("server", user, $"\\crUser \"{arguments[0]}\"'s permission level is not lower than you.");
                return;
            }
            //  Convert time
            DateTime time;
            if (!TypeConvert.TryGetTime(arguments[1], out time))
            {
                ServerMessage("server", user, $"Time format is invalid.");
                return;
            }
            
            DateTime untilTime = DateTime.Now.AddTicks(time.Ticks);

            Users[arguments[0]].banTime = untilTime;
            Broadcast("server", $"\\crUser \"{arguments[0]}\\cr\" is banned until {untilTime.ToString("yyyy/MM/dd HH:mm:ss")}.");
            if (OnlineUsers.ContainsKey(arguments[0]))
            {
                DisconnectMessage(OnlineUsers[arguments[0]], user.Username, $"\\crYou are banned until {untilTime.ToString("yyyy/MM/dd HH:mm:ss")}.");
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
            if (target.permissionLevel >= backendUser.permissionLevel)
            {
                ServerMessage("server", user, $"\\crUser \"{arguments[0]}\"'s permission level is not lower than you.");
                return;
            }
            //  Check if is unmutable
            if (target.banTime < DateTime.Now)
            {
                ServerMessage("server", user, $"\\crUser \"{arguments[0]}\" is not banned now.");
                return;
            }

            Users[arguments[0]].banTime = DateTime.MinValue;

            Broadcast("server", $"\\arUser \"{arguments[0]}\\ar\" is unbanned.");
        }
    }
}