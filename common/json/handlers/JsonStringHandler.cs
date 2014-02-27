// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.json.output;
using jsonbroker.library.common.json.input;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.common.json.handlers
{
    class JsonStringHandler : JsonHandler
    {

        public static readonly JsonStringHandler INSTANCE = new JsonStringHandler();


	    ////////////////////////////////////////////////////////////////////////////

        private JsonStringHandler() 
        {
	    }

	    ////////////////////////////////////////////////////////////////////////////

        public static String readString(JsonInput input)
        {
            MutableData data = input.GetMutableDataPool().reserveMutableData();
            try
            {
                byte b;
                while (true)
                {
                    b = input.nextByte();

                    if ('"' == b)
                    {
                        String answer = data.getUtf8String(0, data.getCount());

                        input.nextByte(); // move past the '"'

                        return answer;
                    }

                    if ('\\' != b)
                    {
                        data.Append(b);
                        continue;
                    }

                    b = input.nextByte();

                    if ('"' == b || '\\' == b || '/' == b )
                    {
                        data.Append(b);
                        continue;
                    }

                    if ('n' == b)
                    {
                        data.Append((byte)'\n');
                        continue;
                    }

                    if ('t' == b)
                    {
                        data.Append((byte)'\t');
                        continue;
                    }
                    if ('r' == b)
                    {
                        data.Append((byte)'\r');
                        continue;
                    }

                }

            }
            finally
            {
                input.GetMutableDataPool().releaseMutableData(data);
            }
        }

        public override Object readValue(JsonInput reader)
        {
            return JsonStringHandler.readString(reader);
        }

        public static void WriteString( String value, JsonOutput writer) {

            writer.append('"');

            String str = (String)value;
            for (int i = 0, count = str.Length; i < count; i++)
            {

                char c = str[i];

                if ('"' == c)
                {
                    writer.append("\\\"");
                    continue;
                }

                if ('\\' == c)
                {
                    writer.append("\\\\");
                    continue;
                }

                if ('/' == c)
                {
                    writer.append("\\/");
                    continue;
                }


                if ('\n' == c)
                {
                    writer.append("\\n");
                    continue;
                }

                if ('\r' == c)
                {
                    writer.append("\\r");
                    continue;
                }

                if ('\t' == c)
                {

                    writer.append("\\t");
                    continue;
                }

                writer.append(c);

            }

            writer.append('"');

        }

        public override void WriteValue(Object value, JsonOutput writer)
        {
            String str = (String)value;
            WriteString(str, writer);

        }



    }
}
