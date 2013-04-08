// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.auxiliary
{
    public class IntegerObject
    {

        int _value;

        public IntegerObject(int value)
        {
            _value = value;
        }

        public int intValue()
        {
            return _value;
        }

    }
}
