using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RUDP_AirLink.Utils;
using System.Collections.Concurrent;
using System.Threading;

namespace RUDP_AirLink.RUDP
{
    class ConnectionUDP
    {
        public string DstHost;
        public int DstPort;
        public Sender MySender;
        public Receiver MyRecevier;
        public UDPOutputStream Uos;
        public UDPInputStream Uis;

        long ConnectionId;
        public Route MyRoute;

        int Mode;

        private bool _connected = true;
        public bool IsConnected
        {
            get
            {
                return _connected;
            }
        }

        long LastLiveTime = DateTimeExtensions.CurrentTimeMillis();
        long LastSendLiveTime = 0;

        static Random Ran = new Random();

        public int ConnectId;

        private BlockingCollection<DatagramPacket> DpBuffer = new BlockingCollection<DatagramPacket>();

        public ClientControl MyClientControl;

        public bool LocalClosed = false;
        public bool RemoteClosed = false;
        public bool Destoryed = false;
        public bool StopNow = false;

        public ConnectionUDP(Route route, string dstHost, int dstPort,
                             int mode, int connectId, ClientControl clientControl)
        {
            MyClientControl = clientControl;
            MyRoute = route;
            DstHost = dstHost;
            DstPort = dstPort;
            Mode = mode;
            ConnectId = connectId;

            try
            {
                MySender = new Sender(this);
                MyRecevier = new Receiver(this);
                Uos = new UDPOutputStream(this);
                Uis = new UDPInputStream(this);
                /*
                if (mode == 2)
                {
                    route.CreateTunnelProcessor().Process(this);
                }
                */
            }
            catch (Exception e)
            {
                _connected = false;
                route.ConnTable.Remove(connectId);

                lock (this)
                {
                    Monitor.PulseAll(this);
                }
                throw (e);
            }
        }

        public DatagramPacket GetPacket(int connectId)
        {
            DatagramPacket datagramPacket = DpBuffer.Take();
            return datagramPacket;
        }

        public override string ToString() => DstHost + ":" + DstPort;

        public void CloseLocal()
        {
            if (!LocalClosed)
            {
                LocalClosed = true;
                if (!StopNow)
                {
                    MySender.SendCloseConnPacket();
                    
                }
                Destory(false);
            }
        }

        public void CloseRemote()
        {
            if (!RemoteClosed)
            {
                RemoteClosed = true;
                Destory(false);
            }
        }

        // Completely Close
        public void Destory(bool force)
        {
            if (!Destoryed)
            {
                if ((LocalClosed && RemoteClosed) || force)
                {
                    Destoryed = true;
                    _connected = false;
                    Uis.CloseLocalStream();
                    Uos.CloseLocalStream();
                    MySender.Destroy();
                    MyRecevier.Destroy();
                    MyRoute.RemoveConnection(this);
                    MyClientControl.RemoveConnection(this);
                }
            }
        }

        public void Live() => LastLiveTime = DateTimeExtensions.CurrentTimeMillis();
    }
}
