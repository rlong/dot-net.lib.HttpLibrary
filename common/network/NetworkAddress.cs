// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.json;

namespace jsonbroker.library.common.network
{
    public class NetworkAddress : NetworkAddress_Generated
    {

        /////////////////////////////////////////////////////////
        // networkHost
        private NetworkHost _networkHost;

        public NetworkHost NetworkHost
        {
            get { return _networkHost; }
            set { _networkHost = value; }
        }

        /////////////////////////////////////////////////////////
        public NetworkAddress()
            : base()
        {
        }


        public NetworkAddress(String hostAddress, int port)
            : base()
        {
            this.Port = port;
            _networkHost = new NetworkHost(hostAddress);
        }


        public NetworkAddress(JsonObject values)
            : base(values)
        {

        }

        public String getHostAddress()
        {
            return _networkHost.getHostAddress();
        }

    }
}
