using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using RUDP_AirLink.Utils;
using RUDP_AirLink.RUDP.Packets;

namespace RUDP_AirLink.RUDP
{
    class ClientControl
    {
        int ClientId;

        Thread SendThread;

        object SynLock = new object();

        private Dictionary<int, SendRecord> SendRecordTable = new Dictionary<int, SendRecord>();

        Dictionary<int, SendRecord> SendRecordTableRemote = new Dictionary<int, SendRecord>();

        long StartSendTime = 0;
        int CurrentSpeed, InitSpeed, MaxSpeed = 1024 * 1024;
        int LastTime = -1;

        object SynTimeId = new object();

        long Sended = 0;
        long MarkTime = 0;
        long LastSendPingTime, LastReceivePingTime = DateTimeExtensions.CurrentTimeMillis();

        Random Ran = new Random();

        Dictionary<int, long> PingTable = new Dictionary<int, long>();

        public int PingDelay = 250;

        int ClientIdReal = -1;

        long NeedSleepAll, TrueSleepAll;

        int MaxAcked = 0;
        long LastLockTime;

        Route MyRoute;
        string DstHost;
        int DstPort;

        public Dictionary<int, ConnectionUDP> ConnTable = new Dictionary<int, ConnectionUDP>();

        object SynConnTable, SynTunTable = new object();

        string Password;

        public ResendManage em = new ResendManage();

        bool Closed = false;

        public ClientControl(Route route, int clientId, string dstHost, int dstPort)
        {
            ClientId = clientId;
            MyRoute = route;
            DstHost = dstHost;
            DstPort = dstPort;
        }

        public void OnReceivePacket(DatagramPacket dp)
        {
            byte[] dpData = dp.Dgram;

            int sType = PacketCheck.CheckSType(dp);
            int remoteClientId = BitConverter.ToInt32(dpData, 9);

            if (sType == PacketType.PingPacket)
            {
                PingPacket pp = new PingPacket(dp);
                //TODO
            }
        }

        public void SendPacket(DatagramPacket dp)
        {
            //TODO
        }

        void AddConnection(ConnctionUDP conn)
        {
            //TODO
        }

        void RemoveConnection(ConnctionUDP conn)
        {
            //TODO
        }

        public void Close()
        {
            Closed = true;
            MyRoute.ClientManager.RemoveClient(ClientId);
            lock (SynConnTable)
            {
                //TODO
            }
        }

        //TODO: Iterator<Integer> getConnTableIterator()

        public void UpdateClientId(int newClientId)
        {
            ClientIdReal = newClientId;
            SendRecordTable.Clear();
            SendRecordTableRemote.Clear();
        }

        //TODO?: public void OnSendDataPacket()

        public void SendPingPacket()
        {
            int pingId = Math.Abs(Ran.Next());
            long pingTime = DateTimeExtensions.CurrentTimeMillis();

            PingTable.Add(pingId, pingTime);
            LastSendPingTime = DateTimeExtensions.CurrentTimeMillis();

            PingPacket pp = new PingPacket(0, MyRoute.LocalClientId, pingId, MyRoute.LocalDownloadSpeed, MyRoute.LocalUploadSoeed);
            pp.DstHost = DstHost;
            pp.DstPort = DstPort;

            //try
            SendPacket(pp.Dp);
        }

        public void SendPingPacket2(int pingId, string dstHost, int dstPort)
        {
            PingPacket2 ppb = new PingPacket2(0, MyRoute.LocalClientId, pingId);
            ppb.DstHost = dstHost;
            ppb.DstPort = dstPort;

            //try
            SendPacket(ppb.Dp);
        }

        //TODO?: public void onReceivePing(PingMessage pm)

        SendRecord GetSendRecord(int timeId)
        {
            SendRecord record;
            lock (SynTimeId)
            {
                record = SendRecordTable[timeId];
                if (record == null)
                {
                    record = new SendRecord();
                    record.TimeId = timeId;
                    SendRecordTable.Add(timeId, record);
                }
            }
            return record;
        }

        public int GetCurrentTimeId()
        {
            long current = DateTimeExtensions.CurrentTimeMillis();
            if (StartSendTime == 0)
            {
                StartSendTime = current;
            }
            int timeId = (int)(current - StartSendTime) / 1000;
            return timeId;
        }

        public int GetTimeId(long time) => (int)(time - StartSendTime) / 1000;

        public void SendSleep(long startTime, int length)
        {
            if (MyRoute.Mode == 1)
            {
                CurrentSpeed = MyRoute.LocalUploadSpeed;
            }
            if (Sended == 0)
            {
                MarkTime = startTime;
            }
            Sended += length;
            // 10K Sleep
            if (Sended > 10 * 1024)
            {
                long needTime = (long)(1000 * 1000 * 1000f * Sended / CurrentSpeed);
                long usedTime = DateTimeExtensions.NanoTime() - MarkTime;
                
                if(usedTime < needTime)
                {
                    long sleepTime = needTime - usedTime;
                    NeedSleepAll += sleepTime;

                    long moreTime = TrueSleepAll - NeedSleepAll;
                    if (moreTime > 0)
                    {
                        if (sleepTime <= moreTime)
                        {
                            sleepTime = 0;
                            TrueSleepAll -= sleepTime;
                        }
                    }

                    int s = (int)(needTime / (1000 * 1000));
                    int n = (int)(needTime % (1000 * 1000));
                    long t1 = DateTimeExtensions.NanoTime();
                    if (sleepTime > 0)
                    {
                        //try
                        Thread.Sleep(s); // no "n", waiting for repairing.
                        TrueSleepAll += DateTimeExtensions.NanoTime() - t1;
                    }
                }
                Sended = 0;
            }
        }
        // TODO: Getter Setter
    }
}