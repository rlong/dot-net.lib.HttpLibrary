// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using jsonbroker.library.common.json.output;
using jsonbroker.library.common.auxiliary;
using jsonbroker.library.common.json.input;

namespace jsonbroker.library.common.json
{
    public class JsonArrayHelper
    {

        public static JsonArray FromString(String jsonString)
        {

            byte[] rawData = StringHelper.ToUtfBytes(jsonString);
            Data data = new Data(rawData);
            JsonInput input = new JsonDataInput(data);

            JsonBuilder builder = new JsonBuilder();
            JsonReader.read(input, builder);

            JsonArray answer = builder.getArrayDocument();
            return answer;
        }


        public static byte[] ToBytes(JsonArray jsonArray)
        {
            MemoryStream memoryStream = new MemoryStream();
            write(jsonArray, memoryStream);
            return memoryStream.ToArray();
        }


        public static void write(JsonArray jsonArray, Stream destination)
        {
            JsonStreamOutput jsonStreamOutput = new JsonStreamOutput(destination);
            JsonWriter writer = new JsonWriter(jsonStreamOutput);
            JsonWalker.walk(jsonArray, writer);
            jsonStreamOutput.Flush();
        }


    }
}
