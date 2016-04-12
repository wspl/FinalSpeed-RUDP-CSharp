using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP.Packets
{
    class PingPacket2 : Packet
    {
        byte[] DpData = new byte[16];

        public int PingId { get; set; }

        public PingPacket2(int connectId, int clientId, int pingId)
        {
            SType = PacketType.PingPacket2;

            BitConverter.GetBytes(Ver).CopyTo(DpData, 0);
            BitConverter.GetBytes(SType).CopyTo(DpData, 2);
            BitConverter.GetBytes(connectId).CopyTo(DpData, 4);
            BitConverter.GetBytes(clientId).CopyTo(DpData, 8);
            BitConverter.GetBytes(pingId).CopyTo(DpData, 12);

            Dp = new DatagramPacket(DpData, DpData.Length);
        }

        public PingPacket2(DatagramPacket dp)
        {
            Dp = dp;
            DpData = dp.Dgram;

            Ver = BitConverter.ToInt16(DpData, 0);
            SType = BitConverter.ToInt16(DpData, 2);
            ConnectId = BitConverter.ToInt32(DpData, 4);
            ClientId = BitConverter.ToInt32(DpData, 8);
            PingId = BitConverter.ToInt32(DpData, 12);
        }
    }
}
