using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP.Packets
{
    class DataPacket : Packet
    {
        public int Sequence { get; set; } = 0;
        short Length { get; set; } = 0;

        public byte[] Data { get; set; }
        byte[] DpData;

        public int TimeId { get; set; }

        public string DstHost { get; set; }
        public int DstPort { get; set; }

        int Offset;

        public int FirstSendTimeId { get; set; }
        public long FirstSendTime { get; set; }

        public DataPacket(int sequence, byte[] dataq, int offset, short length,
                          int connectId, int clientId)
        {
            SType = PacketType.DataPacket;

            Sequence = sequence;
            Offset = offset;
            Length = length;
            Data = new byte[length];
            ClientId = clientId;
            ConnectId = connectId;

            Array.Copy(dataq, offset, Data, 0, length);
            Length = (short)Data.Length;
        }

        public DataPacket(DatagramPacket dp)
        {
            Dp = dp;
            DpData = dp.Dgram;

            Ver = BitConverter.ToInt16(DpData, 0);
            SType = BitConverter.ToInt16(DpData, 2);

            ConnectId = BitConverter.ToInt32(DpData, 4);
            ClientId = BitConverter.ToInt32(DpData, 8);

            Sequence = BitConverter.ToInt32(DpData, 12);
            Length = BitConverter.ToInt16(DpData, 16);
            TimeId = BitConverter.ToInt32(DpData, 18);

            Data = new byte[Length];

            Array.Copy(DpData, 22, Data, 0, Length);
        }

        public void Create(int timeId)
        {
            TimeId = timeId;
            DpData = new byte[Length + 16 + 8];

            BitConverter.GetBytes(Ver).CopyTo(DpData, 0);
            BitConverter.GetBytes(SType).CopyTo(DpData, 2);

            BitConverter.GetBytes(ConnectId).CopyTo(DpData, 4);
            BitConverter.GetBytes(ClientId).CopyTo(DpData, 8);

            BitConverter.GetBytes(Sequence).CopyTo(DpData, 12);
            BitConverter.GetBytes(Length).CopyTo(DpData, 16);
            BitConverter.GetBytes(TimeId).CopyTo(DpData, 18);

            Array.Copy(Data, 0, DpData, 22, Length);

            Dp = new DatagramPacket(DpData, DpData.Length);
            Dp.Host = DstHost;
            Dp.Port = DstPort;
        }
    }
}
