// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.CoreAnnex.exception;
using dotnet.lib.Http.json_broker;
using dotnet.lib.CoreAnnex.json;

namespace dotnet.lib.Http.json_broker.server
{
    public class ServicesRegistery : Service
    {

        // private static readonly int BASE = ErrorCodeUtilities.getBaseErrorCode("jsonbroker.ServicesRegistery");

        public class ErrorDomain
        {
            public static readonly String SERVICE_NOT_FOUND = "jsonbroker.ServicesRegistery.SERVICE_NOT_FOUND";
        }

        private static readonly Log log = Log.getLog(typeof(ServicesRegistery));


        private Dictionary<String, DescribedService> _services;

        ////////////////////////////////////////////////////////////////////////////
        //
        private ServicesRegistery _next;

        public void setNext(ServicesRegistery next)
        {
            _next = next;
        }

        ////////////////////////////////////////////////////////////////////////////
        public ServicesRegistery()
        {
            _services = new Dictionary<String, DescribedService>();
        }

        private DescribedService getService(String serviceName)
        {

            if (_services.ContainsKey(serviceName))
            {
                return _services[serviceName];
            }

            if (null == _next)
            {
                String technicalError = String.Format("null == answer, serviceName = '{0}'", serviceName);
                BaseException e = new BaseException(this, technicalError);
                e.ErrorDomain = ErrorDomain.SERVICE_NOT_FOUND;
                throw e;
            }

            return _next.getService(serviceName);

        }

        public bool containsService(String serviceName)
        {

            if( _services.ContainsKey( serviceName ) )
            {
                return true;
            }
            if (null != _next)
            {
                return _next.containsService(serviceName);
            }
            return false;
        }

        public void addService(DescribedService service ) 
        {
            String serviceName = service.getServiceDescription().getServiceName();
            _services[serviceName] = service;
        }

        public void removeService(DescribedService service)
        {
            String serviceName = service.getServiceDescription().getServiceName();
            _services.Remove(serviceName);
        }

        public void debug()
        {
            foreach (String serviceName in _services.Keys)
            {
                String message = String.Format("{0} -> {1}", serviceName, _services[serviceName].GetType().Name);

                log.debug(message);
            }

        }

        private BrokerMessage processMetaRequest(BrokerMessage request)
        {
            String methodName = request.getMethodName();

            if ("getVersion".Equals(methodName))
            {
                JsonObject associativeParamaters = request.GetAssociativeParamaters();
                String serviceName = request.getServiceName();

                BrokerMessage answer = BrokerMessage.buildMetaResponse(request);
                associativeParamaters = answer.GetAssociativeParamaters();

                if (!_services.ContainsKey(serviceName))
                {
                    associativeParamaters.put("exists", false);
                }
                else
                {
                    associativeParamaters.put("exists", true);

                    DescribedService describedService = _services[serviceName];
                    ServiceDescription serviceDescription = describedService.getServiceDescription();
                    associativeParamaters.put("majorVersion", serviceDescription.getMajorVersion());
                    associativeParamaters.put("minorVersion", serviceDescription.getMinorVersion());

                }

                return answer;

            }

            throw ServiceHelper.methodNotFound(this, request);
        }

        public BrokerMessage process(BrokerMessage request)
        {
            if (BrokerMessageType.META_REQUEST == request.getMessageType())
            {
                return this.processMetaRequest(request);
            }

            Service serviceDelegate = this.getService( request.getServiceName() );
            return serviceDelegate.process(request);
        }
    }
}
