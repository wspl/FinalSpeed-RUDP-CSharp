using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinalSpeed_RUDP_CSharp.RUDP
{
    public class AckListManage
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
                AckListTask ackListTask;

                if (!TaskTable.ContainsKey(conn.ConnectId))
                {
                    ackListTask = new AckListTask(conn);
                    TaskTable.Add(conn.ConnectId, ackListTask);
                }

                ackListTask = TaskTable[conn.ConnectId];
                ackListTask.AddAck(sequence);
            }
        }

        public void AddLastRead(ConnectionUDP conn)
        {
            lock (SynAck)
            {
                if (!TaskTable.ContainsKey(conn.ConnectId))
                {
                    AckListTask ackListTask = new AckListTask(conn);
                    TaskTable.Add(conn.ConnectId, ackListTask);
                }
            }
        }

        public void Run()
        {
            while (true)
            {
                lock (SynAck) {
                    foreach (KeyValuePair<int, AckListTask> ackListTask in TaskTable)
                    {
                        ackListTask.Value.Run();
                    }
                    TaskTable.Clear();
                }

                //try
                Thread.Sleep(RUDPConfig.AckListDelay);
            }
        }
    }
}
