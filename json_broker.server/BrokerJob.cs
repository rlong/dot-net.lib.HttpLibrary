// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.Http.json_broker;
using dotnet.lib.CoreAnnex.work;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.CoreAnnex.auxiliary;


namespace dotnet.lib.Http.json_broker.server
{
    public class BrokerJob : Job
    {


        private static readonly Log log = Log.getLog(typeof(BrokerJob));

        /////////////////////////////////////////////////////////
        private Data _jsonRequestData;

        /////////////////////////////////////////////////////////
        private String _jsonRequestString;

        /////////////////////////////////////////////////////////
        // callbackAdapter
        private JavascriptCallbackAdapter _callbackAdapter;

        public JavascriptCallbackAdapter CallbackAdapter
        {
            get { return _callbackAdapter; }
        }

        /////////////////////////////////////////////////////////
        // service
        Service _service;


        /////////////////////////////////////////////////////////
        public BrokerJob(String jsonRequest, JavascriptCallbackAdapter callbackAdapter, Service servicesRegistery)
        {
            _jsonRequestString = jsonRequest;
            _callbackAdapter = callbackAdapter;
            _service = servicesRegistery;
        }

        public BrokerJob(Data jsonRequest, JavascriptCallbackAdapter callbackAdapter, Service servicesRegistery)
        {
            _jsonRequestData = jsonRequest;
            _callbackAdapter = callbackAdapter;
            _service = servicesRegistery;
        }


        private Data getJsonRequest()
        {

            if (null != _jsonRequestData)
            {
                return _jsonRequestData;
            }

            String jsonString = _jsonRequestString;

            byte[] bytes = StringHelper.ToUtfBytes(jsonString);

            _jsonRequestData = new Data(bytes);

            return _jsonRequestData;

        }

        public void execute()
        {
            BrokerMessage request = null;

            try
            {
                request = Serializer.deserialize(getJsonRequest());
            }
            catch (Exception e)
            {
                log.warn(e);
                return;
            }

            try
            {                
                BrokerMessage response = _service.process(request);

                if (BrokerMessageType.ONEWAY == request.getMessageType())
                {
                    // no reply 
                }
                else
                {
                    _callbackAdapter.onResponse(request, response);
                }
            }
            catch (Exception t)
            {
                _callbackAdapter.onFault(request, t);
            }
        }

    }
}
