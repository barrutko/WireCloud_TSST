using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireCloud
{
    class Link
    {
        Interface[] connectedInterfaces;
        bool status;

        public Link(string callSignL, byte gateL, string callSignR, byte gateR)
        {
            connectedInterfaces = new Interface[2];
            connectedInterfaces[0] = new Interface(callSignL, gateL);
            connectedInterfaces[1] = new Interface(callSignR, gateR);
            status = true;
        }
        public Interface getInterface(int index)
        {
            return connectedInterfaces[index];
        }

        public Interface getConnection(Interface beginning)
        {
            if (status == false)
                return null;

            if (beginning.getDeviceCallSign().Equals(connectedInterfaces[0].getDeviceCallSign()) && (beginning.getGate()).Equals(connectedInterfaces[0].getGate()))
                return connectedInterfaces[1];
            else if (beginning.getDeviceCallSign().Equals(connectedInterfaces[1].getDeviceCallSign()) && (beginning.getGate()).Equals(connectedInterfaces[1].getGate()))
                return connectedInterfaces[0];
            else
                return null;

        }

        public bool getStatus()
        {
            if (status)
                return true;
            else
                return false;
        }

        public void setStatus(bool status)
        {
            this.status = status;
        }
        

    }
}
