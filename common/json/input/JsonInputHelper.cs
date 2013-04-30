// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.json.input
{
    public class JsonInputHelper
    {

        private static bool doesByteBeginToken(byte candidateByte)
        {
            if (' ' == candidateByte)
            {
                return false;
            }

            if ('\t' == candidateByte)
            {
                return false;
            }

            if ('\n' == candidateByte)
            {
                return false;
            }

            if ('\r' == candidateByte)
            {
                return false;
            }

            if (',' == candidateByte)
            {
                return false;
            }

            if (':' == candidateByte)
            {
                return false;
            }

            return true;
        }

        public static byte scanToNextToken(JsonInput jsonInput)
        {

            //byte currentByte = currentByte(); // C# does not like local var names clashing with method names
            byte current = jsonInput.currentByte();

            if (doesByteBeginToken(current))
            {
                return current;
            }

            do
            {
                current = jsonInput.nextByte();
            } while (!doesByteBeginToken(current));


            return current;

        }

    }
}
