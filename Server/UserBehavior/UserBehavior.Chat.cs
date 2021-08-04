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
        /// User chat if user is not muted
        /// </summary>
        /// <param name="user">Speaker</param>
        /// <param name="message">Message content</param>
        void UserChat(OnlineUser user, CommandPacket command)
        {
            User backendUser = Users[user.Username];
            var arguments = command.arguments;
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
            ChatMessage(user, arguments[0]);
            Info($"<{backendUser.nickname}\\rr> {arguments[0]}");
        }

        /// <summary>
        /// User action if user is not muted
        /// </summary>
        /// <param name="user">Speaker</param>
        /// <param name="message">Action</param>
        void UserAction(OnlineUser user, CommandPacket command)
        {
            User backendUser = Users[user.Username];
            var arguments = command.arguments;
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
            ActionMessage(user, arguments[0]);
            Info($"\\cr* \\rr{backendUser.nickname} \\rr{arguments[0]}");
        }

        /// <summary>
        /// User whisper is user is not muted
        /// </summary>
        void UserWhisper(OnlineUser user, CommandPacket command)
        {
            User backendUser = Users[user.Username];
            var arguments = command.arguments;
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
            else if (!OnlineUsers.ContainsKey(arguments[0]))
            {
                ServerMessage(
                    "server", 
                    user, 
                    langFile["command_tell_targetOffline"]
                    .Replace("$NAME$", arguments[0])
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
            WhisperMessage(user, OnlineUsers[arguments[0]], arguments[1]);
            ServerMessage("server", user, $"\\8rYou -> \"{Users[arguments[0]].nickname}\\8r\"\\rr: {arguments[1]}");
            Info($"\\8r<{backendUser.nickname}\\8r> -> <{Users[arguments[0]].nickname}\\8r> \\rr{arguments[1]}");
        }
    }
}
