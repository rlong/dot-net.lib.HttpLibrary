// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.json.output;
using jsonbroker.library.common.json.input;

namespace jsonbroker.library.common.json.handlers
{
    public class JsonObjectHandler : JsonHandler
    {

        private static readonly JsonObjectHandler _instance = new JsonObjectHandler();

        public static JsonObjectHandler getInstance()
        {
            return _instance;
        }


        ////////////////////////////////////////////////////////////////////////////

        private JsonObjectHandler() 
        {
	    }

	    ////////////////////////////////////////////////////////////////////////////


        public static JsonObject readJsonObject(JsonInput input)
        {
            JsonObject answer = new JsonObject();

            input.nextByte(); // move past the '{'

            byte b = JsonInputHelper.scanToNextToken( input );

            while ('}' != b)
            {

                String key = JsonStringHandler.readString(input);

                b = JsonInputHelper.scanToNextToken(input);

                JsonHandler valueHandler = JsonHandler.getHandler(b);
                Object value = valueHandler.readValue(input);

                answer.put(key, value);

                b = JsonInputHelper.scanToNextToken(input);
            }

            // move past the '}' if we can
            if (input.hasNextByte())
            {
                input.nextByte();
            }
            return answer;

        }


        public override Object readValue(JsonInput input)
        {
            return JsonObjectHandler.readJsonObject(input);
        }

        public override void WriteValue(Object value, JsonOutput writer)
        {
            JsonObject jsonObject = (JsonObject)value;

            writer.append('{');


            bool addComma = false;

            for (Dictionary<String, Object>.Enumerator i = jsonObject.GetEnumerator(); i.MoveNext(); )
            {

                if (addComma)
                {
                    writer.append(',');
                }
                addComma = true;

                KeyValuePair<String, Object> entry = i.Current;

                JsonStringHandler.getInstance().WriteValue(entry.Key, writer);

                writer.append(':');

                JsonHandler valueHander = JsonHandler.getHandler(entry.Value);

                valueHander.WriteValue(entry.Value, writer);

            }

            writer.append('}');

        }

    }
}
