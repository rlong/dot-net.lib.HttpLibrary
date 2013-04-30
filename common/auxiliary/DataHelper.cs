// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.auxiliary
{
    public class DataHelper
    {

        public static String ToUtf8String(Data data)
        {
            return data.getUtf8String(0, data.Length);
        }
    }
}
