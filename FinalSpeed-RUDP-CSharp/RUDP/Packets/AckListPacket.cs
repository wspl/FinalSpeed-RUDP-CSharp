using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSpeed_RUDP_CSharp.RUDP.Packets
{
    public class AckListPacket : Packet
    {
        public List<int> AckList { get; set; }

        byte[] DatagramData;

        public int LastRead { get; set; }

        public int R1 { get; set; }
        public int R2 { get; set; }
        public int R3 { get; set; }
        public int S1 { get; set; }
        public int S2 { get; set; }
        public int S3 { get; set; }

        public AckListPacket(List<int> ackList, int lastRead,
                             Dictionary<int, SendRecord> sendRecordTable,
                             int timeId, int connectId, int clientId)
        {
            ClientId = clientId;
            ConnectId = connectId;
            AckList = ackList;
            LastRead = lastRead;

            int len1 = 4 + 4 + 10 + 4 * AckList.Count;
            DatagramData = new byte[len1 + 24 + 9];

            SType = PacketType.AckListPacket;

            BitConverter.GetBytes(Ver).CopyTo(DatagramData, 0);
            BitConverter.GetBytes(SType).CopyTo(DatagramData, 2);
            BitConverter.GetBytes(ConnectId).CopyTo(DatagramData, 4);
            BitConverter.GetBytes(ClientId).CopyTo(DatagramData, 8);

            BitConverter.GetBytes(LastRead).CopyTo(DatagramData, 12);
            BitConverter.GetBytes((short)AckList.Count).CopyTo(DatagramData, 16);

            for (int i = 0; i < AckList.Count; i += 1)
            {
                int sequence = AckList[i];
                BitConverter.GetBytes(sequence).CopyTo(DatagramData, 18 + 4 * i);
            }

            int u1 = timeId - 2;
            BitConverter.GetBytes(u1).CopyTo(DatagramData, len1 + 8);

            SendRecord r1 = sendRecordTable[u1];
            if (r1 != null)
            {
                S1 = r1.SendSize;
            }
            BitConverter.GetBytes(S1).CopyTo(DatagramData, len1 + 4 + 8);

            int u2 = timeId - 1;
            BitConverter.GetBytes(u2).CopyTo(DatagramData, len1 + 8 + 8);

            SendRecord r2 = sendRecordTable[u2];
            if (r2 != null)
            {
                S2 = r2.SendSize;
            }
            BitConverter.GetBytes(S2).CopyTo(DatagramData, len1 + 12 + 8);

            int u3 = timeId;
            BitConverter.GetBytes(u3).CopyTo(DatagramData, len1 + 16 + 8);

            SendRecord r3 = sendRecordTable[u3];
            if (r3 != null)
            {
                S3 = r3.SendSize;
            }
            BitConverter.GetBytes(S3).CopyTo(DatagramData, len1 + 20 + 8);

            MyDatagramPacket = new DatagramPacket(DatagramData, DatagramData.Length);
        }

        public AckListPacket(DatagramPacket datagramPacket)
        {
            MyDatagramPacket = datagramPacket;
            DatagramData = datagramPacket.Data;

            Ver = BitConverter.ToInt16(DatagramData, 0);
            SType = BitConverter.ToInt16(DatagramData, 2);
            ConnectId = BitConverter.ToInt32(DatagramData, 4);
            ClientId = BitConverter.ToInt32(DatagramData, 8);

            LastRead = BitConverter.ToInt32(DatagramData, 12);

            int sum = BitConverter.ToInt16(DatagramData, 16);

            AckList = new List<int>();
            int t = 0;
            for (int i = 0; i < sum; i += 1)
            {
                t = 10 + 4 + i;
                int sequence = BitConverter.ToInt32(DatagramData, t + 8);
                AckList.Add(sequence);
            }

            t = 10 + 4 * sum - 4;

            R1 = BitConverter.ToInt32(DatagramData, t + 4 + 8);
            S1 = BitConverter.ToInt32(DatagramData, t + 8 + 8);

            R2 = BitConverter.ToInt32(DatagramData, t + 12 + 8);
            S2 = BitConverter.ToInt32(DatagramData, t + 16 + 8);

            R3 = BitConverter.ToInt32(DatagramData, t + 20 + 8);
            S3 = BitConverter.ToInt32(DatagramData, t + 24 + 8);
        }
    }
}
