using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RUDP_AirLink.RUDP.Packets;
using RUDP_AirLink.Utils;

namespace RUDP_AirLink.RUDP
{
    class Receiver
    {
        ConnectionUDP Conn;
        Sender MySender;

        public string DstHost;
        public int DstPort;

        Dictionary<int, DataPacket> ReceiveTable = new Dictionary<int, DataPacket>();

        int LastRead = -1;
        int LastRead2 = -1;

        object AvailOb = new object();

        UDPInputStream Uis;

        float AvailWin = RUDPConfig.MaxWin;

        public int CurrentRemoteTimeId { get; set; }
        public int CloseOffset { get; set; }

        bool StreamClose = false;
        bool ReceviedClose = false;

        static int C;

        public int Nw;

        long Received;

        public Receiver(ConnectionUDP conn)
        {
            Conn = conn;
            Uis = new UDPInputStream(conn);
            MySender = conn.MySender;
            DstHost = conn.DstHost;
            DstPort = conn.DstPort;
        }

        public byte[] Receive()
        {
            DataPacket dataPacket;
            if (Conn.IsConnected)
            {
                dataPacket = ReceiveTable[LastRead + 1];
                lock (AvailOb)
                {
                    if (dataPacket == null)
                    {
                        //try
                        Monitor.Wait(AvailOb);

                        dataPacket = ReceiveTable[LastRead + 1];
                    }
                }
            }
            else
            {
                throw new Exception("No connection");
            }

            if (!StreamClose)
            {
                CheckRemoteCloseOffset();

                if (dataPacket == null)
                {
                    throw new Exception("Disconnected");
                }

                Conn.MySender.SendLastReadDelay();

                LastRead += 1;

                lock (AvailOb)
                {
                    ReceiveTable.Remove(dataPacket.Sequence);
                }

                Received += dataPacket.Data.Length;

                return dataPacket.Data;
            }
            else
            {
                throw new Exception("Disconnected");
            }
        }

        public OnReceivePacket(DatagramPacket dp)
        {
            DataPacket dataPacket;
            if (dp != null)
            {
                if (Conn.IsConnected)
                {
                    int ver = PacketCheck.CheckVer(dp);
                    int sType = PacketCheck.CheckSType(dp);

                    if (ver == RUDPConfig.ProtocalVer)
                    {
                        Conn.Live();
                        if (sType == PacketType.DataPacket)
                        {
                            dataPacket = new DataPacket(dp);
                            int timeId = dataPacket.TimeId;

                            SendRecord record = Conn.MyClientControl.SendRecordTableRemote[timeId];
                            if (record == null)
                            {
                                record = new SendRecord();
                                record.TimeId = timeId;
                                Conn.MyClientControl.SendRecordTableRemote.Add(timeId, record);
                            }
                            record.AddSent(dataPacket.Data.Length);

                            if (timeId > CurrentRemoteTimeId)
                            {
                                CurrentRemoteTimeId = timeId;
                            }

                            int sequence = dataPacket.Sequence;

                            Conn.MySender.SendAckDelay(dataPacket.Sequence);
                            if (sequence > LastRead)
                            {
                                lock (AvailOb)
                                {
                                    ReceiveTable.Add(sequence, dataPacket);
                                    if (ReceiveTable.ContainsKey(LastRead + 1))
                                    {
                                        Monitor.Pulse(AvailOb);
                                    }
                                }
                            }
                        }
                        else if (sType == PacketType.AckListPacket)
                        {
                            AckListPacket ackListPacket = new AckListPacket(dp);

                            int lastRead3 = ackListPacket.LastRead;
                            if (lastRead3 > LastRead2)
                            {
                                LastRead2 = lastRead3;
                            }

                            List<int> ackList = ackListPacket.AckList;
                            foreach (int sequence in ackList)
                            {
                                Conn.MySender.RemoveSentAck(sequence);
                            }

                            SendRecord rc1 = Conn.MyClientControl.GetSendRecord(ackListPacket.R1);
                            if (rc1 != null && ackListPacket.S1 > rc1.AckedSize)
                            {
                                rc1.AckedSize = ackListPacket.S1;
                            }

                            SendRecord rc2 = Conn.MyClientControl.GetSendRecord(ackListPacket.R2);
                            if (rc2 != null && ackListPacket.S2 > rc2.AckedSize)
                            {
                                rc2.AckedSize = ackListPacket.S2;
                            }

                            SendRecord rc3 = Conn.MyClientControl.GetSendRecord(ackListPacket.R3);
                            if (rc3 != null && ackListPacket.S3 > rc3.AckedSize)
                            {
                                rc3.AckedSize = ackListPacket.S3;
                            }

                            if (CheckWin())
                            {
                                Conn.MySender.Play();
                            }
                        }
                        else if (sType == PacketType.CloseStreamPacket)
                        {
                            CloseStreamPacket closeStreamPacket = new CloseStreamPacket(dp);
                            ReceviedClose = true;
                            int n = closeStreamPacket.CloseOffset;
                            CloseRemoteStream(n);
                        }
                        else if (sType == PacketType.CloseConnPacket)
                        {
                            CloseConnPacket closeConnPacket = new CloseConnPacket(dp);
                            Conn.CloseRemote();
                        }
                        else
                        {
                            throw Exception("Unknown packet type");
                        }
                    }
                }
            }
        }

        public void Destory()
        {
            lock (AvailOb)
            {
                ReceiveTable.Clear();
            }
        }

        bool CheckWin()
        {
            Nw = Conn.MySender.SendOffset - LastRead2;
            bool b = false;

            if (Nw < AvailWin)
            {
                b = true;
            }

            return b;
        }

        void CloseRemoteSteram(int closeOffset)
        {
            CloseOffset = closeOffset;
            if (!StreamClose)
            {
                CheckRemoteCloseOffset();
            }
        }

        void CheckRemoteCloseOffset()
        {
            if (!StreamClose && 
                ReceviedClose && 
                LastRead >= CloseOffset - 1)
            {
                StreamClose = true;

                lock (AvailOb)
                {
                    Monitor.PulseAll(AvailOb);
                }

                Conn.MySender.CloseRemoteStream();
            }
        }

        void CloseLocalStream()
        {
            if (!StreamClose)
            {
                C += 1;
                StreamClose = true;

                lock (AvailOb)
                {
                    Monitor.PulseAll(AvailOb);
                }

                Conn.MySender.CloseLocalStream();
            }
        }
    }
}
