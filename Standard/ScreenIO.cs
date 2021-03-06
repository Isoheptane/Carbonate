using System;
using System.Text;
using System.Threading.Tasks;
using JsonSharp;

namespace Carbonate.Standard
{
    public static class ScreenIO
    {

        static object screenLock = new object();

        public static bool IsLegalColorFormatter(char colorChar)
        {
            if (colorChar >= '0' && colorChar <= '9')
                return true;
            else if (colorChar >= 'A' && colorChar <= 'F')
                return true;
            else if (colorChar >= 'a' && colorChar <= 'f')
                return true;
            else if (colorChar == 'r' || colorChar == 'R')
                return true;
            else
                return false;
        }

        private static ConsoleColor ConvertColor(char colorChar, ConsoleColor foreground, ConsoleColor background, bool isForeground)
        {
            if (colorChar >= '0' && colorChar <= '9')        //< 0-9 Color foramtter
                return (ConsoleColor)(colorChar - '0');
            else if (colorChar >= 'A' && colorChar <= 'F')   //< A-F Color formatter
                return (ConsoleColor)(colorChar - 'A' + 10);
            else if (colorChar >= 'a' && colorChar <= 'f')   //< a-f Color formatter
                return (ConsoleColor)(colorChar - 'a' + 10);
            else if (colorChar == 'r' || colorChar == 'R')   //< Restore color
                if (isForeground)
                    return foreground;
                else
                    return background;
            else                                            //< This is not supposed to run
                throw new Exception("The color formatter char is illegal.");
        }

        /// <summary>
        /// Write the message to the screen with color formatter and indicated default foreground / background color.
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="foreground">Default foreground color</param>
        /// <param name="background">Default background color</param>
        public static void RawWrite(string message, ConsoleColor foreground, ConsoleColor background)
        {
            lock (screenLock)
            {
                for (int currentPtr = 0; currentPtr < message.Length; currentPtr++)
                {
                    if (message[currentPtr] == '\\')    //< When meets escape
                    {
                        if (                            //< Foreground / Background color formatter
                            currentPtr + 2 < message.Length &&
                            IsLegalColorFormatter(message[currentPtr + 1]) &&
                            IsLegalColorFormatter(message[currentPtr + 2])
                        )
                        {
                            Console.ForegroundColor = ConvertColor(message[currentPtr + 1], foreground, background, true);
                            Console.BackgroundColor = ConvertColor(message[currentPtr + 2], foreground, background, false);
                            currentPtr += 2;
                        }
                        else if (                       //< Foreground color formatter
                            currentPtr + 1 < message.Length &&
                            IsLegalColorFormatter(message[currentPtr + 1])
                        )
                        {
                            Console.ForegroundColor = ConvertColor(message[currentPtr + 1], foreground, background, true);
                            currentPtr += 1;
                        }
                        else if (                       //< '/' Escape
                            currentPtr + 1 < message.Length &&
                            message[currentPtr + 1] == '\\'
                        )
                        {
                            Console.Write('\\');
                            currentPtr += 1;
                        }
                    }
                    else                                //< Regular character
                    {
                        Console.Write(message[currentPtr]);
                    }
                }
            }
        }

        /// <summary>
        /// Write the message to the screen with color formatter.
        /// </summary>
        /// <param name="message">The message to be written</param>
        public static void Write(string message)
        {
            var foreground = Console.ForegroundColor; //< Backup original console color
            var background = Console.BackgroundColor; //

            RawWrite(message, foreground, background);

            Console.ForegroundColor = foreground; //< Restore console color
            Console.BackgroundColor = background; //
        }
        
        /// <summary>
        /// Gets the actual string to output.
        /// </summary>
        public static string Escape(string message)
        {
            StringBuilder escaped = new StringBuilder();
            for (int currentPtr = 0; currentPtr < message.Length; currentPtr++)
            {
                if (message[currentPtr] == '\\')    //< When meets escape
                {
                    if (                            //< Foreground / Background color formatter
                        currentPtr + 2 < message.Length &&
                        IsLegalColorFormatter(message[currentPtr + 1]) &&
                        IsLegalColorFormatter(message[currentPtr + 2])
                    )
                    {
                        currentPtr += 2;
                    }
                    else if (                       //< Foreground color formatter
                        currentPtr + 1 < message.Length &&
                        IsLegalColorFormatter(message[currentPtr + 1])
                    )
                    {
                        currentPtr += 1;
                    }
                    else if (                       //< '/' Escape
                        currentPtr + 1 < message.Length &&
                        message[currentPtr + 1] == '\\'
                    )
                    {
                        escaped.Append(message[currentPtr]);
                        currentPtr += 1;
                    }
                }
                else                                //< Regular character
                {
                    escaped.Append(message[currentPtr]);
                }
            }
            return escaped.ToString();
        }

        /// <value>The current time represented as hh:mm:ss format.</value>
        public static string CurrentTimeString
        {
            get
            {
                return DateTime.Now.ToString("HH:mm:ss");
            }
        }

        /// <summary>
        /// Write a info message.
        /// </summary>
        public static void Info(string message) => Write($"\\rr[{CurrentTimeString} INFO]\\rr {message}\\rr\n");

        /// <summary>
        /// Write a warning message.
        /// </summary>
        public static void Warn(string message) => Write($"\\er[{CurrentTimeString} WARN]\\rr {message}\\rr\n");

        /// <summary>
        /// Write a error message.
        /// </summary>
        public static void Error(string message) => Write($"\\cr[{CurrentTimeString} ERROR]\\rr {message}\\rr\n");

        /// <summary>
        /// Write a debug message.
        /// </summary>
        public static void Debug(string message) => Write($"\\rr[{CurrentTimeString} DEBUG]\\rr {message}\\rr\n");

        /// <summary>
        /// Unescape message.
        /// </summary>
        public static string Unescape(string message) => message.Replace("\\", "\\\\");

    }
}
