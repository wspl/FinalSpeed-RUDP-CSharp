using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RUDP_AirLink.RUDP
{
    interface PipeListener
    {
        void PipeClose();
    }

    class StreamPipe
    {
        BinaryReader MyBinaryReader;
        BinaryWriter MyBinaryWriter;

        List<PipeListener> ListenerList;

        bool Closed = false;
        int MaxSpeed = 100 * 1024 * 1024;
        public int Port { get; set; } = 0;
        public int LimitConnectTime { get; set; }
        public string UserId { get; set; } = "";
        byte[] PreReadData;

        Socket MySocketA { get; set; }
        Socket MySocketB { get; set; }

        int BufferSize;

        List<byte[]> DataList = new List<byte[]>();

        public int SuperSocketId { get; set; } = -1;

        static int TypeRequest = 1;
        static int TypeResponse = 2;

        public int Type { get; set; } = 0;

        public StreamPipe(BinaryReader binaryReader, BinaryWriter binaryWriter, 
                          int bufferSize, int maxSpeed,
                          byte[] preReadData = null, int preReadDataLength = 0)
        {
            ListenerList = new List<PipeListener>();
            MaxSpeed = maxSpeed;
            PreReadData = preReadData;
            BufferSize = bufferSize;

            Task.Factory.StartNew(() => {
                byte[] data = new byte[bufferSize];
                int len = 0;

                //try
                if (preReadData != null)
                {
                    binaryWriter.Write(preReadData, 0, preReadDataLength);
                }
                while ((len = binaryReader.Read(/*data?*/)) > 0)
                {
                    binaryWriter.Write(data, 0, len);
                }

                Close();
            });
        }

        void Close()
        {
            if (!Closed)
            {
                Closed = true;
                Thread.Sleep(500);

                if (MySocketA != null)
                {

                    Task.Factory.StartNew(() => {
                        MySocketA.Close();
                    });
                }

                if (MySocketB != null)
                {

                    Task.Factory.StartNew(() => {
                        MySocketB.Close();
                    });
                }

                foreach (PipeListener pipeListener in ListenerList)
                {
                    pipeListener.PipeClose();
                }
            }
        }
    }
}
