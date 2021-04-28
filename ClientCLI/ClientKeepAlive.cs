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
        public DateTime serverKeepAlive;
        Thread keepAliveThread;

        void KeepAlive() => Send(new CommandPacket("keep-alive").ToPacket());

        public void StartKeepAlive()
        {
            while (connected)
            {
                try
                {
                    Thread.Sleep(2000);
                    if(connected)
                        KeepAlive();
                }
                catch (Exception ex)
                {
                    WriteLine($"\\crError: Error occured while sending keep-alive packet: {ex.Message}");
                }
            }
        }
    }
}