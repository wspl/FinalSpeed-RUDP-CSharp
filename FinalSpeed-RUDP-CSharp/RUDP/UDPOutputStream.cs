using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSpeed_RUDP_CSharp.RUDP
{
    public class UDPOutputStream
    {
        public ConnectionUDP Conn;

        string DstHost;
        int DstPort;

        Sender MySender;

        public UDPOutputStream(ConnectionUDP conn)
        {
            Conn = conn;
            DstHost = conn.DstHost;
            DstPort = conn.DstPort;
            MySender = conn.MySender;
        }

        public void Write(byte[] data, int offset, int length)
        {
            MySender.SendData(data, offset, length);
        }

        public void CloseLocalStream() => MySender.CloseLocalStream();
    }
}
