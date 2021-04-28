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
        static Thread messageListener;

        static void BeginListener()
        {
            messageListener = new Thread(ListenMessage);
            messageListener.Start();
        }

        static void ListenMessage()
        {
            while (client.Connected)
            {
                if (!client.DataAvailable)
                {
                    Thread.Sleep(1);
                    continue;
                }
                try {
                    Packet packet = client.Receive();
                    string sender = packet["sender"];
                    string message = packet["message"];
                    switch ((string)packet["messageType"])
                    {
                        case "chat":
                            {
                                WriteLine($"<{(string)packet["senderNickname"]}\\rr> {message}");
                                break;
                            }
                        case "whisper":
                            {
                                WriteLine($"\\8r{(string)packet["senderNickname"]}\\8r whispered to you: {message}");
                                break;
                            }
                        case "action":
                            {
                                WriteLine($"\\cr* \\rr{(string)packet["senderNickname"]} {message}");
                                break;
                            }
                        case "server":
                            {
                                WriteLine($"\\ar{(string)packet["sender"]}: {message}");
                                break;
                            }
                        case "broadcast":
                            {
                                WriteLine($"\\arBroadcast:\\9r{(string)packet["sender"]}\\ar> \\rr{message}");
                                break;
                            }
                        case "disconnect":
                            {
                                WriteLine($"\\crDisconnected:\\rr {sender}:{message}");
                                client.Disconnect();
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    WriteLine($"\\crError: Error occured while receiving message: {ex.Message}");
                }
            }
        }

    }
}
