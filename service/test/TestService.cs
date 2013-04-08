// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.server.broker;
using jsonbroker.library.common.log;
using jsonbroker.library.common.broker;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.json;

namespace jsonbroker.library.service.test
{
    public class TestService : DescribedService
    {
        
        private static readonly Log log = Log.getLog(typeof(TestService));

        public static readonly String SERVICE_NAME = "jsonbroker.TestService";
        public static readonly ServiceDescription SERVICE_DESCRIPTION = new ServiceDescription(SERVICE_NAME);




        public JsonObject echo(JsonObject associativeParamaters)
        {

            log.enteredMethod();
            return associativeParamaters;
        }

        public void ping()
        {
            log.enteredMethod();            
        }

        private String ToString(Object o)
        {
            return o.ToString();
        }

        public void raise_error()
        {

            log.enteredMethod();
            BaseException e = new BaseException(this, "TestService.raise_error() called");
            e.ErrorDomain = "jsonbroker.TestService.RAISE_ERROR";
            throw e;            
        }



        public BrokerMessage process(BrokerMessage request)
        {
            String methodName = request.getMethodName();


            if ("echo".Equals(methodName))
            {

                JsonObject associativeParamaters = request.getAssociativeParamaters();

                associativeParamaters = this.echo(associativeParamaters);

                BrokerMessage answer = BrokerMessage.buildResponse(request);
                answer.setAssociativeParamaters(associativeParamaters);
                return answer;
            }

            if ("ping".Equals(methodName))
            {
                this.ping();

                BrokerMessage response = BrokerMessage.buildResponse(request);
                return response;
            }

            if ("raiseError".Equals(methodName))
            {
                this.raise_error();

                BrokerMessage response = BrokerMessage.buildResponse(request);
                return response;
            }

            throw ServiceHelper.methodNotFound(this, request);

        }


        public ServiceDescription getServiceDescription()
        {
            return SERVICE_DESCRIPTION;
        }


    }
}
