using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RUDP_AirLink.Utils;
using RUDP_AirLink.RUDP.Packets;

namespace RUDP_AirLink.RUDP
{
    class ResendManage
    {
        Thread MainThread;

        bool HaveTask = false;
        object SingleOb = new object();
        long VTime = 0;
        long LastResendTime;

        BlockingCollection<ResendItem> TaskList = new BlockingCollection<ResendItem>();

        public ResendManage()
        {
            Task.Factory.StartNew(Run);
        }

        public void AddTask(ConnectionUDP conn, int sequence)
        {
            ResendItem resendItem = new ResendItem(conn, sequence);
            resendItem.ResendTime = GetNewResendTime(conn);
            TaskList.Add(resendItem);
        }

        long GetNewResendTime(ConnectionUDP conn)
        {
            int delayAdd = conn.MyClientControl.PingDelay + 
                (int)((float)conn.MyClientControl.PingDelay * RUDPConfig.ReSendDelay);

            if (delayAdd < RUDPConfig.ReSendDelayMin)
            {
                delayAdd = RUDPConfig.ReSendDelayMin;
            }

            return DateTimeExtensions.CurrentTimeMillis() + delayAdd;
        }

        public void Run()
        {
            while (true)
            {
                // try
                ResendItem resendItem = TaskList.Take();

                if (resendItem.Conn.IsConnected)
                {
                    long sleepTime = resendItem.ResendTime - DateTimeExtensions.CurrentTimeMillis();
                    if (sleepTime > 0)
                    {
                        while (sleepTime - Int32.MaxValue > 0)
                        {
                            sleepTime -= Int32.MaxValue;
                            Thread.Sleep(Int32.MaxValue);
                        }
                        Thread.Sleep((int)sleepTime);
                    }
                    resendItem.AddCount();

                    if (resendItem.Conn.MySender.GetDataPacket(resendItem.Sequence) != null)
                    {
                        if (!resendItem.Conn.StopNow)
                        {
                            resendItem.Conn.MySender.Resend(resendItem.Sequence, resendItem.Count);
                        }
                    }

                    if (resendItem.Count < RUDPConfig.ReSendTryTimes)
                    {
                        resendItem.ResendTime = GetNewResendTime(resendItem.Conn);
                        TaskList.Add(resendItem);
                    }
                }
                if (resendItem.Conn.MyClientControl.Closed)
                {
                    break;
                }
            }
        }
    }
}