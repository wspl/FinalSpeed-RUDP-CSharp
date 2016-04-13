using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RUDP_AirLink.Utils;
using System.Collections.Concurrent;

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
        Route MyRoute;

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

        static Random ran = new Random();

        int ConnectId;

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

                if (mode == 2)
                {
                    route.CreateTunnelProcessor().Process(ref this);
                }
            }
            catch (Exception e)
            {
                _connected = false;
                route.ConnTable.Remove(connectId);

                lock (this)
                {
                    NotifyAll();
                }
                throw (e);
            }
        }

        public DatagramPacket GetPacket(int connectId)
        {
            DatagramPacket dp = DpBuffer.Take();
            return dp;
        }

        public override string ToString() => DstHost + ":" + DstPort;

        public void CloseLocal()
        {
            if (!LocalClosed)
            {
                LocalClosed = true;
                if (!StopNow)
                {
                    MySender.SendClosePacketConn();
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
                    MySender.Destory();
                    MyRecevier.Destory();
                    MyRoute.RemoveConnection(this);
                    MyClientControl.RemoveConnection(this);
                }
            }
        }

        public void Live() => LastLiveTime = DateTimeExtensions.CurrentTimeMillis();
    }
}
