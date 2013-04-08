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
    public class ServiceHelper
    {

        private static readonly int BASE = ErrorCodeUtilities.getBaseErrorCode("jsonbroker.ServiceHelper");

        private static readonly int METHOD_NOT_FOUND = BASE | 0x01;

        public static BaseException methodNotFound(Object originator, BrokerMessage request)
        {            
            String methodName = request.getMethodName();

            BaseException answer = new BaseException(originator, "Unknown methodName; methodName = '{0}'", methodName);
            answer.FaultCode = METHOD_NOT_FOUND;

            return answer;
        }
    }
}
