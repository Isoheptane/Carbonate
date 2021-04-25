using System;
using System.Net;
using System.Net.Sockets;
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
            if(arguments.Count != 1)
            {
                ServerMessage("server", user, "\\crCommand only supports 1 arguments.");
                return;
            } 
            else if(backendUser.muteTime > DateTime.Now)
            {
                ServerMessage("server", user, "\\crYou are currently muted.");
                return;
            }
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
            if(arguments.Count != 1)
            {
                ServerMessage("server", user, "\\crCommand only supports 1 arguments.");
                return;
            } 
            else if(backendUser.muteTime > DateTime.Now)
            {
                ServerMessage("server", user, "\\crYou are currently muted.");
                return;
            }
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
            if(arguments.Count != 2)
            {
                ServerMessage("server", user, "\\crCommand only supports 2 arguments.");
                return;
            }
            else if(!OnlineUsers.ContainsKey(arguments[0]))
            {
                ServerMessage("server", user, $"\\crUser \"{arguments[0]}\" is offline or doesn't exist.");
                return;
            }
            else if(backendUser.muteTime > DateTime.Now)
            {
                ServerMessage("server", user, "\\crYou are currently muted.");
                return;
            }
            WhisperMessage(user, OnlineUsers[arguments[0]], arguments[1]);
            Info($"\\7r<{backendUser.nickname}\\7r> -> <{Users[arguments[0]].nickname}\\7r> {arguments[1]}");
        }
    }
}