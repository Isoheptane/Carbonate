using System;
using System.Net;
using System.Text;

namespace ClientCLI
{
    public static class Converter
    {
        public static IPEndPoint GetIPEndPoint(string ep)
        {
            IPEndPoint endPoint;
            IPAddress address;
            if (IPEndPoint.TryParse(ep, out endPoint))
                return endPoint;
            else if (IPAddress.TryParse(ep, out address))
                return new IPEndPoint(address, 7235);
            else if (ep.LastIndexOf(':') == -1)
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
    }
}
