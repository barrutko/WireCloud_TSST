using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireCloud
{
    class LinkTable
    {
        List<Link> linkList;

        public LinkTable(string configurationFilePath)
        {
            try
            {
                string[] linkDefinitions = File.ReadAllLines(configurationFilePath);
            
            

            linkList = new List<Link>();
            foreach (string definition in linkDefinitions)
            {
                string[] definitionParameters = definition.Split(';');
                string callSignL = definitionParameters[0];
                string callSignR = definitionParameters[2];
                byte gateL = Byte.Parse(definitionParameters[1]);
                byte gateR = Byte.Parse(definitionParameters[3]);
                linkList.Add(new Link(callSignL, gateL, callSignR, gateR));

            }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Add proper config file (Links.txt) and restart program");
                while (true) ;
            }
        }

        public Interface getConnectedInterface (Interface connectedTo)
        {
            
            Interface connectedInterface;
            foreach(Link link in linkList)
            {

                connectedInterface = link.getConnection(connectedTo);
                if (connectedInterface != null)
                {
                    return connectedInterface;
                }
                    
            }
            return null;
        }

        

        public void killConnectionsWith(string devicesCallSign)
        {
            List<string> logCollector = new List<string>();
            foreach (Link link in linkList)
            {
                if (link.getInterface(0).getDeviceCallSign().Equals(devicesCallSign))
                {
                    link.getInterface(0).setStatus(false);
                    if(link.getStatus() == true)
                    {
                        link.setStatus(false);
                        logCollector.Add(link.getInterface(1).getDeviceCallSign());
                        logCollector.Add(devicesCallSign);
                    }
                    
                    
                }
                else if (link.getInterface(1).getDeviceCallSign().Equals(devicesCallSign))
                {
                    link.getInterface(1).setStatus(false);
                    if (link.getStatus() == true)
                    {
                        link.setStatus(false);
                        logCollector.Add(link.getInterface(0).getDeviceCallSign());
                        logCollector.Add(devicesCallSign);
                    }
                }
            }
            LogWriter.connectionsDown(logCollector, true);
        }
        public void reviveConnectionsWith(string devicesCallSign)
        {
            List<string> logCollector = new List<string>();
            foreach (Link link in linkList)
            {                
                if (link.getInterface(0).getDeviceCallSign().Equals(devicesCallSign))
                {
                    link.getInterface(0).setStatus(true);
                    if (link.getInterface(1).getStatus() == true)
                    {
                        link.setStatus(true);
                        logCollector.Add(link.getInterface(1).getDeviceCallSign());
                        logCollector.Add(devicesCallSign);
                    }
                        
                }
                else if(link.getInterface(1).getDeviceCallSign().Equals(devicesCallSign))
                {
                    link.getInterface(1).setStatus(true);
                    if(link.getInterface(0).getStatus() == true)
                    {
                        link.setStatus(true);
                        logCollector.Add(link.getInterface(0).getDeviceCallSign());
                        logCollector.Add(devicesCallSign);
                    }
                        
                }
            }
            LogWriter.connectionsUp(logCollector, true);
        }

        public bool controlSpecificLink(Interface from, Interface to, bool decision)
        {

            foreach(Link link in linkList)
            {
                if (link.getInterface(1).getDeviceCallSign().Equals(from.getDeviceCallSign())
                    && link.getInterface(0).getDeviceCallSign().Equals(to.getDeviceCallSign())
                    && link.getInterface(1).getGate().Equals(from.getGate())
                    && link.getInterface(0).getGate().Equals(to.getGate())
                    && link.getStatus() != decision)
                {
                    link.setStatus(decision);
                    List<string> logCollector = new List<string>
                    {
                        from.getDeviceCallSign(),
                        to.getDeviceCallSign()
                    };
                    if (decision)
                        LogWriter.connectionsUp(logCollector, true);
                    else
                        LogWriter.connectionsDown(logCollector, true);
                    return true;
                }
                else if (link.getInterface(0).getDeviceCallSign().Equals(from.getDeviceCallSign())
                    && link.getInterface(1).getDeviceCallSign().Equals(to.getDeviceCallSign())
                    && link.getInterface(0).getGate().Equals(from.getGate())
                    && link.getInterface(1).getGate().Equals(to.getGate())
                    && link.getStatus() != decision)
                {
                    link.setStatus(decision);
                    List<string> logCollector = new List<string>
                    {
                        from.getDeviceCallSign(),
                        to.getDeviceCallSign()
                    };
                    if (decision)
                        LogWriter.connectionsUp(logCollector, true);
                    else
                        LogWriter.connectionsDown(logCollector, true);
                    return true;
                }
            }
            return false;
        }

        public bool controlSpecificLink(string from, string to, bool decision)
        {

            foreach (Link link in linkList)
            {
                if (link.getInterface(1).getDeviceCallSign().Equals(from)
                    && link.getInterface(0).getDeviceCallSign().Equals(to)
                    && link.getStatus() != decision)
                {
                    link.setStatus(decision);
                    List<string> logCollector = new List<string>
                    {
                        from,
                        to
                    };
                    if (decision)
                        LogWriter.connectionsUp(logCollector, true);
                    else
                        LogWriter.connectionsDown(logCollector, true);
                    return true;

                }
                else if (link.getInterface(0).getDeviceCallSign().Equals(from)
                    && link.getInterface(1).getDeviceCallSign().Equals(to)
                    && link.getStatus() != decision)
                {
                    link.setStatus(decision);
                    List<string> logCollector = new List<string>
                    {
                        from,
                        to
                    };
                    if (decision)
                        LogWriter.connectionsUp(logCollector, true);
                    else
                        LogWriter.connectionsDown(logCollector, true);
                    return true;
                }
            }
            return false;
        }
    }
}
