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
    class JsonNumberHandler : JsonHandler
    {
        private static readonly JsonNumberHandler _instance = new JsonNumberHandler();

        public static JsonNumberHandler getInstance()
        {
            return _instance;
        }


	    ////////////////////////////////////////////////////////////////////////////

        private JsonNumberHandler() 
        {
	    }

        ////////////////////////////////////////////////////////////////////////////


        public static Object readNumber(JsonInput input)
        {
            MutableData data = input.GetMutableDataPool().reserveMutableData();

            try
            {

                bool isFloat = false;

                byte b = input.currentByte();

                do
                {

                    if ('0' <= b && b <= '9')
                    {

                        data.Append(b);
                        b = input.nextByte();
                        continue;
                    }

                    if ('.' == b || 'e' == b || 'E' == b)
                    {
                        isFloat = true;
                        data.Append(b);
                        b = input.nextByte();
                        continue;
                    }

                    if ('+' == b || '-' == b)
                    {
                        data.Append(b);
                        b = input.nextByte();
                        continue;
                    }

                    // fallen through ... none of the chars above matched
                    String stringValue = data.getUtf8String(0, data.getCount());

                    if (isFloat)
                    {
                        return NumericUtilities.parseFloat(stringValue);
                    }
                    else
                    {
                        long longValue = NumericUtilities.parseLong(stringValue);
                        if (longValue > int.MaxValue || longValue < int.MinValue)
                        {
                            return longValue;
                        }
                        return (int)longValue;
                    }


                } while (true);
            }
            finally
            {
                input.GetMutableDataPool().releaseMutableData(data);
            }

        }

        public override Object readValue(JsonInput input)
        {

            return JsonNumberHandler.readNumber(input);
        }

        public static void WriteNumber(Object value, JsonOutput writer)
        {
            writer.append(value.ToString());
        }

        public override void WriteValue(Object value, JsonOutput writer)
        {
            WriteNumber(value, writer);
        }


    }
}
