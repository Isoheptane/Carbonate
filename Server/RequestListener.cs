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
        Thread listenerThread;

        /// <summary>
        /// Start listener with initialized listener object.
        /// </summary>
        void StartListener(TcpListener listener)
        {
            Info($"Starting listener at port {Port}...");
            listener.Server.DualMode = true;
            listener.Start();
            while (serverOn)
            {
                try
                {
                    if (!listener.Pending())
                    {   //< If there is no pending requests
                        Thread.Sleep(1);        //< Sleep for 1 ms
                        continue;
                    }
                    TcpClient client = listener.AcceptTcpClient();
                    Task.Run(() => { ProcessRequest(client); });
                }
                catch (Exception ex)
                {
                    Error($"Error occured when processing request: {ex.Message}");
                }
            }
            listener.Stop();
        }
    }

}