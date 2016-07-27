// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.Http;
using dotnet.lib.Http.json_broker;
using dotnet.lib.Http.client;
using dotnet.lib.CoreAnnex.exception;
using dotnet.lib.CoreAnnex.json.output;
using dotnet.lib.CoreAnnex.auxiliary;
using dotnet.lib.CoreAnnex.json;


namespace dotnet.lib.Http.json_broker.client
{
    public class BrokerMessageResponseHandler : HttpResponseHandler
    {
        private static Log log = Log.getLog(typeof(BrokerMessageResponseHandler));

        /////////////////////////////////////////////////////////
        // responseData
        private Data _responseData;

        protected Data ResponseData
        {
            get { return _responseData; }
            set { _responseData = value; }
        }


        public BrokerMessageResponseHandler()
        {

        }

        private static Data toData(Entity responseEntity)
        {
            if (responseEntity is DataEntity)
            {
                DataEntity dataEntity = (DataEntity)responseEntity;
                return dataEntity.Data;
            }

            Data answer = new Data(responseEntity.getContent(), (int)responseEntity.getContentLength());

            return answer;
        }

        public void handleResponseEntity(System.Net.WebHeaderCollection headers, Entity responseEntity)
        {

            _responseData = toData(responseEntity);

        }

        public BrokerMessage getResponse()
        {

            log.debug(_responseData, "_responseData");
            BrokerMessage answer = Serializer.deserialize(_responseData);

            if (Log.isDebugEnabled())
            {
                log.debug(answer.ToJsonArray().ToString(), "answer.ToJsonArray().ToString()");
            }

            if (BrokerMessageType.FAULT == answer.getMessageType())
            {
                JsonObject associativeParamaters = answer.GetAssociativeParamaters();
                BaseException e = FaultSerializer.toBaseException(associativeParamaters);
                throw e;
            }


            return answer;
        }
    }
}
