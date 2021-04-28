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
        public static void LocalCommand(Command command)
        {
            switch(command.command.ToLower())
            {
                case "connect":
                    {
                        Connect(command);
                        break;
                    }
                case "register":
                    {
                        Register(command);
                        break;
                    }
                case "ping":
                    {
                        Ping(command);
                        break;
                    }
                case "disconnect":
                    {
                        Disconnect();
                        break;
                    }
                case "clear":
                    {
                        Console.Clear();
                        Initialize();
                        break;
                    }
                case "fs":
                case "flowspeed":
                    {
                        SetFlowSpeed(command);
                        break;
                    }
                case "exit":
                    {
                        Environment.Exit(0);
                        break;
                    }
                default:
                    {
                        WriteLine($"\\crInvalid command \"{command.command}\".\n");
                        break;
                    }
            }
        }

        static void Connect(Command command)
        {
            if (command.arguments.Count != 1)
            {
                WriteLine("\\crLocal command \"connect\" only supports 1 arguments:");
                WriteLine("\\cr  !command <address>[:port]");
                return;
            }
            try
            {
                IPEndPoint ep = Converter.GetIPEndPoint(command.arguments[0]);
                WriteLine($"\\erConnecting \\br{command.arguments[0]}\\rr(\\er{ep}\\rr)...");
                if (client.Connect(ep))
                {
                    BeginListener();
                }
            }
            catch (Exception ex)
            {
                WriteLine($"\\crFailed to connect to the server: {ex.Message}");
            }
        }

        static void Register(Command command)
        {
            if (command.arguments.Count != 1)
            {
                WriteLine("\\crLocal command \"register\" only supports 1 arguments:");
                WriteLine("\\cr  !register <address>[:port]\n");
                return;
            }
            try
            {
                IPEndPoint ep = Converter.GetIPEndPoint(command.arguments[0]);
                WriteLine($"\\erConnecting \\br{command.arguments[0]}\\rr(\\er{ep}\\rr)...");
                if (client.Register(ep))
                {
                    BeginListener();
                }
            }
            catch (Exception ex)
            {
                WriteLine($"\\crFailed to connect to the server: {ex.Message}");
            }
        }

        static void Ping(Command command)
        {
            if (command.arguments.Count != 1)
            {
                WriteLine("\\crLocal command \"register\" only supports 1 arguments:");
                WriteLine("\\cr  !ping <address>[:port]");
                return;
            }
            try
            {
                IPEndPoint ep = Converter.GetIPEndPoint(command.arguments[0]);
                WriteLine($"\\erPinging \\br{command.arguments[0]}\\rr(\\er{ep}\\rr)...");
                Packet response = client.Ping(ep);
                WriteLine($"Server Name: {(string)response["name"]}");
                WriteLine($"Description:");
                foreach (string line in ((JsonArray)(response["description"])).elements)
                    WriteLine($" -{line}");
            }
            catch (Exception ex)
            {
                WriteLine($"\\crFailed to connect to the server: {ex.Message}");
            }
        }

        static void Disconnect()
        {
            client.Disconnect();
            WriteLine("\\crForced disconnect.");
        }

        static void SetFlowSpeed(Command command)
        {
            if (command.arguments.Count != 1)
            {
                WriteLine("\\crLocal command \"register\" only supports 1 arguments:");
                WriteLine("\\cr  !flowspeed <miliseconds>");
                return;
            }
            try
            {
                flowspeed = int.Parse(command.arguments[0]);
                WriteLine($"\\erChanged flowspeed to \\cr{flowspeed} \\ermiliseconds.");
            }
            catch (Exception ex)
            {
                WriteLine($"\\crFailed to change flow speed: {ex.Message}");
            }
        }
    }
}
