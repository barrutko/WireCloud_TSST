using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireCloud
{
    class Interface
    {
        string deviceCallSign;
        byte gate;
        bool status;

        public byte getGate()
        {
            return gate;
        }

        public string getDeviceCallSign ()
        {
            return deviceCallSign;
        }

        public Interface (string deviceCallSign, byte gate)
        {            
            this.deviceCallSign = deviceCallSign;            
            this.gate = gate;
            status = true;
        }

        public void setStatus(bool status)
        {
            this.status = status;
        }


        public bool getStatus()
        {
            return status;
        }
    }
}
