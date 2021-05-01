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
        byte[] pingBuffer = new byte[256000];
        byte[] buffer = new byte[256000];
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

        TcpClient CreateClient()
        {
            TcpClient client = new TcpClient();
            client.Client.DualMode = true;
            return client;
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
            if (Connected)
                Disconnect();
            client = CreateClient();
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
                serverKeepAlive = DateTime.Now.AddMilliseconds(5000);
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
            if (Connected)
                Disconnect();
            client = CreateClient();
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
                serverKeepAlive = DateTime.Now.AddMilliseconds(5000);
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
            TcpClient pingClient = CreateClient();
            pingClient.Connect(remote);
            NetworkStream pingStream = pingClient.GetStream();
            Packet request = new Packet();
            request["requestType"] = "ping";
            request["username"] = username;
            Packet.SendPacket(pingStream, request);
            Packet result = Packet.ReceivePacket(pingStream, pingBuffer);
            pingStream.Close();
            pingClient.Close();
            pingClient.Dispose();
            pingStream = null;
            pingClient = null;
            return result;
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                Send(new CommandPacket("disconnect").ToPacket());
            }
            catch (Exception ex)
            {
                WriteLine($"Error: Error occured while sending disconnect message: {ex.Message}");
            }
            DisposeClient();
            connected = false;
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void ForceDisconnect()
        {
            DisposeClient();
            connected = false;
        }

    }
}
