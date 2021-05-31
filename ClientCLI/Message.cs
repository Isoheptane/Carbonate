using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using Carbonate.Standard;
using JsonSharp;
using static ClientCLI.ScreenManager;

namespace ClientCLI
{
    partial class Program
    {
        static void ShowGroupMessage(JsonArray array)
        {
            foreach (var message in array.elements)
                ShowMessage(message);
        }

        static void ShowMessage(JsonObject packet)
        {
            string sender = packet["sender"];
            string message = packet["message"];
            switch ((string)packet["messageType"])
            {
                case "chat":
                    WriteLine($"<{(string)packet["senderNickname"]}\\rr> {message}");
                    break;
                
                case "whisper":                
                    WriteLine($"\\8r{(string)packet["senderNickname"]}\\8r whispered to you: {message}");
                    break;

                case "action":
                    WriteLine($"\\cr* \\rr{(string)packet["senderNickname"]} {message}");
                    break;

                case "server":
                    WriteLine($"\\9r{(string)packet["sender"]}\\rr: {message}");
                    break;

                case "broadcast":
                    WriteLine($"\\arBroadcast:\\9r{(string)packet["sender"]}\\ar> \\rr{message}");
                    break;
            }
        }

    }
}
