// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.json;

namespace jsonbroker.library.common.broker
{
    public class BrokerMessage
    {
        ////////////////////////////////////////////////////////////////////////////
        private BrokerMessageType _messageType = BrokerMessageType.REQUEST;

        public BrokerMessageType getMessageType()
        {
            return _messageType;
        }

        public void setMessageType(BrokerMessageType messageType)
        {
            _messageType = messageType;
        }

        ////////////////////////////////////////////////////////////////////////////
        private JsonObject _metaData;


        public JsonObject getMetaData()
        {
            return _metaData;
        }


        ////////////////////////////////////////////////////////////////////////////
        private String _serviceName;

        public String getServiceName()
        {
            return _serviceName;
        }

        public void setServiceName(String service)
        {
            _serviceName = service;
        }


        ///////////////////////////////////////////////////////////////////////
        // methodName;
        private String _methodName;

        public String getMethodName()
        {
            return _methodName;
        }

        public void setMethodName(String methodName)
        {
            _methodName = methodName;
        }

        ///////////////////////////////////////////////////////////////////////
        // associativeParamaters
        private JsonObject _associativeParamaters;


        public JsonObject GetAssociativeParamaters()
        {
            return _associativeParamaters;
        }

        public void SetAssociativeParamaters(JsonObject associativeParamaters)
        {
            _associativeParamaters = associativeParamaters;
        }

        ///////////////////////////////////////////////////////////////////////
        // orderedParamaters
        private JsonArray _orderedParamaters;


        public JsonArray GetOrderedParamaters()
        {
            return _orderedParamaters;
        }

        public void SetOrderedParamaters(JsonArray paramaters)
        {
            _orderedParamaters = paramaters;
        }

        ///////////////////////////////////////////////////////////////////////

        public BrokerMessage()
        {
            _metaData = new JsonObject();
            _associativeParamaters = new JsonObject();
            _orderedParamaters = new JsonArray();
        }


        public BrokerMessage(JsonArray values)
        {

            String messageTypeIdentifer = values.GetString(0);
            _messageType = BrokerMessageType.lookup(messageTypeIdentifer);
            _metaData = values.GetJsonObject(1);
            _serviceName = values.GetString(2);
            // int majorVersion = values.GetInteger(3);
            // int minorVersion = values.GetInteger(4);
            _methodName = values.GetString(5);
            _associativeParamaters = values.GetJsonObject(6);
            if (7 < values.Count())
            {
                _orderedParamaters = values.GetJsonArray(7);
            }
            else
            {
                _orderedParamaters = new JsonArray(0);
            }
        }

        public static BrokerMessage buildRequest(String serviceName, String methodName)
        {
            BrokerMessage answer = new BrokerMessage();
            answer._messageType = BrokerMessageType.REQUEST;
            answer._serviceName = serviceName;
            answer._methodName = methodName;

            return answer;

        }

        public static BrokerMessage buildMetaRequest(String serviceName, String methodName)
        {

            BrokerMessage answer = new BrokerMessage();

            answer._messageType = BrokerMessageType.META_REQUEST;
            answer._serviceName = serviceName;
            answer._methodName = methodName;

            return answer;
        }


        public static BrokerMessage buildFault(BrokerMessage request, Exception e)
        {
            BrokerMessage answer = new BrokerMessage();

            answer._messageType = BrokerMessageType.FAULT;
            answer._metaData = request.getMetaData();
            answer._serviceName = request._serviceName;
            answer._methodName = request._methodName;
            answer._associativeParamaters = FaultSerializer.ToJsonObject(e);
            answer._orderedParamaters = new JsonArray(0);

            return answer;

        }

        public static BrokerMessage buildMetaResponse(BrokerMessage request)
        {

            BrokerMessage answer = new BrokerMessage();

            answer._messageType = BrokerMessageType.META_RESPONSE;
            answer._metaData = request.getMetaData();
            answer._serviceName = request._serviceName;
            answer._methodName = request._methodName;
            answer._associativeParamaters = new JsonObject();
            answer._orderedParamaters = new JsonArray(0);

            return answer;
        }

        public static BrokerMessage buildResponse(BrokerMessage request)
        {
            BrokerMessage answer = new BrokerMessage();

            answer._messageType = BrokerMessageType.RESPONSE;
            answer._metaData = request.getMetaData();
            answer._serviceName = request._serviceName;
            answer._methodName = request._methodName;
            answer._associativeParamaters = new JsonObject();
            answer._orderedParamaters = new JsonArray(0);

            return answer;
        }


        public JsonArray ToJsonArray()
        {

            JsonArray answer = new JsonArray(5);

            answer.Add(_messageType.getIdentifier());
            answer.Add(_metaData);
            answer.Add(_serviceName);
            answer.Add(1);
            answer.Add(0);
            answer.Add(_methodName);
            answer.Add(_associativeParamaters);
            answer.Add(_orderedParamaters);

            return answer;

        }


        public void addParameter(int parameter)
        {
            _orderedParamaters.Add(parameter);
        }

        public void addParameter(JsonObject parameter)
        {
            _orderedParamaters.Add(parameter);
        }


        public void addParameter(JsonArray parameter)
        {
            _orderedParamaters.Add(parameter);
        }

        public void addParameter(Object parameter)
        {
            _orderedParamaters.Add(parameter);
        }

        public void addParameter(String parameter)
        {
            _orderedParamaters.Add(parameter);
        }

        public void setResponseType(String type)
        {
            _metaData.put("responseType", type);
        }

    }
}
