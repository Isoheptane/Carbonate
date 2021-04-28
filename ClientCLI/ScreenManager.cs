using System;
using System.Text;
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

        public static void WriteLine(string message)
        {
            lock (screenLock)
            {
                int interruptX = Console.CursorLeft;
                int interruptY = Console.CursorTop;

                int lines = MessageLines(Unescape(message));
                if (lines >= Console.BufferHeight / 3)
                {
                    message = TrimToLines(message, Console.BufferHeight / 3);
                    lines = MessageLines(Unescape(message));
                }
                Console.MoveBufferArea(0, lines, Console.BufferWidth, Console.BufferHeight - 3 - lines, 0, 0);
                Console.SetCursorPosition(0, Console.BufferHeight - 4 - lines);
                ScreenIO.Write(message + '\n');
                Console.SetCursorPosition(interruptX, interruptY);

            }
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

        public static string TrimToLines(string message, int targetLine)
        {
            StringBuilder trim = new StringBuilder();
            string[] lines = message.Split('\n');
            int rows = 0;
            foreach (string line in lines)
            {
                int col = 0;
                foreach (char ch in line)
                {
                    trim.Append(ch);
                    col++;
                    if(col >= Console.BufferWidth)
                    {
                        rows++;
                        col = 0;
                    }
                    if(rows > targetLine)
                        return trim.ToString();
                }
                rows++;
            }
            return trim.ToString();
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
