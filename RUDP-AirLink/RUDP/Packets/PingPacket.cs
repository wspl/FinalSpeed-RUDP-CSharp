using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP.Packets
{
    class PingPacket : Packet
    {
        byte[] DpData = new byte[20];

        public int PingId { get; set; }

        public int DownloadSpeed { get; set; }
        public int UploadSpeed { get; set; }

        public PingPacket(int connectId, int clientId, int pingId, 
                          int downloadSpeed, int uploadSpeed)
        {
            SType = PacketType.PingPacket;

            BitConverter.GetBytes(Ver).CopyTo(DpData, 0);
            BitConverter.GetBytes(SType).CopyTo(DpData, 2);
            BitConverter.GetBytes(connectId).CopyTo(DpData, 4);
            BitConverter.GetBytes(clientId).CopyTo(DpData, 8);
            BitConverter.GetBytes(pingId).CopyTo(DpData, 12);
            BitConverter.GetBytes((short)(downloadSpeed / 1024)).CopyTo(DpData, 16);
            BitConverter.GetBytes((short)(uploadSpeed / 1024)).CopyTo(DpData, 18);

            Dp = new DatagramPacket(DpData, DpData.Length);
        }

        
        public PingPacket(DatagramPacket dp)
        {
            Dp = dp;
            DpData = dp.Dgram;

            Ver = BitConverter.ToInt16(DpData, 0);
            SType = BitConverter.ToInt16(DpData, 2);
            ConnectId = BitConverter.ToInt32(DpData, 4);
            ClientId = BitConverter.ToInt32(DpData, 8);
            PingId = BitConverter.ToInt32(DpData, 12);
            DownloadSpeed = BitConverter.ToInt32(DpData, 16);
            UploadSpeed = BitConverter.ToInt32(DpData, 18);
        }
    }
}
