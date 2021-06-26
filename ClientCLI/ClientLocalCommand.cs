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
                    Connect(command);
                    break;

                case "register":
                    Register(command);
                    break;
                    
                case "ping":
                    Ping(command);
                    break;

                case "disconnect":
                    Disconnect();
                    break;

                case "clear":
                    Console.Clear();
                    Initialize();
                    break;

                case "changeuser":
                    ChangeUser(command);
                    break;

                case "exit":
                    Environment.Exit(0);
                    break;

                default:
                    WriteLine($"\\crInvalid command \"{command.command}\".");
                    break;
            }
        }

        static void ChangeUser(Command command)
        {
            if (command.arguments.Count != 1)
            {
                WriteLine("\\crLocal command \"changeuser\" only supports 1 argument:");
                WriteLine("\\cr  !changeuser <profilePath>");
                return;
            }
            if (client.Connected)
            {
                WriteLine("\\crYou can only change user after disconnected.");
                return;
            }
            try
            {
                client = null;
                ReadConfigureFile(command.arguments[0]);
                WriteLine($"Current user: \\br{client.nickname}\\rr({client.username})");
            }
            catch (Exception ex)
            {
                WriteLine($"\\crFailed to change user: {ex.Message}");
            }
        }

        static void Connect(Command command)
        {
            if (command.arguments.Count != 1)
            {
                WriteLine("\\crLocal command \"connect\" only supports 1 argument:");
                WriteLine("\\cr  !connect <address>[:port]");
                return;
            }
            try
            {
                IPEndPoint ep = Converter.GetIPEndPoint(command.arguments[0]);
                WriteLine($"\\erConnecting \\br{command.arguments[0]}\\rr(\\er{ep}\\rr)...");
                var response = client.Connect(ep);
                if (response.accpeted)
                    WriteLine($"Server accepted connection: {response.message}");
                else
                    WriteLine($"\\crServer refused connection: {response.message}");
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
                WriteLine("\\crLocal command \"register\" only supports 1 argument:");
                WriteLine("\\cr  !register <address>[:port]\n");
                return;
            }
            try
            {
                IPEndPoint ep = Converter.GetIPEndPoint(command.arguments[0]);
                WriteLine($"\\erConnecting \\br{command.arguments[0]}\\rr(\\er{ep}\\rr)...");
                var response = client.Register(ep);
                if (response.accpeted)
                    WriteLine($"Server accepted registration: {response.message}");
                else
                    WriteLine($"\\crServer refused registration: {response.message}");
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
                WriteLine("\\crLocal command \"register\" only supports 1 argument:");
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
    }
}
