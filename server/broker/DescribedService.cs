// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.broker;

namespace jsonbroker.library.server.broker
{
    public interface DescribedService : Service
    {

        ServiceDescription getServiceDescription();

    }
}
