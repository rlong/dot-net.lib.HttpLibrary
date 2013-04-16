// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;
using jsonbroker.library.common.broker;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.json;
using jsonbroker.library.common.json.handlers;
using jsonbroker.library.common.json.output;

namespace jsonbroker.library.server.broker
{
    public class JavascriptCallbackAdapterHelper
    {

        private static Log log = Log.getLog(typeof(JavascriptCallbackAdapterHelper));


        public static String buildJavascriptFault(BrokerMessage request, Exception fault)
        {

            log.enteredMethod();

            JsonOutput jsonWriter = new JsonStringOutput();
            jsonWriter.append("jsonbroker.forwardFault(\"fault\",");

            JsonObjectHandler jsonObjectHandler = JsonObjectHandler.getInstance();
            jsonObjectHandler.WriteValue(request.getMetaData(), jsonWriter);
            jsonWriter.append(",\"");
            jsonWriter.append(request.getServiceName());
            jsonWriter.append("\",1,0,\"");
            jsonWriter.append(request.getMethodName());
            jsonWriter.append("\",");
            jsonObjectHandler.WriteValue(FaultSerializer.ToJsonObject(fault), jsonWriter);
            jsonWriter.append(");");

            String answer = jsonWriter.ToString();

            log.debug(answer, "answer");

            return answer;

        }

        public static String buildJavascriptResponse(BrokerMessage response)
        {


            log.enteredMethod();


            JsonStringOutput jsonWriter = new JsonStringOutput();
            jsonWriter.append("jsonbroker.forwardResponse(\"response\",");
            JsonObjectHandler jsonObjectHandler = JsonObjectHandler.getInstance();
            jsonObjectHandler.WriteValue(response.getMetaData(), jsonWriter);
            jsonWriter.append(",\"");
            jsonWriter.append(response.getServiceName());
            jsonWriter.append("\",1,0,\"");
            jsonWriter.append(response.getMethodName());
            jsonWriter.append("\",");
            jsonObjectHandler.WriteValue(response.GetAssociativeParamaters(), jsonWriter);
            JsonArray parameters = response.GetOrderedParamaters();

            for (int i = 0, count = parameters.Count(); i < count; i++)
            {
                jsonWriter.append(',');
                Object blob = parameters.GetObject(i);
                JsonHandler handler = JsonHandler.getHandler(blob);
                handler.WriteValue(blob, jsonWriter);
            }

            jsonWriter.append(");");

            String answer = jsonWriter.ToString();

            log.debug(answer, "answer");

            return answer;

        }

    }
}
