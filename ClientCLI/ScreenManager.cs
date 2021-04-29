using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace ClientCLI
{
    public class ScreenManager
    {
        public static object screenLock = new object();
        public static void Initialize()
        {
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
            Console.SetCursorPosition(0, Console.BufferHeight - 3);
        }

        public static void WriteSingleLine(string message)
        {
            lock (screenLock)
            {
                int interruptX = Console.CursorLeft;
                int interruptY = Console.CursorTop;

                int lines = 1;
                Console.MoveBufferArea(0, 1, Console.BufferWidth, Console.BufferHeight - 4, 0, 0);
                Console.SetCursorPosition(0, Console.BufferHeight - 4 - lines);
                ScreenIO.Write(message + '\n');
                Console.SetCursorPosition(interruptX, interruptY);

            }
        }

        public static void WriteLine(string message)
        {
            string[] lines = GetLines(message);
            Task.Run(() => 
            {
                foreach (string line in lines)
                    WriteSingleLine(line);
            });
        }

        public static string Read(string prompt = "")
        {
            lock (screenLock)
            {
                Console.SetCursorPosition(0, Console.BufferHeight - 3);
                Console.Write(prompt);
            }
            string str = Console.ReadLine();
            lock (screenLock)
            {
                Console.SetCursorPosition(0, Console.BufferHeight - 3);
                Console.Write(new string(' ', StringCharLength(str) + StringCharLength(prompt)));
                Console.SetCursorPosition(0, Console.BufferHeight - 3);
            }
            return str;
        }

        public static int MessageLines(string message)
        {
            string[] lines = message.Split('\n');
            int rows = lines.Length;
            foreach (string line in lines)
            {
                rows += StringCharLength(message) / Console.BufferWidth;
            }
            return rows;
        }

        public static string[] GetLines(string message)
        {
            string[] originalLines = message.Split('\n');
            List<string> lines = new List<string>();
            foreach (string line in originalLines)
            {
                StringBuilder currentLine = new StringBuilder();
                int length = 0;
                foreach (char ch in line)
                {
                    int charLength = ch < 256 ? 1 : 2;
                    if (length + charLength >= Console.BufferWidth)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                        length = charLength;
                        currentLine.Append(ch);
                    }
                    else
                        currentLine.Append(ch);
                }
                lines.Add(currentLine.ToString());
            }
            return lines.ToArray();
        }

        public static int StringCharLength(string str)
        {
            int length = 0;
            foreach (char ch in str)
            {
                if (ch < 256)
                    length += 1;
                else
                    length += 2;
            }
            return length;
        }
    }
}
