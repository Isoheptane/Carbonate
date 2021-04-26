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
        void ProcessConnectRequest(TcpClient client, Packet request, byte[] buffer)
        {
            var stream = client.GetStream();
            string username = request["username"];
            string password = request["password"];
            Info($"Incoming connection from user \"{username}\" {client.Client.RemoteEndPoint}.");
            Packet response = new Packet();
            response["accepted"] = false;
            if (!Users.ContainsKey(username))
            {   //  User doesn't exist
                response["message"] =
                "The user doesn't exist.";
            }
            else if (users[username].password != password)
            {   //  Incorrect password
                response["message"] =
                "Password incorrect.";
            }
            else if (Users[username].banTime > DateTime.Now)
            {   //  Ban user
                response["message"] =
                $"You are banned from the server until {Users[username].banTime.ToString("yyyy/MM/dd HH:mm:ss")}";
            }
            else if (onlineUsers.ContainsKey(username))
            {   //  Already login
                response["message"] =
                "User already login.";
            }
            else if (onlineUsers.Count >= MaxOnline)
            {   //  Server full
                response["message"] =
                "The server is full.";
            }
            else
            {
                response["accepted"] = true;
                response["message"] = "Welcome";
            }
            Packet.SendPacket(stream, response);
            if (response["accepted"] == true)
            {
                Connect(username, client);
            }
            else
            {
                Info($"User \"{username}\" from {client.Client.RemoteEndPoint} failed to connect: {response["message"]}");
                stream.Close();
                client.Close();
            }
        }
    }

}