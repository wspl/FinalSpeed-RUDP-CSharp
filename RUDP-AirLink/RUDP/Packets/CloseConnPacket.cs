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
        byte[] DatagramData;

        public CloseConnPacket(int connectId, int clientId)
        {
            SType = PacketType.CloseConnPacket;

            DatagramData = new byte[12];
            ClientId = clientId;
            ConnectId = connectId;

            BitConverter.GetBytes(Ver).CopyTo(DatagramData, 0);
            BitConverter.GetBytes(SType).CopyTo(DatagramData, 2);
            BitConverter.GetBytes(ConnectId).CopyTo(DatagramData, 4);
            BitConverter.GetBytes(ClientId).CopyTo(DatagramData, 8);

            MyDatagramPacket = new DatagramPacket(DatagramData, DatagramData.Length);
        }

        public CloseConnPacket(DatagramPacket datagramPacket)
        {
            MyDatagramPacket = datagramPacket;
            DatagramData = datagramPacket.Data;

            Ver = BitConverter.ToInt16(DatagramData, 0);
            SType = BitConverter.ToInt16(DatagramData, 2);
            ConnectId = BitConverter.ToInt32(DatagramData, 4);
            ClientId = BitConverter.ToInt32(DatagramData, 8);
        }
    }
}
