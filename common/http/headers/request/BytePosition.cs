// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.http.headers.request
{
    public class BytePosition
    {

        /////////////////////////////////////////////////////////
        // value
        private long _value;

        public long Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public BytePosition(long value)
        {
            _value = value;
        }

    }
}
