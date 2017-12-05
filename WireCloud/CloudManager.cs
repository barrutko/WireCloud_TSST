using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WireCloud
{
    class CloudManager
    {
        public CloudManager(LinkTable linkTable, int port)
        {
            IPEndPoint managingEndPoint = new IPEndPoint(IPAddress.Any, 0);
            EndPoint manager = (EndPoint)managingEndPoint;

            Socket orderingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            orderingSocket.Bind(endPoint);
            LogWriter.notificationMessage("Manager communication ready", true);
            while(true)
            {
                byte[] buffer = new byte[orderingSocket.SendBufferSize];
                int readBytesNumber = orderingSocket.ReceiveFrom(buffer, ref manager);
                byte[] receivedData = new byte[readBytesNumber];
                Array.Copy(buffer, receivedData, readBytesNumber);
                ManagementDataObjects.MdoForCloud mdoForCloud = ByteToMdoForCloud(receivedData);

                LogWriter.newOrderFromManager();

                bool decision;
                if (mdoForCloud.Command.Equals("LinkDown"))
                    decision = false;
                else if (mdoForCloud.Command.Equals("LinkUp"))
                    decision = true;
                else
                {
                    LogWriter.generalMessage("Invalid command", true);
                    continue;
                }
                
                if (!linkTable.controlSpecificLink(mdoForCloud.DeviceFrom, mdoForCloud.DeviceTo, decision))
                    LogWriter.generalMessage("Order had no effect", true);
            }
            
        }
        static private ManagementDataObjects.MdoForCloud ByteToMdoForCloud(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Seek(0, SeekOrigin.Begin);
            object o = (object)bf.Deserialize(ms);
            return (ManagementDataObjects.MdoForCloud)o;
        }
    }
}
