using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{
    public partial class Server
    {

        ConcurrentDictionary<string, OnlineUser> onlineUsers = new ConcurrentDictionary<string, OnlineUser>();

        /// <value>Online users on the server.</value>
        public ConcurrentDictionary<string, OnlineUser> OnlineUsers
        {
            get { return onlineUsers; }
        }

        /// <summary>
        /// Add a new user to the online user list
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="client">Remote client object</param>
        public void Connect(string username, TcpClient client)
        {
            onlineUsers.TryAdd(username, new OnlineUser(username, client));
            onlineUsers[username].KeepAlive();
            onlineUsers[username].BeginDaemon(this);
            User backendUser = Users[username];
            Broadcast("server", $"\\er{backendUser.nickname}\\rr({username})\\rr joined the server.");
            Info($"User \\er{backendUser.nickname}\\rr({username})\\rr from {client.Client.RemoteEndPoint} connected.");
        }

        /// <summary>
        /// Remove user from the online user list
        /// </summary>
        /// <param name="username">Username</param>
        public void Disconnect(string username)
        {
            OnlineUser rec;
            onlineUsers.TryRemove(username, out rec);
            User backendUser = Users[username];
            Broadcast("server", $"\\er{backendUser.nickname}\\rr({username})\\rr left the server.");
            rec.Disconnect();
            Info($"User \\er{backendUser.nickname}\\rr({username})\\rr disconnected.");
        }
    }
}
