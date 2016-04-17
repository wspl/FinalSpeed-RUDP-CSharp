using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP
{
    class TrafficEvent
    {
        long EventId;
        int Traffic { get; }

        public static int Download = 10;
        public static int Upload = 11;

        public int Type { get; } = 10;

        string UserId;

        public TrafficEvent(string userId, long eventId, int traffic, int type)
        {
            UserId = userId;
            EventId = eventId;
            Traffic = traffic;
            Type = type;
        }
    }
}
