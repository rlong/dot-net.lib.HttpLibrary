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
    public class HostName : HostName_Generated
    {
        public HostName() : base()
        {            
        }

        /////////////////////////////////////////////////////////
        public HostName(JsonObject values)
            : base(values)
        {
        }

        private static bool compare(String s1, String s2)
        {
            if (s1 == null)
            {
                if (s2 == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return s1.Equals(s2);
        }

        public bool isEqualToHostName(HostName other)
        {
            if (this == other)
            {
                return true;
            }

            if (!compare(_applicationName, other._applicationName))
            {
                return false;
            }

            if (!compare(_zeroconfName, other._zeroconfName))
            {
                return false;
            }

            if (!compare(_dnsName, other._dnsName))
            {
                return false;
            }

            return true;

        }

        public override String ToString()
        {

            if (null != _applicationName)
            {
                return _applicationName;
            }

            if (null != _zeroconfName)
            {
                return _zeroconfName;
            }

            if (null != _dnsName)
            {
                return _dnsName;
            }
            return null;
        }




    }
}
