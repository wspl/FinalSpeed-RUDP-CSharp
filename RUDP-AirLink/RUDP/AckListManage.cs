using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP
{
    class AckListManage
    {
        Thread MainThread;
        Dictionary<int, AckListTask> TaskTable;

        public AckListManage()
        {
            TaskTable = new Dictionary<int, AckListTask>();
            MainThread = new Thread(new ThreadStart(Run));
            MainThread.Start();
        }

        public void AddAck()
        {
            //TODO
        }

            

        public void Run()
        {
            
        }
    }
}
