using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using JsonSharp;

namespace Carbonate.Standard
{
    public static class TypeConvert
    {
        
        /// <summary>
        /// Get time from a string in yy:MM:dd:hh:mm:ss format
        /// </summary>
        /// <param name="format">Source format string</param>
        /// <param name="time">Out time</param>
        public static bool TryGetTime(string format, out DateTime time)
        {
            time = DateTime.MinValue;
            string[] strValues = format.Split(':');
            int[] values = new int[strValues.Length];

            if (strValues.Length == 0 || strValues.Length > 6)
                return false;

            for (int i = 0; i < values.Length; i++) 
            {
                if (!int.TryParse(strValues[i], out values[i]))
                    return false;
            }

            for (int i = values.Length - 1; i >= 0; i--)
            {
                switch (values.Length - i)
                {
                    case 1:
                        time = time.AddSeconds(values[i]);
                        break;
                    case 2:
                        time = time.AddMinutes(values[i]);
                        break;
                    case 3:
                        time = time.AddHours(values[i]);
                        break;
                    case 4:
                        time = time.AddDays(values[i]);
                        break;
                    case 5:
                        time = time.AddMonths(values[i]);
                        break;
                    case 6:
                        time = time.AddYears(values[i]);
                        break;
                }
            }
            return true;

        }

    }
}
