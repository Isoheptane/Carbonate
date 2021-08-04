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
            if (arguments.Count != 1 && arguments.Count != 2)
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_arguments_error"]
                    .Replace("$COUNT$", "1, 2")
                );
                return;
            }
            //  Admin command detection
            if (arguments.Count == 2)
            {
                AdminChangeName(user, command);
                return;
            }
            if (!IsLegalNickname(arguments[0]))
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_changename_invalid"]
                );
                return;
            }
            if (!(backendUser.muteTime > DateTime.Now))
                Broadcast(
                    "server", 
                    langFile["command_changename_broadcast"]
                    .Replace("$OLD$", backendUser.nickname)
                    .Replace("$NEW$", arguments[0])
                );
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
                    ServerMessage(
                        "server", 
                        user, 
                        langFile["command_userNotExist"]
                        .Replace("$NAME$", arguments[0])
                    );
                    return;
                }
            }
            else
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_arguments_error"]
                    .Replace("$COUNT$", "0, 1")
                );
                return;
            }
    
            string message;
            message = string.Format(
                "\n" + langFile["command_info_nickname"].Replace("$VALUE$", targetUser.nickname) +
                "\n" + langFile["command_info_username"].Replace("$VALUE$", targetUser.username) +
                "\n" + langFile["command_info_permissionLevel"].Replace("$VALUE$", targetUser.permissionLevel.ToString()) +
                "\n" + langFile["command_info_registerTime"].Replace("$VALUE$", targetUser.registerTime.ToString("yyyy/MM/dd HH:mm:ss")) +
                "\n" + langFile["command_info_lastChatTime"].Replace("$VALUE$", targetUser.lastChatTime.ToString("yyyy/MM/dd HH:mm:ss"))
            );
            if (targetUser.muteTime > DateTime.Now) //< Mute information
                message += "\n" + langFile["command_info_muteUntil"].Replace("$VALUE$", targetUser.muteTime.ToString("yyyy/MM/dd HH:mm:ss"));
            if (targetUser.banTime > DateTime.Now)  //< Ban information
                message += "\n" + langFile["command_info_banUntil"].Replace("$VALUE$", targetUser.banTime.ToString("yyyy/MM/dd HH:mm:ss"));
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
