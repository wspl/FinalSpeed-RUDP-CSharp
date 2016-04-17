using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSpeed_RUDP_CSharp.RUDP.Packets
{
    public class CloseStreamPacket : Packet
    {
        byte[] Data;
        byte[] DatagramData;

        public int CloseOffset { get; set; }

        public CloseStreamPacket(int connectId, int clientId, int closeOffset)
        {
            SType = PacketType.CloseStreamPacket;

            DatagramData = new byte[16];
            ClientId = clientId;
            ConnectId = connectId;
            CloseOffset = closeOffset;

            BitConverter.GetBytes(Ver).CopyTo(DatagramData, 0);
            BitConverter.GetBytes(SType).CopyTo(DatagramData, 2);
            BitConverter.GetBytes(ConnectId).CopyTo(DatagramData, 4);
            BitConverter.GetBytes(ClientId).CopyTo(DatagramData, 8);
            BitConverter.GetBytes(CloseOffset).CopyTo(DatagramData, 12);

            MyDatagramPacket = new DatagramPacket(DatagramData, DatagramData.Length);
        }

        public CloseStreamPacket(DatagramPacket datagramPacket)
        {
            MyDatagramPacket = datagramPacket;
            DatagramData = datagramPacket.Data;

            Ver = BitConverter.ToInt16(DatagramData, 0);
            SType = BitConverter.ToInt16(DatagramData, 2);

            ConnectId = BitConverter.ToInt32(DatagramData, 4);
            ClientId = BitConverter.ToInt32(DatagramData, 8);
            CloseOffset = BitConverter.ToInt32(DatagramData, 12);
        }
    }
}
