using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalSpeed_RUDP_CSharp.RUDP
{
    public interface TrafficListener
    {
        void TrafficDownload(TrafficEvent e);
        void TrafficUpload(TrafficEvent e);
    }
}
