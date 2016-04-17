using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FinalSpeed_RUDP_CSharp.Utils;
using FinalSpeed_RUDP_CSharp.RUDP.Packets;

namespace FinalSpeed_RUDP_CSharp.RUDP
{
    public class Sender
    {
        ConnectionUDP Conn;

        public int Sum = 0;
        Receiver MyReceiver;

        public Dictionary<int, DataPacket> SendTable = new Dictionary<int, DataPacket>();

        object WinOb = new object();

        public string DstHost;
        public int DstPort;

        public int Sequence = 0;
        public int SendOffset = -1;

        int UnAckMax = -1;
        int SendSum = 0;

        UDPOutputStream Uos;

        int Sw = 0;

        static Random Ran = new Random();

        long LastSendTime = -1;
        bool Closed = false;
        bool StreamClosed = false;

        static int S = 0;

        object SynSendTable = new object();

        Dictionary<int, DataPacket> UnAckTable = new Dictionary<int, DataPacket>();

        public Sender(ConnectionUDP conn)
        {
            Conn = conn;
            Uos = new UDPOutputStream(conn);
            MyReceiver = conn.MyRecevier;
            DstHost = conn.DstHost;
            DstPort = conn.DstPort;
        }

        public void SendData(byte[] data, int offset, int length)
        {
            int packetLength = RUDPConfig.PackageSize;
            int sum = (length / packetLength);
            if (length % packetLength != 0)
            {
                sum += 1;
            }
            if (sum == 0)
            {
                sum = 1;
            }
            if (length < packetLength)
            {
                Sw += 1;
                SendNata(data, 0, length);
                Sw -= 1;
            }
            else
            {
                int len = packetLength;
                for (int i = 0; i < sum; i += 1)
                {
                    byte[] b = new byte[len];
                    Array.Copy(data, offset, b, 0, len);
                    SendNata(b, 0, b.Length);
                    offset += packetLength;
                    if (offset + len > length)
                    {
                        len = length - (sum - 1) * packetLength;
                    }
                }
            }
        }

        void SendNata(byte[] data, int offset, int length)
        {
            if (!Closed && !StreamClosed)
            {
                DataPacket dataPacket = new DataPacket(Sequence, data, 0, (short)length, Conn.ConnectId, Conn.MyRoute.LocalClientId);
                dataPacket.DstHost = DstHost;
                dataPacket.DstPort = DstPort;

                lock (SynSendTable)
                {
                    SendTable.Add(dataPacket.Sequence, dataPacket);
                }

                lock (WinOb)
                {
                    if (!Conn.MyRecevier.CheckWin())
                    {
                        //try
                        Monitor.Wait(WinOb);
                    }
                }

                bool twice = false;
                if (RUDPConfig.TwiceTcp ||
                    (RUDPConfig.DoubleSendStart && dataPacket.Sequence <= 5))
                {
                    twice = true;
                }

                SendDataPacket(dataPacket, false, twice, true);

                LastSendTime = DateTimeExtensions.CurrentTimeMillis();
                SendOffset += 1;
                S += dataPacket.Data.Length;
                Conn.MyClientControl.MyResendManage.AddTask(Conn, Sequence);
                Sequence += 1;
            }
            else
            {
                throw new Exception("Disconnected");
            }
        }

        public void CloseLocalStream()
        {
            if (!StreamClosed)
            {
                StreamClosed = true;
                Conn.MyRecevier.CloseLocalStream();
                if (!Conn.StopNow)
                {
                    SendCloseStreamPacket();
                }
            }
        }

        public void CloseRemoteStream()
        {
            if (!StreamClosed)
            {
                StreamClosed = true;
            }
        }

