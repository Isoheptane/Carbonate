using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Carbonate.Standard;
using System.Security.Cryptography;
using static ClientCLI.ScreenManager;

namespace ClientCLI
{
    public partial class Client
    {
        bool connected = false;
        byte[] buffer = new byte[65536];
        public string username;
        public string nickname;
        public string password;
        TcpClient client;
        NetworkStream stream;

        public bool Connected
        {
            get { return connected; }
        }
        public Client(string username, string nickname, string password)
        {
            this.username = username;
            this.nickname = nickname;
            this.password = password;
        }

        void CreateClient()
        {
            client = new TcpClient();
            client.Client.DualMode = true;
        }

        void DisposeClient()
        {
            if (stream != null)
                stream.Close();
            client.Close();
            client.Dispose();
            stream = null;
            client = null;
        }

        public bool DataAvailable
        {
            get { return stream.DataAvailable; }
        }

        public void Send(Packet packet)
        {
            lock(stream)
            {
                Packet.SendPacket(stream, packet);
            }
        }

        public Packet Receive()
        {
            return Packet.ReceivePacket(stream, buffer);
        }

        /// <summary>
        /// Connect to the indicated server.
        /// </summary>
        /// <param name="remote">Remote server</param>
        public bool Connect(IPEndPoint remote)
        {
            CreateClient();
            client.Connect(remote);
            stream = client.GetStream();
            Packet request = new Packet();
            request["requestType"] = "connect";
            request["username"] = username;
            request["password"] = password;
            Send(request);
            Packet response = Receive();
            string message = response["message"];
            if(response["accepted"] == true) 
            {
                connected = true;
                WriteLine($"Server accepted connection: {message}");
            }
            else
            {
                WriteLine($"\\crServer refused connection: {message}");
                DisposeClient();
            }
            keepAliveThread = new Thread(StartKeepAlive);
            keepAliveThread.Start();
            return response["accepted"];
        }

        /// <summary>
        /// Register at the indicated server.
        /// </summary>
        /// <param name="remote">Remote server</param>
        public bool Register(IPEndPoint remote)
        {
            CreateClient();
            client.Connect(remote);
            stream = client.GetStream();
            Packet request = new Packet();
            request["requestType"] = "register";
            request["username"] = username;
            request["nickname"] = nickname;
            request["password"] = password;
            Send(request);
            Packet response = Receive();
            string message = response["message"];
            if(response["accepted"] == true) 
            {
                connected = true;
                WriteLine($"Server accepted register: {message}");
            }
            else
            {
                WriteLine($"\\crServer refused register: {message}");
                DisposeClient();
            }
            keepAliveThread = new Thread(StartKeepAlive);
            keepAliveThread.Start();
            return response["accepted"];
        }

        /// <summary>
        /// Ping remote server
        /// </summary>
        /// <param name="remote">Remote server</param>
        public Packet Ping(IPEndPoint remote)
        {
            CreateClient();
            client.Connect(remote);
            stream = client.GetStream();
            Packet request = new Packet();
            request["requestType"] = "ping";
            request["username"] = username;
            Send(request);
            Packet result = Receive();
            DisposeClient();
            return result;
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            DisposeClient();
            connected = false;
        }

    }
}
