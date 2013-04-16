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
    public class NetworkAddress_Generated
    {
        /////////////////////////////////////////////////////////
        // port
        private int _port;

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        //////////////////////////////////////////////////////
        public NetworkAddress_Generated()
        {
        }

        public NetworkAddress_Generated(JsonObject values)
        {
            _port = values.GetInt("port");
        }

        public JsonObject toJsonObject()
        {
            JsonObject answer = new JsonObject();
            answer.put("port", _port);
            return answer;
        }

    }
}