        void SendDataPacket(DataPacket dataPacket, bool resend, bool twice, bool block)
        {
            lock (Conn.MyClientControl.SynLock)
            {
                long startTime = DateTimeExtensions.NanoTime();
                long t1 = DateTimeExtensions.CurrentTimeMillis();

                Conn.MyClientControl.OnSendDataPacket(Conn);

                int timeId = Conn.MyClientControl.GetCurrentTimeId();
                dataPacket.Create(timeId);

                SendRecord currentRecord = Conn.MyClientControl.GetSendRecord(timeId);
                if (!resend)
                {
                    dataPacket.FirstSendTimeId = timeId;
                    dataPacket.FirstSendTime = DateTimeExtensions.CurrentTimeMillis();
                    currentRecord.AddSentFirst(dataPacket.Data.Length);
                    currentRecord.AddSent(dataPacket.Data.Length);
                }
                else
                {
                    SendRecord record = Conn.MyClientControl.GetSendRecord(dataPacket.FirstSendTimeId);
                    record.AddResent(dataPacket.Data.Length);
                    currentRecord.AddSent(dataPacket.Data.Length);
                }

                //try
                SendSum += 1;
                Sum += 1;
                UnAckMax += 1;

                Send(dataPacket.MyDatagramPacket);

                if (twice)
                {
                    Send(dataPacket.MyDatagramPacket);
                }
                if (block)
                {
                    Conn.MyClientControl.SendSleep(startTime, dataPacket.Data.Length);
                }
                TrafficEvent e = new TrafficEvent("", (long)Ran.Next(), dataPacket.Data.Length, TrafficEvent.Upload);
                Route.FireEvent(e);
            }
        }

        public void SendAckDelay(int ackSequence) => Conn.MyRoute.DelayAckManage.AddAck(Conn, ackSequence);

        public void SendLastReadDelay() => Conn.MyRoute.DelayAckManage.AddLastRead(Conn);

        public DataPacket GetDataPacket(int sequence) => SendTable[sequence];

        public void Resend(int sequence, int count)
        {
            if (SendTable.ContainsKey(sequence))
            {
                DataPacket dataPacket = SendTable[sequence];
                if (dataPacket != null)
                {
                    SendDataPacket(dataPacket, true, false, true);
                }
            }
        }

        public void Destroy()
        {
            lock (SynSendTable)
            {
                SendTable.Clear();
            }
        }

        public void RemoveSentAck(int sequence)
        {
            lock (SynSendTable)
            {
                //? DataPacket dataPacket = SendTable[sequence];
                SendTable.Remove(sequence);
            }
        }

        public void Play()
        {
            lock (WinOb)
            {
                Monitor.PulseAll(WinOb);
            }
        }

        public void Close()
        {
            lock (WinOb)
            {
                Closed = true;
                Monitor.PulseAll(WinOb);
            }
        }

        void SendCloseStreamPacket()
        {
            CloseStreamPacket closeStreamPacket = new CloseStreamPacket(Conn.ConnectId, Conn.MyRoute.LocalClientId, Sequence);
            closeStreamPacket.DstHost = DstHost;
            closeStreamPacket.DstPort = DstPort;

            //try
            Send(closeStreamPacket.MyDatagramPacket);
            Send(closeStreamPacket.MyDatagramPacket);
        }

        public void SendCloseConnPacket()
        {
            CloseConnPacket closeConnPacket = new CloseConnPacket(Conn.ConnectId, Conn.MyRoute.LocalClientId);
            closeConnPacket.DstHost = DstHost;
            closeConnPacket.DstPort = DstPort;

            //try
            Send(closeConnPacket.MyDatagramPacket);
            Send(closeConnPacket.MyDatagramPacket);
        }

        public void SendAckListPacket(List<int> ackList)
        {
            int currentTimeId = Conn.MyRecevier.CurrentRemoteTimeId;
            AckListPacket ackListPakcet = new AckListPacket(ackList, Conn.MyRecevier.LastRead,
                                                            Conn.MyClientControl.SendRecordTableRemote, currentTimeId,
                                                            Conn.ConnectId, Conn.MyRoute.LocalClientId);
            ackListPakcet.DstHost = DstHost;
            ackListPakcet.DstPort = DstPort;

            Send(ackListPakcet.MyDatagramPacket);
        }

        void Send(DatagramPacket dp)
        {
            SendPacket(dp);
        }

        void SendPacket(DatagramPacket dp)
        {
            Conn.MyClientControl.SendPacket(dp);
        }
    }
}
