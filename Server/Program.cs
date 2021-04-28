using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using JsonSharp;
using Carbonate.Standard;

namespace Carbonate.Server
{
    class Program
    {
        static Server server;
        static bool programStop = true;
        // On cancel key pressed, stop the server and quit program.
        static void OnCancelKeyPressed(object sender, ConsoleCancelEventArgs e)
        {
            server.Stop();
            programStop = false;
        }
        static void Main(string[] args)
        {
            ScreenIO.Info("Carbonate Hydro Beta v0.1.1");
            ScreenIO.Warn("This server-side program is still under development, use it at your own risk.");
            // Load config file
            JsonObject info = JsonObject.Parse(
                File.ReadAllText("workspace/server_config.json")
            );
            // Load server name
            string name = info["name"];
            // Load description;
            var descriptionArray =
                ((JsonArray)info["description"])
                .elements
                .ToArray();
            var description = new List<string>();
            foreach (var value in descriptionArray)
                description.Add(value);
            // Load other parameters
            int maxOnlineCount          = info["maxOnlineCount"];
            int port                    = info["port"];
            string workspace            = info["workspace"];
            bool allowRegister          = info["allowRegister"];
            int autosave                = info["autosave"];
            // Create server object
            server = new Server(
                name,
                description.ToArray(),
                maxOnlineCount,
                port,
                workspace,
                allowRegister,
                autosave
            );
            server.Start();

            // Add event bind
            Console.CancelKeyPress += OnCancelKeyPressed;

            while (programStop)
                Thread.Sleep(100);
        }
    }
}
