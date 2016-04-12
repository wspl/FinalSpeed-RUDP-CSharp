using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP.Packets
{
    class AckListPacket : Packet
    {
        List<int> AckList { get; set; }

        byte[] DpData;

        int LastRead { get; set; }

        int R1 { get; set; }
        int R2 { get; set; }
        int R3 { get; set; }
        int S1 { get; set; }
        int S2 { get; set; }
        int S3 { get; set; }

        public AckListPacket(List<int> ackList, int lastRead,
                             Dictionary<int, SendRecord> sendRecordTable,
                             int timeId, int connectId, int clientId)
        {
            ClientId = clientId;
            ConnectId = connectId;
            AckList = ackList;
            LastRead = lastRead;

            int len1 = 4 + 4 + 10 + 4 * AckList.Count;
            DpData = new byte[len1 + 24 + 9];

            SType = PacketType.AckListPacket;

            BitConverter.GetBytes(Ver).CopyTo(DpData, 0);
            BitConverter.GetBytes(SType).CopyTo(DpData, 2);
            BitConverter.GetBytes(ConnectId).CopyTo(DpData, 4);
            BitConverter.GetBytes(ClientId).CopyTo(DpData, 8);

            BitConverter.GetBytes(LastRead).CopyTo(DpData, 12);
            BitConverter.GetBytes((short)AckList.Count).CopyTo(DpData, 16);

            for (int i = 0; i < AckList.Count; i += 1)
            {
                int sequence = AckList[i];
                BitConverter.GetBytes(sequence).CopyTo(DpData, 18 + 4 * i);
            }

            int u1 = timeId - 2;
            BitConverter.GetBytes(u1).CopyTo(DpData, len1 + 8);

            SendRecord r1 = sendRecordTable[u1];
            if (r1 != null)
            {
                S1 = r1.SendSize;
            }
            BitConverter.GetBytes(S1).CopyTo(DpData, len1 + 4 + 8);

            int u2 = timeId - 1;
            BitConverter.GetBytes(u2).CopyTo(DpData, len1 + 8 + 8);

            SendRecord r2 = sendRecordTable[u2];
            if (r2 != null)
            {
                S2 = r2.SendSize;
            }
            BitConverter.GetBytes(S2).CopyTo(DpData, len1 + 12 + 8);

            int u3 = timeId;
            BitConverter.GetBytes(u3).CopyTo(DpData, len1 + 16 + 8);

            SendRecord r3 = sendRecordTable[u3];
            if (r3 != null)
            {
                S3 = r3.SendSize;
            }
            BitConverter.GetBytes(S3).CopyTo(DpData, len1 + 20 + 8);

            Dp = new DatagramPacket(DpData, DpData.Length);
        }

        public AckListPacket(DatagramPacket dp)
        {
            Dp = dp;
            DpData = dp.Dgram;

            Ver = BitConverter.ToInt16(DpData, 0);
            SType = BitConverter.ToInt16(DpData, 2);
            ConnectId = BitConverter.ToInt32(DpData, 4);
            ClientId = BitConverter.ToInt32(DpData, 8);

            LastRead = BitConverter.ToInt32(DpData, 12);

            int sum = BitConverter.ToInt16(DpData, 16);

            AckList = new List<int>();
            int t = 0;
            for (int i = 0; i < sum; i += 1)
            {
                t = 10 + 4 + i;
                int sequence = BitConverter.ToInt32(DpData, t + 8);
                AckList.Add(sequence);
            }

            t = 10 + 4 * sum - 4;

            R1 = BitConverter.ToInt32(DpData, t + 4 + 8);
            S1 = BitConverter.ToInt32(DpData, t + 8 + 8);

            R2 = BitConverter.ToInt32(DpData, t + 12 + 8);
            S2 = BitConverter.ToInt32(DpData, t + 16 + 8);

            R3 = BitConverter.ToInt32(DpData, t + 20 + 8);
            S3 = BitConverter.ToInt32(DpData, t + 24 + 8);
        }
    }
}
