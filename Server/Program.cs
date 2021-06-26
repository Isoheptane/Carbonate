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
            ScreenIO.Info("Carbonate Hydro Beta v0.3.0");
            ScreenIO.Warn("This server-side program is still under development, use it at your own risk.");
            // Create server object
            server = new Server(JsonObject.Parse(File.ReadAllText("server_config.json")));
            server.Start();

            // Add event bind
            Console.CancelKeyPress += OnCancelKeyPressed;

            while (programStop)
                Thread.Sleep(100);
        }
    }
}
