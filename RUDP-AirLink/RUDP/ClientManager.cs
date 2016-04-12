using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RUDP_AirLink.Utils;

namespace RUDP_AirLink.RUDP
{
    class ClientManager
    {
        Dictionary<int, ClientControl> ClientTable = new Dictionary<int, ClientControl>();

        Thread MainThread;

        Route MyRoute;

        int ReceivePingTimeout = 8 * 1000;
        int SendPingInterval = 1 * 1000;

        object SynClientTable = new Object();

        public ClientManager(Route route)
        {
            MyRoute = route;
            MainThread = new Thread(new ThreadStart(() => {
                while (true)
                {
                    // try
                    Thread.Sleep(1000);
                    ScanClientControl();
                }
            }));
            MainThread.Start();
        }

        void ScanClientControl()
        {
            long current = DateTimeExtensions.CurrentTimeMillis();

            foreach (KeyValuePair<int, ClientControl> ccItem in ClientTable)
            {
                ClientControl cc = ccItem.Value;
                if (cc != null)
                {
                    if (current - cc.LastReceivePingTime < ReceivePingTimeout &&
                        current - cc.LastReceivePingTime > SendPingInterval)
                    {
                        cc.SendPingPacket();
                    }
                }
                else
                {
                    //Timtout
                    Console.WriteLine("Timeout and close client: " + cc.DstHost + ":" + cc.DstPort + " " + DateTime.Now.ToString());

                    lock (SynClientTable)
                    {
                        cc.Close();
                    }
                }
            }
        }

        void RemoveClient(int clientId) => ClientTable.Remove(clientId);

        ClientControl GetClientControl(int clientId, string dstHost, int dstPort)
        {
            ClientControl c = ClientTable[clientId];
            if (c == null)
            {
                c = new ClientControl(MyRoute, clientId, dstHost, dstPort);
                
                lock (SynClientTable)
                {
                    ClientTable.Add(clientId, c);
                }
            }
            return c;
        }
    }
}
