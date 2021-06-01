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
        public int keepAliveSendInterval = 2000;
        public DateTime serverKeepAlive;
        Thread keepAliveThread;
        void KeepAlive() => Send(new CommandPacket("keep-alive").ToPacket());

        public void BeginKeepAlive()
        {
            keepAliveThread = new Thread(() => {
                while (connected)
                {
                    try
                    {
                        Thread.Sleep(keepAliveSendInterval);
                        if(connected)
                            KeepAlive();
                    }
                    catch (Exception ex)
                    {
                        events.RaiseErrorEvent(
                            Thread.CurrentThread,
                            $"Error occured while sending keep-alive packet: {ex.Message}"
                        );
                    }
                }
            });
            keepAliveThread.Name = "keep_alive";
            keepAliveThread.Start();
        }
    }
}
