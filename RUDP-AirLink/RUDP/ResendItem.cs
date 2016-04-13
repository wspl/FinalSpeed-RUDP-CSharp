using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP
{
    class ResendItem
    {
        public ConnectionUDP Conn { get; set; }

        public int Count { get; set; }

        public int Sequence { get; set; }
        public long ResendTime { get; set; }

        public ResendItem(ConnectionUDP conn, int sequence)
        {
            Conn = conn;
            Sequence = sequence;
        }

        public void AddCount() => Count += 1;
    }
}
