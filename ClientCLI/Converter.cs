using System;
using System.Net;
using System.Text;

namespace ClientCLI
{
    public static class Converter
    {
        public static IPEndPoint GetIPEndPoint(string ep)
        {
            IPAddress address;
            if (ep.LastIndexOf(':') == -1)
            {
                address = Dns.GetHostEntry(ep).AddressList[0];
                return new IPEndPoint(address, 7235);
            }
            else
            {
                int position = ep.LastIndexOf(':');
                string domain = ep.Substring(0, position);
                string port = ep.Substring(position + 1);
                address = Dns.GetHostEntry(domain).AddressList[0];
                return new IPEndPoint(address, int.Parse(port));
            }
        }

        public static string GetHostname(string ep)
        {
            if (ep.LastIndexOf(':') == -1)
                return ep;
            else
                return ep.Substring(0, ep.LastIndexOf(':'));
        }
    }
}
