using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP.Packets
{
    class CloseConnPacket : Packet
    {
        byte[] Data;
        byte[] DpData;

        public CloseConnPacket(int connectId, int clientId)
        {
            SType = PacketType.CloseConnPacket;

            DpData = new byte[12];
            ClientId = clientId;
            ConnectId = connectId;

            BitConverter.GetBytes(Ver).CopyTo(DpData, 0);
            BitConverter.GetBytes(SType).CopyTo(DpData, 2);
            BitConverter.GetBytes(ConnectId).CopyTo(DpData, 4);
            BitConverter.GetBytes(ClientId).CopyTo(DpData, 8);

            Dp = new DatagramPacket(DpData, DpData.Length);
        }

        public CloseConnPacket(DatagramPacket dp)
        {
            Dp = dp;
            DpData = dp.Dgram;

            Ver = BitConverter.ToInt16(DpData, 0);
            SType = BitConverter.ToInt16(DpData, 2);
            ConnectId = BitConverter.ToInt32(DpData, 4);
            ClientId = BitConverter.ToInt32(DpData, 8);
        }
    }
}
