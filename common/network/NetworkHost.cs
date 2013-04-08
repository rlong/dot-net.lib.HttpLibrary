// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.network
{
    public class NetworkHost
    {

        /////////////////////////////////////////////////////////
        // hostName
        private HostName _hostName;

        public HostName HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        /////////////////////////////////////////////////////////
        // ipAddress
        private String _ipAddress;

        public String IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        /////////////////////////////////////////////////////////
        public NetworkHost(String ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public String getHostAddress()
        {
            return _ipAddress;
        }

    }
}
