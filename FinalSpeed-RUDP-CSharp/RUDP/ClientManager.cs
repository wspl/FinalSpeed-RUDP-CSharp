using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FinalSpeed_RUDP_CSharp.Utils;

namespace FinalSpeed_RUDP_CSharp.RUDP
{
    public class ClientManager
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
                ClientControl clientControl = ccItem.Value;
                if (clientControl != null)
                {
                    if (current - clientControl.LastReceivePingTime < ReceivePingTimeout &&
                        current - clientControl.LastReceivePingTime > SendPingInterval)
                    {
                        clientControl.SendPingPacket();
                    }
                }
                else
                {
                    //Timtout
                    Console.WriteLine("Timeout and close client: " 
                        + clientControl.DstHost + ":" + clientControl.DstPort 
                        + " " + DateTime.Now.ToString());

                    lock (SynClientTable)
                    {
                        clientControl.Close();
                    }
                }
            }
        }

        public void RemoveClient(int clientId) => ClientTable.Remove(clientId);

        public ClientControl GetClientControl(int clientId, string dstHost, int dstPort)
        {
            ClientControl clientControl = ClientTable[clientId];
            if (clientControl == null)
            {
                clientControl = new ClientControl(MyRoute, clientId, dstHost, dstPort);
                
                lock (SynClientTable)
                {
                    ClientTable.Add(clientId, clientControl);
                }
            }
            return clientControl;
        }
    }
}
