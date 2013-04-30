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


        ////////////////////////////////////////////////////////////////////////////
        //
        private MutableDataPool _mutableDataPool;

        public MutableDataPool GetMutableDataPool()
        {
            if (null == _mutableDataPool)
            {
                _mutableDataPool = new MutableDataPool();
            }
            return _mutableDataPool;
        }


        ////////////////////////////////////////////////////////////////////////////
        //

        public JsonDataInput(Data data) {
            _data = data;
            _cursor = 0;

        }

        public bool hasNextByte()
        {
            if (1 + _cursor >= _data.getCount())
            {
                return false;
            }

            return true;
        }

        public byte nextByte()
        {
            _cursor++;
            return _data.getByte(_cursor);
        }


        public byte currentByte()
        {
            return _data.getByte(_cursor);
        }

    }
}
