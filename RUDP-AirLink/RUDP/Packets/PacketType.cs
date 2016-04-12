using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP.Packets
{
    class PacketType
    {
        public static short DataPacket = 80;
        public static short ConnectPacket1 = 71;
        public static short ConnectPacket2 = 72;
        public static short ConnectPacket3 = 73;
        public static short CloseStreamPacket = 75;
        public static short CloseConnPacket = 76;

        public static short AckPacket = 61;
        public static short LastReadPacket = 65;
        public static short AckListPacket = 60;

        public static short AskFillPacket = 63;
        public static short ReSendPacket = 62;

        public static short AckListPacketTun = 66;


        public static short UdpTunDataPacket = 90;
        public static short UdpTunOpenPacket = 91;
        public static short UdpTunClosePacket = 92;

        public static short CleanPacket1 = 225;
        public static short CleanPacket2 = 226;

        public static short RegPacket = 101;
        public static short RegPacket2 = 102;
        public static short ExitPacket = 111;
        public static short ExitPacket2 = 112;
        public static short PubAddPacket = 141;
        public static short PubAddPacket2 = 142;
        public static short PubDelPacket = 151;
        public static short PubDelPacket2 = 152;
        public static short SLivePacket = 131;
        public static short SLivePacket2 = 132;
        public static short GetSNodePacket = 161;
        public static short GetSNodePacket2 = 162;
        public static short JoinDBFailPacket1 = 168;
        public static short JoinDBFailPacket2 = 169;
        public static short TimeSynPacket1 = 175;
        public static short TimeSynPacket2 = 176;
        public static short AdvCSPacket1 = 178;
        public static short AdvCSPacket2 = 179;

        public static short CastGroupPacket = 181;
        public static short CastGroupPacket2 = 182;
        public static short CastGroupRandomPacket = 191;
        public static short CastGroupRandomPacket2 = 192;
        public static short CastGroupRandomPacket3 = 193;

        public static short Assist_RegPacket = 500;
        public static short Assist_RegPacket2 = 501;
        public static short Assist_PingPacket1 = 510;
        public static short Assist_PingPacket2 = 511;
        public static short ReversePingPacket1 = 515;
        public static short ReversePingPacket2 = 516;
        public static short ReversePingPacket3 = 517;
        public static short ReverseConnTCPPacket1 = 518;
        public static short ReverseConnTCPPacket2 = 519;
        public static short AssistOtherOutAddressPacket = 520;
        public static short AssistOtherOutAddressPacket2 = 521;
        public static short AssistOtherOutAddressPacket3 = 522;
        public static short AssistOtherOutAddressPacket4 = 523;

        public static short AssistLivePacket = 4125;
        public static short AssistLivePacket2 = 4126;
        public static short AssistExitPacket = 540;
        public static short AssistExitPacket2 = 541;


        public static short DBCastAddPacket = 601;
        public static short DBCastAddPacket2 = 602;
        public static short DBCastRemovePacket = 701;
        public static short DBCastRemovePacket2 = 702;
        public static short DBSourceSearchPacket = 711;
        public static short DBSourceSearchPacket2 = 712;
        public static short DBSourceSumPacket1 = 715;
        public static short DBSourceSumPacket2 = 716;

        public static short GetOutAddressPacket = 1181;
        public static short GetOutAddressPacket2 = 1182;

        public static short PingPacket = 301;
        public static short PingPacket2 = 302;

        public static short PingPacketc = 311;
        public static short PingPacketc2 = 312;

        public static short PingPacketr = 321;
        public static short PingPacketr2 = 322;
    }
}
