// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.auxiliary
{
    public class BooleanObject
    {

        public static BooleanObject FALSE = new BooleanObject(false);
        public static BooleanObject TRUE = new BooleanObject(true);


        bool _value;


        private BooleanObject(bool value)
        {
            _value = value;
        }


        public bool booleanValue()
        {
            return _value;
        }
    }
}
