using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{

    public partial class Server
    {
        void ProcessRequest(TcpClient client)
        {
            byte[] buffer = new byte[65536];
            var stream = client.GetStream();
            Packet request = Packet.ReceivePacket(stream, buffer);
            string requestType = request["requestType"];
            if (requestType == "ping")
            {
                ProcessPingRequest(client, request, buffer);
            }
            else if (requestType == "connect")
            {
                ProcessConnectRequest(client, request, buffer);
            }
            else if (requestType == "register")
            {
                ProcessRegisterRequest(client, request, buffer);
            }
        }
    }

}