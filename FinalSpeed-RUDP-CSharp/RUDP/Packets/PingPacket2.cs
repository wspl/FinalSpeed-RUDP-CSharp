using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSpeed_RUDP_CSharp.RUDP.Packets
{
    public class PingPacket2 : Packet
    {
        byte[] DatagramData = new byte[16];

        public int PingId { get; set; }

        public PingPacket2(int connectId, int clientId, int pingId)
        {
            SType = PacketType.PingPacket2;

            BitConverter.GetBytes(Ver).CopyTo(DatagramData, 0);
            BitConverter.GetBytes(SType).CopyTo(DatagramData, 2);
            BitConverter.GetBytes(connectId).CopyTo(DatagramData, 4);
            BitConverter.GetBytes(clientId).CopyTo(DatagramData, 8);
            BitConverter.GetBytes(pingId).CopyTo(DatagramData, 12);

            MyDatagramPacket = new DatagramPacket(DatagramData, DatagramData.Length);
        }

        public PingPacket2(DatagramPacket datagramPacket)
        {
            MyDatagramPacket = datagramPacket;
            DatagramData = datagramPacket.Data;

            Ver = BitConverter.ToInt16(DatagramData, 0);
            SType = BitConverter.ToInt16(DatagramData, 2);
            ConnectId = BitConverter.ToInt32(DatagramData, 4);
            ClientId = BitConverter.ToInt32(DatagramData, 8);
            PingId = BitConverter.ToInt32(DatagramData, 12);
        }
    }
}
