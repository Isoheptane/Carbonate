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
        void UserGetManual(OnlineUser user, CommandPacket command)
        {
            User backendUser = Users[user.Username];
            var arguments = command.arguments;
            if (arguments.Count == 0)
            {
                string[] filePath = Directory.GetFiles($"{WorkspaceDirectory}/manual/");
                string[] dirPath = Directory.GetDirectories($"{WorkspaceDirectory}/manual/");
                StringBuilder message = new StringBuilder();
                message.Append($"\n\\erThrere are \\cr{filePath.Length} \\erfiles in the manual folder.\n");
                foreach (string path in filePath)
                    message.Append($"\\7r{Unescape(path)}\n");
                message.Append($"\n\\erThrere are \\cr{dirPath.Length} \\erdirectories in the manual directory.\n");
                foreach (string path in dirPath)
                    message.Append($"\\7r{Unescape(path)}\n");
                ServerMessage("server", user, message.ToString());
            }
        }
    }
}
