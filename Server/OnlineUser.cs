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
        DateTime keepalive;
        Thread daemonThread;
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
        /// Keep user connection alive.
        /// </summary>
        public void KeepAlive()
        {
            keepalive = DateTime.Now.AddSeconds(5);
        }

        /// <summary>
        /// Send a packet to the user
        /// </summary>
        /// <param name="packet">The packet to be sent</param>
        public void Send(Packet packet)
        {
            if (Connected)
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

        void StartDaemon(Carbonate.Server.Server server)
        {
            while (Connected)
            {
                Thread.Sleep(1);
                if(DateTime.Now > keepalive)
                {
                    server.Disconnect(Username);
                    stream.Close();
                    client.Close();
                    client.Dispose();
                    connected = false;
                    ScreenIO.Info($"User \"{Username}\" was kicked from the server due to keep-alive.");
                    break;
                }
                try {
                    lock (stream)
                    {
                        if(!stream.DataAvailable)
                        {
                            continue;
                        }
                        Packet packet = Packet.ReceivePacket(stream, receiveBuffer);
                        server.ProcessUserPacket(this, packet);
                    }
                }
                catch (Exception ex)
                {
                    ScreenIO.Error($"Error occured in user daemon thread of \"{Username}\": {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Begin daemon thread.
        /// </summary>
        /// <param name="server">Server object</param>
        public void BeginDaemon(Carbonate.Server.Server server)
        {
            daemonThread = new Thread(() => { StartDaemon(server); });
            daemonThread.IsBackground = true;
            daemonThread.Start();
        }

    }
}
