using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP
{
    class DatagramPacket
    {
        public byte[] Dgram { set; get; }
        public int Bytes { set; get; }
        public string Host { set; get; }
        public int Port { set; get; }

        public DatagramPacket(byte[] dgram, int bytes)
        {
            Dgram = dgram;
            Bytes = bytes;
        }

        public DatagramPacket(byte[] dgram, int bytes, string host, int port)
        {
            Dgram = dgram;
            Bytes = bytes;
            Host = host;
            Port = port;
        }
    }
}
