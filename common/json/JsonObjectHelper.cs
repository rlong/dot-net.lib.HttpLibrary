// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.auxiliary;
using jsonbroker.library.common.json.input;

namespace jsonbroker.library.common.json
{
    public class JsonObjectHelper
    {



        public static JsonObject BuildFromString(String jsonString)
        {

            byte[] rawData = StringHelper.ToUtfBytes(jsonString);
            Data data = new Data(rawData);
            JsonInput input = new JsonDataInput(data);

            JsonBuilder builder = new JsonBuilder();
            JsonReader.read(input, builder);
            
            JsonObject answer = builder.getObjectDocument();

            return answer;

        }

    }
}
