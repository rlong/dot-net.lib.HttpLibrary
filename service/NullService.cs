// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;
using jsonbroker.library.common.broker;
using jsonbroker.library.server.broker;

namespace jsonbroker.library.service
{
    public class NullService : DescribedService
    {
        private static readonly Log log = Log.getLog(typeof(NullService));

        private ServiceDescription _serviceDescription;

        public NullService(String serviceName)
        {
            _serviceDescription = new ServiceDescription(serviceName);
        }



        public BrokerMessage process(BrokerMessage request)
        {

            String warning = String.Format("unimplemented; serviceName = '{0}'; methodName = '{1}'", request.getServiceName(), request.getMethodName());

            log.warn(warning);

            return BrokerMessage.buildResponse(request);

        }

        public ServiceDescription getServiceDescription()
        {
            return _serviceDescription;
        }



    }
}
