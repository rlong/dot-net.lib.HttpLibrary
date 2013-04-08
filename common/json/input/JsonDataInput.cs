// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.common.json.input
{
    public class JsonDataInput : JsonInput
    {

        Data _data;

        /////////////////////////////////////////////////////////
        // cursor
        private uint _cursor;

        public uint Cursor
        {
            get { return _cursor; }
            protected set { _cursor = value; }
        }

        public JsonDataInput(Data data) : base(data ) {
            _data = data;
            _cursor = 0;

        }

        public override bool hasNextByte()
        {
            if (1 + _cursor >= _data.getCount())
            {
                return false;
            }

            return true;
        }

        public override byte nextByte()
        {
            _cursor++;
            return _data.getByte(_cursor);
        }


        public override byte currentByte()
        {
            return _data.getByte(_cursor);
        }

    }
}
