using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSpeed_RUDP_CSharp.RUDP
{
    public class RUDPConfig
    {
        public static short ProtocalVer = 0;

        public static int PackageSize = 1000;

        public static bool TwiceUdp = false;
        public static bool TwiceTcp = false;

        public static int MaxWin = 5 * 1024;

        public static int AckListDelay = 5;
        public static int AckListSum = 300;

        public static bool DoubleSendStart = true;

        public static int ReSendDelayMin = 100;
        public static float ReSendDelay = 0.37f;
        public static int ReSendTryTimes = 10;

    }
}
