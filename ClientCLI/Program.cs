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
        static Client client;
        static void Main(string[] args)
        {
            Console.Clear();
            Initialize();
            string username, nickname, password;
            if (File.Exists("client_config.json"))
            {
                JsonObject config = JsonObject.Parse(File.ReadAllText("client_config.json"));
                username = config["username"];
                nickname = config["nickname"];
                password = PasswordHash.SHA256x7(config["password"]);
            }
            else
            {
                JsonObject config = new JsonObject();
                config["username"] = "username";
                config["nickname"] = "nickname";
                config["password"] = "password";
                File.WriteAllText("client_config.json", config.Serialize());
                WriteLine(
                    "\\crClient configure file doesn't exist!\n" +
                    "Please check newly created configure file\"client_config.json\".");
                Console.ReadLine();
                return;
            }
            client = new Client(username, nickname, password);
            WriteLine("Carbonate Client-CLI (Windows)");
            WriteLine($"User: \\br{nickname}\\rr({username})");
            while (true)
            {
                string input = Read("> ");
                if (input.Length >= 1 && input[0] == '!')
                {
                    input = input.Substring(1);
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
                    input = input.Substring(1);
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
