// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.json.output
{
    public class JsonStringOutput : JsonOutput
    {
               
        StringBuilder _stringBuilder;

        public JsonStringOutput()
        {
            _stringBuilder = new StringBuilder();
        }

        public void reset()
        {
            _stringBuilder.Length = 0;
        }

        public override void append(char c)
        {
            _stringBuilder.Append(c);
        }

        public override void append(String str)
        {
            _stringBuilder.Append(str);
        }

        public override String ToString()
        {
            return _stringBuilder.ToString();
        }

    }
}
