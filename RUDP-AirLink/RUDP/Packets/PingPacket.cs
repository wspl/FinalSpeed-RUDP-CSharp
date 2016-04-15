using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP.Packets
{
    class PingPacket : Packet
    {
        byte[] DatagramData = new byte[20];

        public int PingId { get; set; }

        public int DownloadSpeed { get; set; }
        public int UploadSpeed { get; set; }

        public PingPacket(int connectId, int clientId, int pingId, 
                          int downloadSpeed, int uploadSpeed)
        {
            SType = PacketType.PingPacket;

            BitConverter.GetBytes(Ver).CopyTo(DatagramData, 0);
            BitConverter.GetBytes(SType).CopyTo(DatagramData, 2);
            BitConverter.GetBytes(connectId).CopyTo(DatagramData, 4);
            BitConverter.GetBytes(clientId).CopyTo(DatagramData, 8);
            BitConverter.GetBytes(pingId).CopyTo(DatagramData, 12);
            BitConverter.GetBytes((short)(downloadSpeed / 1024)).CopyTo(DatagramData, 16);
            BitConverter.GetBytes((short)(uploadSpeed / 1024)).CopyTo(DatagramData, 18);

            MyDatagramPacket = new DatagramPacket(DatagramData, DatagramData.Length);
        }

        
        public PingPacket(DatagramPacket datagramPacket)
        {
            MyDatagramPacket = datagramPacket;
            DatagramData = datagramPacket.Data;

            Ver = BitConverter.ToInt16(DatagramData, 0);
            SType = BitConverter.ToInt16(DatagramData, 2);
            ConnectId = BitConverter.ToInt32(DatagramData, 4);
            ClientId = BitConverter.ToInt32(DatagramData, 8);
            PingId = BitConverter.ToInt32(DatagramData, 12);
            DownloadSpeed = BitConverter.ToInt32(DatagramData, 16);
            UploadSpeed = BitConverter.ToInt32(DatagramData, 18);
        }
    }
}
