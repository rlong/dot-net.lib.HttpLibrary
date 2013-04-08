// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.json.input;
using jsonbroker.library.common.json.output;

namespace jsonbroker.library.common.json.handlers
{
    public class JsonBooleanHandler : JsonHandler
    {
        
        private static readonly JsonBooleanHandler _instance = new JsonBooleanHandler();


        public static JsonBooleanHandler getInstance()
        {
            return _instance;
        }

	    ////////////////////////////////////////////////////////////////////////////

        private JsonBooleanHandler() 
        {
	    }

	    ////////////////////////////////////////////////////////////////////////////



        public static bool readBoolean(JsonInput input)
        {
            byte currentByte = input.currentByte();

            if ('t' == currentByte || 'T' == currentByte)
            {
                //'t' is the current character
                input.nextByte(); // 'r' is the current character
                input.nextByte(); // 'u' is the current character
                input.nextByte(); // 'e' is the current character
                input.nextByte(); // after true

                return true;
            }
            else
            {
                //'f' is the current character
                input.nextByte(); // 'a' is the current character
                input.nextByte(); // 'l' is the current character
                input.nextByte(); // 's' is the current character
                input.nextByte(); // 'e' is the current character
                input.nextByte(); // after false

                return false;
            }

        }

        public override Object readValue(JsonInput input)
        {
            return JsonBooleanHandler.readBoolean(input);
        }


        public static void WriteBoolean(bool value, JsonOutput output)
        {
            if (value)
            {
                output.append("true");
            }
            else
            {
                output.append("false");
            }
        }

        public override void WriteValue(Object value, JsonOutput output)
        {
            Boolean booleanValue = (Boolean)value;
            WriteBoolean(booleanValue, output);

        }

    }
}
