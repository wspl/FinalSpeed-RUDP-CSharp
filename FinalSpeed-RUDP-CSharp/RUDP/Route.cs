using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Net;

using FinalSpeed_RUDP_CSharp.Utils;
using FinalSpeed_RUDP_CSharp.RUDP.Packets;

namespace FinalSpeed_RUDP_CSharp.RUDP
{

    public class Route
    {
        private UdpClient MyUdpClient;

        public Dictionary<int, ConnectionUDP> ConnTable = new Dictionary<int, ConnectionUDP>();

        Thread MainThread;
        Thread ReceiveThread;

        public AckListManage DelayAckManage = new AckListManage();

        Random Ran = new Random();

        public int LocalClientId;

        BlockingCollection<DatagramPacket> PacketBuffer = new BlockingCollection<DatagramPacket>();

        public static int ServerMode = 2;
        public static int ClientMode = 1;
        public int Mode = 1;

        string ProcessName = "";

        HashSet<int> SettedTable = new HashSet<int>();
        HashSet<int> ClosedTable = new HashSet<int>();

        public int LocalDownloadSpeed;
        public int LocalUploadSpeed;

        public ClientManager MyClientManager;
        public ClientControl LastClientControl;

        public static List<TrafficListener> ListenerList = new List<TrafficListener>();

        public Route(int mode, int port = 150)
        {
            LocalClientId = Ran.Next();
            Mode = mode;

            if (mode == ClientMode)
            {
                MyUdpClient = new UdpClient();
            }
            else if (mode == ClientMode)
            {
                MyUdpClient = new UdpClient(port);
            }

            MyClientManager = new ClientManager(this);
            Task.Factory.StartNew(() => {
                while (true)
                {
                    try
                    {
                        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] data = MyUdpClient.Receive(ref RemoteIpEndPoint);
                        DatagramPacket datagramPacket = new DatagramPacket(data, data.Length);
                        PacketBuffer.Add(datagramPacket);
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                }
            });

            Task.Factory.StartNew(() => {
                while (true)
                {
                    DatagramPacket datagramPacket = PacketBuffer.Take();
                    if (datagramPacket == null) continue;

                    byte[] data = datagramPacket.Data;
                    if (data.Length < 4)
                    {
                        return;
                    }

                    int sType = PacketCheck.CheckSType(datagramPacket);
                    int connectId = BitConverter.ToInt32(data, 4);
                    int remoteClientId = BitConverter.ToInt32(data, 8);

                    if (ClosedTable.Contains(connectId) && connectId != 0) continue;

                    ClientControl clientControl;
                    if (sType == PacketType.PingPacket || sType == PacketType.PingPacket2)
                    {
                        if (mode == ClientMode)
                        {
                            string key = datagramPacket.Host + ":" + datagramPacket.Port;
                            int simClientId = Math.Abs(key.GetHashCode());
                            clientControl = MyClientManager.GetClientControl(simClientId, datagramPacket.Host, datagramPacket.Port);
                        }
                        else if (mode == ServerMode)
                        {
                            clientControl = MyClientManager.GetClientControl(remoteClientId, datagramPacket.Host, datagramPacket.Port);
                        }
                    }
                    else
                    {
                        if (mode == ClientMode)
                        {
                            if (!SettedTable.Contains(remoteClientId))
                            {
                                string key = datagramPacket.Host + ":" + datagramPacket.Port;
                                int simClientId = Math.Abs(key.GetHashCode());
                                clientControl = MyClientManager.GetClientControl(simClientId, datagramPacket.Host, datagramPacket.Port);

                                if (clientControl.ClientIdReal == -1)
                                {
                                    clientControl.ClientIdReal = remoteClientId;
                                }
                                else if (clientControl.ClientIdReal != remoteClientId)
                                {
                                    clientControl.UpdateClientId(remoteClientId);
                                }

                                SettedTable.Add(remoteClientId);
                            }
                        }
                        else if (mode == ServerMode)
                        {
                            GetConnection2(datagramPacket.Host, datagramPacket.Port, connectId, remoteClientId);
                        }

                        ConnectionUDP conn = ConnTable[connectId];
                        if (conn != null)
                        {
                            conn.MyRecevier.OnReceivePacket(datagramPacket);
                            if (sType == PacketType.DataPacket)
                            {
                                TrafficEvent e = new TrafficEvent("", Ran.Next(), datagramPacket.Data.Length, TrafficEvent.Download);
                                FireEvent(e);
                            }
                        }
                    }
                }
            });
        }

        public static void AddTrafficListener(TrafficListener listener) => ListenerList.Add(listener);

        public static void FireEvent(TrafficEvent e)
        {
            foreach (TrafficListener listener in ListenerList)
            {
                int type = e.Type;
                if (type == TrafficEvent.Download)
                {
                    listener.TrafficDownload(e);
                }
                else if (type == TrafficEvent.Upload)
                {
                    listener.TrafficUpload(e);
                }
            }
        }

        public void SendPacket(DatagramPacket datagramPacket)
        {
            MyUdpClient.Send(datagramPacket.Data, datagramPacket.Bytes);
        }

        public ConnectionUDP GetConnection2(string dstHost, int dstPort, int connectId, int clientId)
        {
            ConnectionUDP conn = ConnTable[connectId];
            if (conn == null)
            {
                ClientControl clientControl = MyClientManager.GetClientControl(clientId, dstHost, dstPort);
                conn = new ConnectionUDP(this, dstHost, dstPort, 2, connectId, clientControl);

                lock (this)
                {
                    ConnTable.Add(connectId, conn);
                }
                clientControl.AddConnection(conn);
            }
            return conn;
        }

        public void RemoveConnection(ConnectionUDP conn)
        {
            lock(this)
            {
                ClosedTable.Add(conn.ConnectId);
                ConnTable.Remove(conn.ConnectId);
            }
        }
    }
}
