using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace WireCloud
{
    class DeviceContainer
    {
        List<Device> deviceList;
        LinkTable linkTable;

        public void addDevice(string deviceType, string IDNumber, int listenFromPortNumber, int sendToPortNumber)
        {
            deviceList.Add(new Device(deviceType, IDNumber, listenFromPortNumber, sendToPortNumber));
        }
        public void addDevice(Device newDevice)
        {
            deviceList.Add(newDevice);
        }
        public Device searchByCallSign (string callSign)
        {
            foreach (Device device in deviceList)
            {
                if (callSign.Equals(device.getCallSign()))
                     return device;
            }
            return new Device("", "", 0, 0);
                        
        }
        
        public DeviceContainer(string configurationFilePath, LinkTable linkTable)
        {
            try
            {
                this.linkTable = linkTable;
                string[] deviceDefinitions = System.IO.File.ReadAllLines(configurationFilePath);
                deviceList = new List<Device>();
                foreach (string definition in deviceDefinitions)
                {
                    string[] definitionParameters = definition.Split('.');
                    string deviceType = definitionParameters[0];
                    string IDNumber = definitionParameters[1];
                    int listenFromPortNumber = Int32.Parse(definitionParameters[2]);
                    int sendToPortNumber = Int32.Parse(definitionParameters[3]);
                    this.addDevice(deviceType, IDNumber, listenFromPortNumber, sendToPortNumber);
                }
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Console.WriteLine("Add proper config file (Devices.txt) and restart program");
                while (true) ;
            }
        }

        public void initializeSockets()
        {
            
            LogWriter.notificationMessage("Initializing processes started", false);
            int deviceNumber = deviceList.Count;
            Thread[] threads = new Thread[deviceNumber];
            
            int i = 0;
            foreach(Device device in deviceList)
            {
                threads[i] = new Thread(() => device.initializeSockets());
                threads[i].Start();
                i++;
            }

            foreach(Thread thread in threads)
            {
                thread.Join();
            }
            LogWriter.notificationMessage("Initialization finished", true);
            

        }

        

        public void deviceService(Device device)
        {
            while (true)
            {
                try
                {
                    byte[] receivedData = device.Receive();
                    LogWriter.receivedMessage(receivedData, device.getCallSign(), true);
                    byte gate = receivedData[0];
                    Interface receivingInterface = new Interface(device.getCallSign(), gate);
                    Interface destinationInterface = linkTable.getConnectedInterface(receivingInterface);
                    if (destinationInterface != null)
                    {

                        receivedData[0] = destinationInterface.getGate();
                        searchByCallSign(destinationInterface.getDeviceCallSign()).Send(receivedData);
                        LogWriter.sendMessage(receivedData, destinationInterface.getDeviceCallSign(), true);
                    }
                    else
                    {
                        LogWriter.dataLossMessage(receivedData, true);
                    }
                }
                catch (SocketException ex)
                {
                    LogWriter.deviceDown(device.getCallSign(), true);
                    linkTable.killConnectionsWith(device.getCallSign());
                    device.reviveConnection();
                    linkTable.reviveConnectionsWith(device.getCallSign());
                }
            }

        }

        public void run()
        {
            LogWriter.generalMessage("Cloud ready...", false);
            int deviceNumber = deviceList.Count;
            Thread[] threads = new Thread[deviceNumber];

            int i = 0;
            foreach (Device device in deviceList)
            {
                threads[i] = new Thread(() => deviceService(device));
                threads[i].Start();
                i++;
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }



    }

    
}
