using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP
{
    class DatagramPacket
    {
        public byte[] Data { set; get; }
        public int Bytes { set; get; }
        public string Host { set; get; }
        public int Port { set; get; }

        public DatagramPacket(byte[] data, int bytes)
        {
            Data = data;
            Bytes = bytes;
        }

        public DatagramPacket(byte[] data, int bytes, string host, int port)
        {
            Data = data;
            Bytes = bytes;
            Host = host;
            Port = port;
        }
    }
}
