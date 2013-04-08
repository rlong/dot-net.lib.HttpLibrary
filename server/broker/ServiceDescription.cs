// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.server.broker
{
    public class ServiceDescription
    {
        private String _serviceName;

        public String getServiceName()
        {
            return _serviceName;
        }

        public ServiceDescription(String serviceName)
        {
            _serviceName = serviceName;
        }

        public int getMajorVersion()
        {
            return 1;
        }

        public int getMinorVersion()
        {
            return 0;
        }

    }
}
