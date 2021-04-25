using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using JsonSharp;

namespace Carbonate.Standard
{
    public class CommandPacket
    {
        public string command;
        public List<string> arguments;

        /// <summary>
        /// Create a empty command packet.
        /// </summary>
        public CommandPacket()
        {
            command = "";
            arguments = new List<string>();
        }

        /// <summary>
        /// Create a command packet with empty arguments.
        /// </summary>
        public CommandPacket(string command)
        {
            this.command = command;
            this.arguments = new List<string>();
        }

        /// <summary>
        /// Create a command packet.
        /// </summary>
        public CommandPacket(string command, params string[] arguments)
        {
            this.command = command;
            this.arguments = new List<string>();
            foreach (string argument in arguments)
                this.arguments.Add(argument);
        }

        /// <summary>
        /// Create a command packet from a packet.
        /// </summary>
        /// <param name="json">Packet object</param>
        public CommandPacket(Packet packet)
        {
            command = packet["command"];
            arguments = new List<string>();
            foreach (var value in ((JsonArray)packet["arguments"]).elements)
                arguments.Add(value);
        }

        /// <summary>
        /// Convert command packet into a packet object.
        /// </summary>
        public Packet ToPacket()
        {
            Packet packet = new Packet();
            packet["command"] = command;
            JsonArray args = new JsonArray();
            foreach (string argument in arguments)
                args.elements.Add(argument);
            packet["arguments"] = args;
            return packet;
        }

        public static implicit operator Packet(CommandPacket command)
        {
            return command.ToPacket();
        }

    }
}
