using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP.Packets
{
    class CloseStreamPacket : Packet
    {
        byte[] Data;
        byte[] DpData;

        public int CloseOffset { get; set; }

        public CloseStreamPacket(int connectId, int clientId, int closeOffset)
        {
            SType = PacketType.CloseStreamPacket;

            DpData = new byte[16];
            ClientId = clientId;
            ConnectId = connectId;
            CloseOffset = closeOffset;

            BitConverter.GetBytes(Ver).CopyTo(DpData, 0);
            BitConverter.GetBytes(SType).CopyTo(DpData, 2);
            BitConverter.GetBytes(ConnectId).CopyTo(DpData, 4);
            BitConverter.GetBytes(ClientId).CopyTo(DpData, 8);
            BitConverter.GetBytes(CloseOffset).CopyTo(DpData, 12);

            Dp = new DatagramPacket(DpData, DpData.Length);
        }

        public CloseStreamPacket(DatagramPacket dp)
        {
            Dp = dp;
            DpData = dp.Dgram;

            Ver = BitConverter.ToInt16(DpData, 0);
            SType = BitConverter.ToInt16(DpData, 2);

            ConnectId = BitConverter.ToInt32(DpData, 4);
            ClientId = BitConverter.ToInt32(DpData, 8);
            CloseOffset = BitConverter.ToInt32(DpData, 12);
        }
    }
}
