using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP
{
    class UDPInputStream
    {
        Receiver MyReceiver;
        bool StreamClosed = false;
        ConnectionUDP Conn;

        public UDPInputStream(ConnectionUDP conn)
        {
            Conn = conn;
            MyReceiver = conn.MyRecevier;
        }

        public int Read(byte[] data, int offset, int length)
        {
            byte[] rawData = MyReceiver.Receive();
            if (length >= rawData.Length)
            {
                Array.Copy(rawData, 0, data, offset, rawData.Length);
                return rawData.Length;
            }
            else
            {
                throw new Exception("Error 5");
            }
        }

        public void CloseLocalStream()
        {
            if (!StreamClosed)
            {
                MyReceiver.CloseLocalStream();
            }
        }
    }
}
