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
    class JsonNullHandler : JsonHandler
    {
        private static readonly JsonNullHandler _instance = new JsonNullHandler();

        public static JsonNullHandler getInstance()
        {
            return _instance;
        }



	    ////////////////////////////////////////////////////////////////////////////

        private JsonNullHandler() 
        {
	    }

        ////////////////////////////////////////////////////////////////////////////



        public static Object readNull(JsonInput input)
        {
            //'n' is the current character
            input.nextByte(); // 'u' is the current character
            input.nextByte(); // 'l' is the current character
            input.nextByte(); // 'l' is the current character
            input.nextByte(); // after null

            return null;
        }

        public override Object readValue(JsonInput input)
        {
            return JsonNullHandler.readNull(input);
        }


        public static void WriteNull(JsonOutput writer)
        {
            writer.append("null");
        }

        public override void WriteValue(Object value, JsonOutput writer)
        {
            WriteNull(writer);
        }

    }
}
