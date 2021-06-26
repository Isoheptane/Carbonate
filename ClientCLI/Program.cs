using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using Carbonate.Standard;
using Carbonate.Client;
using JsonSharp;
using static ClientCLI.ScreenManager;

namespace ClientCLI
{
    partial class Program
    {
        static Client client;

        static void ReadConfigureFile(string path)
        {
            string username, nickname, password;

            JsonObject config = JsonObject.Parse(File.ReadAllText(path));
            username    = config["username"];
            nickname    = config["nickname"];
            password    = PasswordHash.SHA256x7(config["password"]);

            client = new Client(username, nickname, password);
            //  Register Events
            client.events.OnMessageEvent += OnReceiveMessage;
            client.events.ErrorEvent += OnError;
            client.events.ServerDisconnectEvent += ServerDisconnect;
        }
        static void Main(string[] args)
        {
            Console.Clear();
            Initialize();
            if (File.Exists("default_user.json"))
            {
                ReadConfigureFile("default_user.json");
            }
            else
            {
                JsonObject config = new JsonObject();
                config["username"]  = "username";
                config["nickname"]  = "nickname";
                config["password"]  = "password";
                File.WriteAllText("default_user.json", config.Serialize());
                WriteLine(
                    "\\crDefalut user profile doesn't exist!\n" +
                    "Please check the newly created user profile \"defalut_user.json\".");
                Console.ReadLine();
                return;
            }

            //  Splash
            WriteLine("Carbonate ClientCLI");
            WriteLine($"Current user: \\br{client.nickname}\\rr({client.username})");
            while (true)
            {
                string input = Read("> ");
                if (input.Length >= 1 && input[0] == '!')
                {
                    input = input.Substring(1).Trim();
                    Command command = new Command(input);
                    try
                    {
                        LocalCommand(command);
                    }
                    catch (Exception ex)
                    {
                        WriteLine($"\\crError: Error occured while executing local command: {ex.Message}");
                    }
                }
                else if (input.Length >= 1 && input[0] == '/')
                {
                    input = input.Substring(1).Trim();
                    Command command = new Command(input);
                    if (client.Connected)
                    {
                        try
                        {
                            client.Send(command.ToCommandPacket().ToPacket());
                        }
                        catch (Exception ex)
                        {
                            WriteLine($"\\crError: Error occured while executing remote command: {ex.Message}");
                        }
                    }
                    else
                    {
                        WriteLine($"\\crYou are currently offline.");
                    }
                }
                else if (input != "")
                {
                    if (client.Connected)
                    {
                        try
                        {
                            client.Send(new CommandPacket("say", input).ToPacket());
                        }
                        catch (Exception ex)
                        {
                            WriteLine($"\\crError: Error occured while executing remote command: {ex.Message}");
                        }
                    }
                    else
                    {
                        WriteLine($"\\crYou are currently offline.");
                    }
                }
            }
        }

    }
}
