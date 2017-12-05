using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WireCloud
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 100;
            Console.BufferWidth = 100;
            Console.Title = "WireCloud";
            LogWriter.startingMessage();
            
            string linksPath = Environment.CurrentDirectory + "\\Links.txt";
            LinkTable linkTable = new LinkTable(linksPath);
            
            string devicesPath = Environment.CurrentDirectory + "\\Devices.txt";
            DeviceContainer deviceContainer = new DeviceContainer(devicesPath, linkTable);
            int managerPortNumber;
            Thread managerThread;
            if (args.Length>0)
            {
                if(Int32.TryParse(args[0], out managerPortNumber))
                {
                    managerThread = new Thread(() => new CloudManager(linkTable, managerPortNumber));
                }
                else
                {
                    managerThread = new Thread(() => new CloudManager(linkTable, 56012));
                }
            }
            else
            {
                managerThread = new Thread(() => new CloudManager(linkTable, 56012));
            }
            
            managerThread.Start();
            deviceContainer.initializeSockets();
            deviceContainer.run();
        }
    }
}
