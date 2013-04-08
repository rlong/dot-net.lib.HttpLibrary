// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;
using jsonbroker.library.common.http;
using jsonbroker.library.common.broker;
using jsonbroker.library.client.http;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.json;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.client.broker
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

            BrokerMessage answer = Serializer.deserialize(_responseData);

            if (Log.isDebugEnabled())
            {
                log.debug(answer.ToJsonArray().ToString(), "answer.ToJsonArray().ToString()");
            }

            if (BrokerMessageType.FAULT == answer.getMessageType())
            {
                JsonObject associativeParamaters = answer.getAssociativeParamaters();
                BaseException e = FaultSerializer.toBaseException(associativeParamaters);
                throw e;
            }


            return answer;
        }
    }
}
