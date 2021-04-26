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
        bool IsLegalNickname(string username)
        {
            foreach (char ch in username)
            {
                if ((ch < 32 || ch > 126) && ch < 256)
                    return false;
                else
                    continue;
            }
            return true;
        }
        bool IsLegalUsername(string username)
        {
            foreach (char ch in username)
            {
                if (ch < 32 || ch > 126)
                    return false;
                else
                    continue;
            }
            return true;
        }
        void ProcessRegisterRequest(TcpClient client, Packet request, byte[] buffer)
        {
            var stream = client.GetStream();
            string username = request["username"];
            string password = request["password"];
            Packet response = new Packet();
            response["accepted"] = false;
            if (!AllowRegister)
            {
                response["message"] =
                "The server doesn't allow register.";
            }
            else if (Users.ContainsKey(username))
            {
                response["message"] =
                "The user already exists.";
            }
            else if (!IsLegalUsername(username))
            {
                response["message"] =
                "Username contains invalid character.";
            }
            else if (request.ToJsonObject().Exist("nickname") && !IsLegalNickname(request["nickname"]))
            {
                response["message"] =
                "Nickname contains invalid character.";
            }
            else
            {
                response["accepted"] = true;
                response["message"] =
                "Registered.";
            }
            Packet.SendPacket(stream, response);
            if (response["accepted"] == true)
            {
                ServerUser user = new ServerUser();
                user.username = username;
                user.registerTime = DateTime.Now;
                user.password = password;
                if (request.ToJsonObject().Exist("nickname"))
                    user.nickname = request["nickname"];
                else
                    user.nickname = username;
                Users.TryAdd(username, user);
                Connect(username, client);
                Info($"User \"{username}\" from {client.Client.RemoteEndPoint} registered.");
            }
            else
            {
                Info($"User \"{username}\" from {client.Client.RemoteEndPoint} failed to register: {response["message"]}");
                stream.Close();
                client.Close();
            }
        }
    }

}