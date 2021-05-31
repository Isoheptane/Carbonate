using System;
using System.Threading;
using Carbonate.Standard;

namespace Carbonate.Client
{
    public partial class Client
    {
        Thread messageListener;

        void BeginListener()
        {
            messageListener = new Thread(ListenMessage);
            messageListener.Name = "message_listener";
            messageListener.Start();
        }

        void ListenMessage()
        {
            while (Connected)
            {
                if (DateTime.Now > serverKeepAlive)
                {
                    events.RaiseErrorEvent(Thread.CurrentThread, "Disconnected due to server keep-alive.");
                    Disconnect();
                    break;
                }
                if (!DataAvailable)
                {
                    Thread.Sleep(1);
                    continue;
                }
                try {
                    Packet packet = Receive();
                    switch ((string)packet["messageType"])
                    {
                        case "chat":
                        case "whisper":
                        case "action":
                        case "server":
                        case "broadcast":
                        case "history":
                            events.RaiseOnMessageEvent(packet);
                            break;

                        case "keep-alive":
                            serverKeepAlive = DateTime.Now.AddMilliseconds(5000);
                            break;

                        case "disconnect":
                            events.RaiseServerDisconnectEvent(packet);
                            Disconnect();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    events.RaiseErrorEvent(Thread.CurrentThread, $"Error occured while receiving message: {ex.Message}");
                }
            }
        }

    }
}