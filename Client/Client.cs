using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Carbonate.Standard;
using System.Security.Cryptography;

namespace Carbonate.Client
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

        /// <value>Return the status of connection</value>
        public bool Connected
        {
            get { return connected; }
        }

        /// <summary>
        /// Create a client object from indicated user informations
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="nickname">Nickname</param>
        /// <param name="password">Hashed password</param>
        public Client(string username, string nickname, string password)
        {
            this.username = username;
            this.nickname = nickname;
            this.password = password;
        }

        /// <summary>
        /// Create TcpClient object
        /// </summary>
        TcpClient CreateClient()
        {
            TcpClient client = new TcpClient();
            client.Client.DualMode = true;
            return client;
        }

        /// <summary>
        /// Dispose TcpClient object
        /// </summary>
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
        public RequestResponse Connect(IPEndPoint remote)
        {
            RequestResponse status;
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
            while (!DataAvailable)
            {
                Thread.Sleep(1);
                continue;
            }
            Packet response = Receive();
            string message = response["message"];
            if(response["accepted"] == true) 
            {
                connected = true;
                status = new RequestResponse(true, message);
                serverKeepAlive = DateTime.Now.AddMilliseconds(5000);
                BeginListener();
                BeginKeepAlive();
            }
            else
            {
                status = new RequestResponse(false, message);
                DisposeClient();
            }
            return status;
        }

        /// <summary>
        /// Register at the indicated server.
        /// </summary>
        /// <param name="remote">Remote server</param>
        public RequestResponse Register(IPEndPoint remote)
        {
            RequestResponse status;
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
            while (!DataAvailable)
            {
                Thread.Sleep(1);
                continue;
            }
            Packet response = Receive();
            string message = response["message"];
            if(response["accepted"] == true) 
            {
                connected = true;
                status = new RequestResponse(true, message);
                serverKeepAlive = DateTime.Now.AddMilliseconds(5000);
                BeginListener();
                BeginKeepAlive();
            }
            else
            {
                status = new RequestResponse(false, message);
                DisposeClient();
            }
            return status;
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
            while (!pingStream.DataAvailable)
            {
                Thread.Sleep(1);
                continue;
            }
            Packet response = Receive();
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
                events.RaiseErrorEvent(
                    Thread.CurrentThread, 
                    $"Error occured while sending disconnect message: {ex.Message}"
                );
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
