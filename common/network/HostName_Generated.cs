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
    public class HostName_Generated
    {
        /////////////////////////////////////////////////////////
        // applicationName
        protected String _applicationName;

        public String ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        /////////////////////////////////////////////////////////
        // zeroconfName
        protected String _zeroconfName;

        public String ZeroconfName
        {
            get { return _zeroconfName; }
            set { _zeroconfName = value; }
        }

        /////////////////////////////////////////////////////////
        // dnsName
        protected String _dnsName;

        public String DnsName
        {
            get { return _dnsName; }
            set { _dnsName = value; }
        }

        /////////////////////////////////////////////////////////
        public HostName_Generated()
        {            
        }

        /////////////////////////////////////////////////////////
        public HostName_Generated(JsonObject values)
        {
            _applicationName = values.getString("applicationName", null);
            _zeroconfName = values.getString("zeroconfName", null);
            _dnsName = values.getString("dnsName", null);
        }

        public JsonObject toJsonObject()
        {
            JsonObject answer = new JsonObject();
            answer.put("applicationName", _applicationName);
            answer.put("zeroconfName", _zeroconfName);
            answer.put("dnsName", _dnsName);
            return answer;
        }

    }
}
