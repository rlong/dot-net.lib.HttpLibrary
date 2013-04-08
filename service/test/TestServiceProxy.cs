// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.server.broker;
using jsonbroker.library.common.broker;

namespace jsonbroker.library.service.test
{
    class TestServiceProxy
    {

        ////////////////////////////////////////////////////////////////////////////
        Service _service;


        public TestServiceProxy(Service service)
        {
            _service = service;
        }

        public void ping()
        {
            BrokerMessage request = BrokerMessage.buildRequest(TestService.SERVICE_NAME, "ping");
            _service.process(request);
        }

    }
}
