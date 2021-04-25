using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using JsonSharp;

namespace Carbonate.Standard
{
    public class OnlineUser
    {
        byte[] receiveBuffer = new byte[65536];
        bool connected;
        string username;
        TcpClient client;
        NetworkStream stream;

        public bool Connected
        {
            get { return connected; }
        }

        public string Username
        {
            get { return username; }
        }

        /// <summary>
        /// Create a online user object with indicated username and client object
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="client">Remote client object</param>
        public OnlineUser(string username, TcpClient client)
        {
            connected = true;
            this.username = username;
            this.client = client;
            this.stream = client.GetStream();
        }

        /// <summary>
        /// Send a packet to the user
        /// </summary>
        /// <param name="packet">The packet to be sent</param>
        public void Send(Packet packet)
        {
            if (connected)
            {
                lock (stream)
                {
                    Packet.SendPacket(stream, packet);
                }
            }
            else
            {
                throw new Exception($"User {username} from {client.Client.RemoteEndPoint} is disconnected.");
            }
        }

    }
}
