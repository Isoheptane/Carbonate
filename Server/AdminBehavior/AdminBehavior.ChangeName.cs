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
        void AdminChangeName(OnlineUser user, CommandPacket command)
        {
            User backendUser = Users[user.Username];
            var arguments = command.arguments;
            //  Permission check
            if (backendUser.permissionLevel < 1)
            {
                PermissionDeniedMessage(user);
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
            //  Check nickname
            if (!IsLegalNickname(arguments[1]))
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_changename_invalid"]
                );
                return;
            }
            if (!(target.muteTime > DateTime.Now))
                Broadcast(
                    "server", 
                    langFile["command_changename_force"]
                    .Replace("$OLD$", target.nickname)
                    .Replace("$NAME$", target.username)
                    .Replace("$NEW$", arguments[1])
                );
            target.nickname = arguments[1];
        }

    }
}
