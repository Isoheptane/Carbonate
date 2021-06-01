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

        static void ChangeLine() {
            Console.MoveBufferArea(0, 1, Console.BufferWidth, Console.BufferHeight - 4, 0, 0);
            Console.SetCursorPosition(0, Console.BufferHeight - 5);
        }

        public static void WriteSingleLine(string message)
        {
            lock (screenLock)
            {
                int interruptX = Console.CursorLeft;
                int interruptY = Console.CursorTop;

                ChangeLine();
                ScreenIO.Write('\n' + message);
                Console.SetCursorPosition(interruptX, interruptY);

            }
        }

        public static void WriteLine(string message)
        {
            string[] lines = GetLines(message);
            foreach (string line in lines)
                WriteSingleLine(line);
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
                for (int i = 0; i < line.Length; i++)
                {
                    char ch = line[i];
                    if (ch == '\\') {   //< Escape
                        if (
                            i + 2 < line.Length && 
                            IsLegalColorFormatter(line[i + 1]) &&
                            IsLegalColorFormatter(line[i + 2])
                        )
                        {
                            currentLine.Append(ch);
                            currentLine.Append(line[i + 1]);
                            currentLine.Append(line[i + 2]);
                            i += 2;
                            continue;
                        }
                        else if (
                            i + 1 < line.Length &&
                            IsLegalColorFormatter(line[i + 1])
                        )
                        {
                            currentLine.Append(ch);
                            currentLine.Append(line[i + 1]);
                            i += 1;
                            continue;
                        }
                        else if (
                            i + 1 < line.Length &&
                            line[i + 1] == '\\'
                        )
                        {
                            currentLine.Append('\\');
                            currentLine.Append('\\');
                            length += 1;
                            i += 1;
                            continue;
                        }
                    }
                    int charLength = ch < 256 ? 1 : 2;
                    if (length + charLength >= Console.BufferWidth)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                        length = charLength;
                        currentLine.Append(ch);
                    }
                    else
                    {
                        length += charLength;
                        currentLine.Append(ch);
                    }
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
