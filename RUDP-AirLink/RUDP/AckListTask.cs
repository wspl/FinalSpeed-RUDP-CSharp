using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RUDP_AirLink.RUDP.Packets;
using RUDP_AirLink.Utils;

namespace RUDP_AirLink.RUDP
{
    class AckListTask
    {
        ConnectionUDP Conn;

        List<int> AckList = new List<int>();
        HashSet<int> Set = new HashSet<int>();

        object SynAck = new object();

        public AckListTask(ConnectionUDP conn)
        {
            Conn = conn;
        }

        public void AddAck(int sequence)
        {
            lock (SynAck)
            {
                if (!Set.Contains(sequence))
                {
                    AckList.Add(sequence);
                    Set.Add(sequence);
                }
            }
        }

        public void Run()
        {
            int offset = 0;
            int packetLength = RUDPConfig.AckListSum;
            int length = AckList.Count;

            int sum = (length / packetLength);
            if (length % packetLength != 0)
            {
                sum += 1;
            }
            if (sum == 0)
            {
                sum = 1;
            }
            int len = packetLength;
            if (length < len)
            {
                Conn.MySender.SendAckListPacket(AckList);
                Conn.MySender.SendAckListPacket(AckList);
            }
            else
            {
                for (int i = 0; i < sum; i += 1)
                {
                    List<int> nl = Copy(offset, len, AckList);
                    Conn.MySender.SendAckListPacket(nl);
                    Conn.MySender.SendAckListPacket(nl);

                    offset += packetLength;
                    if (offset + len > length)
                    {
                        len = length - (sum - 1) * packetLength;
                    }
                }
            }
        }

        List<int> Copy(int offset, int length, List<int> ackList)
        {
            List<int> nl = new List<int>();
            for (int i = 0; i < length; i += 1)
            {
                nl.Add(ackList[offset + i]);
            }
            return nl;
        }
    }
}
