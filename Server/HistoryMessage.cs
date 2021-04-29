using System;
using System.Collections.Generic;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{
    public partial class Server
    {
        int historyCapacity;
        
        Queue<Packet> historyMessage;

        public void EnqueueHistory(Packet packet)
        {
            historyMessage.Enqueue(packet);
            if(historyMessage.Count > historyCapacity)
                historyMessage.Dequeue();
        }

        public void SendHistory(OnlineUser user)
        {
            Packet packet = new Packet();
            packet["messageType"] = "history";
            JsonSharp.JsonArray array = new JsonSharp.JsonArray();
            foreach (Packet message in historyMessage)
                array.elements.Add(message.ToJsonObject());
            packet["messages"] = array;
            user.Send(packet);
        }

    }
}
