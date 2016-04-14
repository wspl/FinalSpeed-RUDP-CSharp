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

        object SynAck = new object();

        public AckListManage()
        {
            TaskTable = new Dictionary<int, AckListTask>();
            MainThread = new Thread(new ThreadStart(Run));
            MainThread.Start();
        }

        public void AddAck(ConnectionUDP conn, int sequence)
        {
            lock (SynAck)
            {
                if (!TaskTable.ContainsKey(conn.ConnectId))
                {
                    AckListTask at = new AckListTask(conn);
                    TaskTable.Add(conn.ConnectId, at);
                }

                AckListTask at = TaskTable[conn.ConnectId];
                at.AddAck(sequence);
            }
        }

        public void AddLastRead(ConnectionUDP conn)
        {
            lock (SynAck)
            {
                if (!TaskTable.ContainsKey(conn.ConnectId))
                {
                    AckListTask at = new AckListTask(conn);
                    TaskTable.Add(conn.ConnectId, at);
                }
            }
        }

        public void Run()
        {
            while (true)
            {
                lock (SynAck) {
                    foreach (KeyValuePair<int, AckListTask> at in TaskTable)
                    {
                        at.Run();
                    }
                    TaskTable.Clear();
                }

                //try
                Thread.Sleep(RUDPConfig.AckListDelay);
            }
        }
    }
}
