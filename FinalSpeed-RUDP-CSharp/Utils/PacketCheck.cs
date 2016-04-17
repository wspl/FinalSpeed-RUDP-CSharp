using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalSpeed_RUDP_CSharp.RUDP;

namespace FinalSpeed_RUDP_CSharp.Utils
{
    public class PacketCheck
    {
        public static int CheckVer(DatagramPacket dp) => BitConverter.ToInt16(dp.Data, 0);
        public static int CheckSType(DatagramPacket dp) => BitConverter.ToInt16(dp.Data, 2);
    }
}
