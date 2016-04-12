using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP.Packets
{
    class Packet
    {

        public short SType { get; set; } = 0;
        public short Ver { get; set; } = RUDPConfig.ProtocalVer;

        public DatagramPacket Dp { get; set; }

        public int ConnectId { get; set; }
        public int ClientId { get; set; }

    }
}
