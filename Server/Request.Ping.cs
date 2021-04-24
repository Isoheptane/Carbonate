using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server {

    public partial class Server
    {
        void ProcessPingRequest(TcpClient client, Packet request, byte[] buffer)
        {
            var stream = client.GetStream();
            Packet response = new Packet();
            JsonSharp.JsonArray descriptionLines = new JsonSharp.JsonArray();
            foreach(string lines in Description)
                descriptionLines.elements.Add(lines);
            response["name"]        = Name;
            response["description"] = descriptionLines;
            response["program"]     = "Carbonate Hydro Alpha";
            response["online"]      = 0;
            response["maxOnline"]   = MaxOnline;
            Packet.SendPacket(stream, response);
            stream.Close();
            client.Close();
            client.Dispose();
        }
    }

}