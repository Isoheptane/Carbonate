using System;
using System.Threading;
using Carbonate.Standard;
using static ClientCLI.ScreenManager;

namespace ClientCLI
{
    partial class Program
    {
        static void OnReceiveMessage(Packet packet)
        {
            switch ((string)packet["messageType"])
            {
                case "chat":
                case "whisper":
                case "action":
                case "server":
                case "broadcast":
                    ShowMessage(packet.ToJsonObject());
                    break;

                case "history":
                    WriteLine("\\8r---- History Message ----");
                    ShowGroupMessage(packet["messages"]);
                    WriteLine("\\8r---- History Message ----");
                    break;
            }
        }

        static void OnError(Thread sender, string message)
        {
            WriteLine($"\\crError occured in thread \"{sender.Name}\": {message}");
        }

        static void ServerDisconnect(Packet packet)
        {
            WriteLine($"\\crDisconnected: \\9r{(string)packet["sender"]}\\rr: {(string)packet["message"]}");
        }
    }
}