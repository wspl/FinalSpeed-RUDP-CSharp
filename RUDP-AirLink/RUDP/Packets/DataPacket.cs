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
        byte[] DatagramData;

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

        public DataPacket(DatagramPacket datagramPacket)
        {
            MyDatagramPacket = datagramPacket;
            DatagramData = datagramPacket.Data;

            Ver = BitConverter.ToInt16(DatagramData, 0);
            SType = BitConverter.ToInt16(DatagramData, 2);

            ConnectId = BitConverter.ToInt32(DatagramData, 4);
            ClientId = BitConverter.ToInt32(DatagramData, 8);

            Sequence = BitConverter.ToInt32(DatagramData, 12);
            Length = BitConverter.ToInt16(DatagramData, 16);
            TimeId = BitConverter.ToInt32(DatagramData, 18);

            Data = new byte[Length];

            Array.Copy(DatagramData, 22, Data, 0, Length);
        }

        public void Create(int timeId)
        {
            TimeId = timeId;
            DatagramData = new byte[Length + 16 + 8];

            BitConverter.GetBytes(Ver).CopyTo(DatagramData, 0);
            BitConverter.GetBytes(SType).CopyTo(DatagramData, 2);

            BitConverter.GetBytes(ConnectId).CopyTo(DatagramData, 4);
            BitConverter.GetBytes(ClientId).CopyTo(DatagramData, 8);

            BitConverter.GetBytes(Sequence).CopyTo(DatagramData, 12);
            BitConverter.GetBytes(Length).CopyTo(DatagramData, 16);
            BitConverter.GetBytes(TimeId).CopyTo(DatagramData, 18);

            Array.Copy(Data, 0, DatagramData, 22, Length);

            MyDatagramPacket = new DatagramPacket(DatagramData, DatagramData.Length);
            MyDatagramPacket.Host = DstHost;
            MyDatagramPacket.Port = DstPort;
        }
    }
}
