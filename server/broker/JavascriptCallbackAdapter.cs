// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.broker;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.server.broker
{
    public interface JavascriptCallbackAdapter
    {

        void onFault(BrokerMessage request, Exception fault);
        void onResponse(BrokerMessage request, BrokerMessage response);

    }
}
