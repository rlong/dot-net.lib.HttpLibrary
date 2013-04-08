// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.server.broker;
using jsonbroker.library.common.log;
using jsonbroker.library.client.http;
using jsonbroker.library.common.broker;
using jsonbroker.library.common.http;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.client.broker
{
    public class ServiceHttpProxy : Service
    {
        private static Log log = Log.getLog(typeof(ServiceHttpProxy));

        ////////////////////////////////////////////////////////////////////////////
        HttpDispatcher _httpDispatcher;

        ////////////////////////////////////////////////////////////////////////////
        Authenticator _authenticator = null; // just to be clear about our intent

        public Authenticator Authenticator
        {
            get { return _authenticator; }
            protected set { _authenticator = value; }
        }

        ////////////////////////////////////////////////////////////////////////////    
        BrokerMessageResponseHandler _responseHandler;

        public ServiceHttpProxy(HttpDispatcher httpDispatcher, Authenticator authenticator)
        {
            _httpDispatcher = httpDispatcher;
            _authenticator = authenticator;
            _responseHandler = new BrokerMessageResponseHandler();
        }

        public ServiceHttpProxy(HttpDispatcher httpDispatcher)
        {
            _httpDispatcher = httpDispatcher;
            _responseHandler = new BrokerMessageResponseHandler();
        }

        public BrokerMessage process(BrokerMessage request)
        {
            Data bodyData = Serializer.Serialize(request);
            log.debug(bodyData, "bodyData");

            Entity entity = new DataEntity(bodyData);
            String requestUri;
            if (null == _authenticator)
            {
                requestUri = "/_dynamic_/open/services";
            }
            else
            {
                if (_authenticator.AuthInt)
                {
                    requestUri = "/_dynamic_/auth-int/services";
                }
                else
                {
                    requestUri = "/_dynamic_/auth/services";
                }
            }
            HttpRequestAdapter requestAdapter = new HttpRequestAdapter(requestUri);
            requestAdapter.RequestEntity = entity;

            _httpDispatcher.post(requestAdapter, _authenticator, _responseHandler);
            return _responseHandler.getResponse();
        }

        public String getServiceName()
        {
            return null;
        }

    }
}
