// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.json.output
{
    public abstract class JsonOutput
    {

        public abstract void append(char c);
        public abstract void append(String str);
        public override abstract String ToString();

    }
}
