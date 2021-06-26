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
                Broadcast("server", $"{backendUser.nickname}\\rr changed name to {arguments[0]}.");
            backendUser.nickname = arguments[0];
        }

        /// <summary>
        /// Get user's own information
        /// </summary>
        void UserGetInfo(OnlineUser user, CommandPacket command)
        {
            User targetUser;
            var arguments = command.arguments;
            if (arguments.Count == 0)       //< Query for sender's information
            {
                targetUser = Users[user.Username];
            }
            else if (arguments.Count == 1)  //< Query for other's information 
            {
                if (Users.ContainsKey(arguments[0]))
                    targetUser = Users[arguments[0]];
                else
                {
                    ServerMessage("server", user, $"\\crUser \"{arguments[0]}\" does not exist.");
                    return;
                }
            }
            else
            {
                ServerMessage("server", user, "\\crCommand only supports 0 or 1 arguments.");
                return;
            }
    
            string message;
            message = string.Format(
                $"\n\\er          Nickname:\\rr {targetUser.nickname}" +
                $"\n\\er          Username:\\rr {targetUser.username}" +
                $"\n\\er  Permission Level:\\rr {targetUser.permissionLevel}" +
                $"\n\\er     Register Time:\\rr {targetUser.registerTime.ToString("yyyy/MM/dd HH:mm:ss")}" +
                $"\n\\er    Last Chat Time:\\rr {targetUser.lastChatTime.ToString("yyyy/MM/dd HH:mm:ss")}"
            );
            if (targetUser.muteTime > DateTime.Now) //< Mute information
                message += $"\n\\er       Muted until:\\rr {targetUser.muteTime.ToString("yyyy/MM/dd HH:mm:ss")}";
            if (targetUser.banTime > DateTime.Now)  //< Ban information
                message += $"\n\\er      Banned until:\\rr {targetUser.banTime.ToString("yyyy/MM/dd HH:mm:ss")}";
            ServerMessage("server", user, message);
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
