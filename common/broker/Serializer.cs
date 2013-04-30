// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//


using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.json.handlers;
using jsonbroker.library.common.json;
using jsonbroker.library.common.json.input;
using jsonbroker.library.common.json.output;
using jsonbroker.library.common.auxiliary;
using jsonbroker.library.common.log;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.common.broker
{
    public class Serializer
    {

        private static readonly Log log = Log.getLog(typeof(FaultSerializer));

        private static readonly JsonArrayHandler _jsonArrayHandler = JsonArrayHandler.getInstance();

        public static BrokerMessage deserialize(Data data)
        {
            JsonDataInput jsonInput = new JsonDataInput(data);

            JsonInputHelper.scanToNextToken(jsonInput);
            

            JsonArray messageComponents;
            try
            {
                messageComponents = _jsonArrayHandler.readJSONArray(jsonInput);
            }
            catch (BaseException exception)
            {
                exception.addContext("Serializer.dataOffset", jsonInput.Cursor);
                throw exception;
            }
            

            BrokerMessage answer = new BrokerMessage(messageComponents);

            return answer;

        }

        public static Data Serialize(BrokerMessage message)
        {
            JsonStringOutput writer = new JsonStringOutput();

            JsonArray messageComponents = message.ToJsonArray();
            _jsonArrayHandler.WriteValue(messageComponents, writer);

            String json = writer.ToString();

            byte[] jsonBytes = StringHelper.ToUtfBytes(json);

            Data answer = new Data(jsonBytes);
            return answer;

        }


    }
}
