using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSpeed_RUDP_CSharp.RUDP.Packets
{
    public class Packet
    {

        public short SType { get; set; } = 0;
        public short Ver { get; set; } = RUDPConfig.ProtocalVer;

        public DatagramPacket MyDatagramPacket { get; set; }

        public int ConnectId { get; set; }
        public int ClientId { get; set; }

        public string DstHost { get; set; }
        public int DstPort { get; set; }
    }
}
