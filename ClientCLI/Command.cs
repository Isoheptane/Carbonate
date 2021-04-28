using System;
using System.Collections.Generic;
using System.Text;
using Carbonate.Standard;

namespace ClientCLI
{
    // Remote commands are indicated with a initial '/'
    public class Command
    {
        public string command;
        public List<string> arguments;

        public Command(string input)
        {
            string[] array = CommandReader.ReadStringArray(input);
            command = array[0];
            arguments = new List<string>();
            for (int i = 1; i < array.Length; i++)
                arguments.Add(array[i]);
        }

        /// <summary>
        /// Convert this command into a packet
        /// </summary>
        public CommandPacket ToCommandPacket()
        {
            return new CommandPacket(command, arguments.ToArray());
        }
    }
}
