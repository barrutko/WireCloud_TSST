using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WireCloud
{
    class Device
    {
        int sendToPortNumber;
        int listenFromPortNumber;
        string callSign;
        string deviceType;
        string IDNumber;
        
        Socket sendingSocket;
        Socket listeningSocket;
        Socket handlingSocket;
        
        
        public Device (string deviceType, string IDNumber, int listenFromPortNumber, int sendToPortNumber)
        {            
            this.deviceType = deviceType;
            this.IDNumber = IDNumber;
            callSign = deviceType + "." + IDNumber;
            this.listenFromPortNumber = listenFromPortNumber;
            this.sendToPortNumber = sendToPortNumber;
        }
        
        public string getCallSign()
        {
            return callSign;
        }

        public void initializeSockets()
        {
            LogWriter.generalMessage("Waiting for " + callSign + " to connect...", false);

            IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
            IPAddress iPAddress = iPHostEntry.AddressList[0];

            this.listeningSocket = newSocket();
            IPEndPoint receiveFrom = new IPEndPoint(iPAddress, listenFromPortNumber);
            listeningSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listeningSocket.Bind(receiveFrom);
            listeningSocket.Listen(0);            
            handlingSocket = listeningSocket.Accept();

            sendingSocket = newSocket(); ;
            IPEndPoint sendTo = new IPEndPoint(iPAddress, sendToPortNumber);
            waitForConnection(sendingSocket, sendTo);
            
            LogWriter.generalMessage("Connected to " + callSign + ". Listening on: " + listenFromPortNumber + ". Sending to: " + sendToPortNumber + ".", true);
           
        }
        private static Socket newSocket()
        {
            IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
            IPAddress iPAddress = iPHostEntry.AddressList[0];
            return new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); ;
        }

        private static void waitForConnection(Socket socket, IPEndPoint ipEndPoint)
        {
            while (true)
            {
                try
                {
                    socket.Connect(ipEndPoint);
                    break;
                }
                catch (SocketException ex)
                {

                }
            }
        }

        public void reviveConnection()
        {

            handlingSocket.Disconnect(true);
            handlingSocket.Close();
            handlingSocket.Dispose();
            handlingSocket = null;

            sendingSocket.Disconnect(true);
            sendingSocket.Close();
            sendingSocket.Dispose();
            sendingSocket = null;

            handlingSocket = listeningSocket.Accept();
            IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
            IPAddress iPAddress = iPHostEntry.AddressList[0];
            sendingSocket = newSocket(); ;
            IPEndPoint sendTo = new IPEndPoint(iPAddress, sendToPortNumber);
            waitForConnection(sendingSocket, sendTo);
            LogWriter.deviceRevived(callSign, true);

        }

       
        public void Send(byte[] data)
        {
            sendingSocket.Send(data);
        }

        public byte[] Receive()
        {
            byte[] buffer = new byte[handlingSocket.SendBufferSize];
            int readBytesNumber = handlingSocket.Receive(buffer);
            byte[] receivedData = new byte[readBytesNumber];
            Array.Copy(buffer, receivedData, readBytesNumber);
            return receivedData;
        }
    }
}
