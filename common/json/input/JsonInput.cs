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
    public abstract class JsonInput
    {


        ////////////////////////////////////////////////////////////////////////////

        MutableData[] _mutableDataPool;
        int _mutableDataPoolIndex;

        ////////////////////////////////////////////////////////////////////////////

        public JsonInput(Data data)
        {

            _mutableDataPool = null; // just to explicit about our intent
            _mutableDataPoolIndex = 0; // the next free MutableData


        }

        public abstract bool hasNextByte();
        public abstract byte nextByte();
        public abstract byte currentByte();

        private bool doesByteBeginToken(byte candidateByte)
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

        public byte scanToNextToken()
        {

            //byte currentByte = currentByte(); // C# does not like local var names clashing with method names
            byte current = currentByte();

            if (doesByteBeginToken(current))
            {
                return current;
            }

            do
            {
                current = nextByte();
            } while (!doesByteBeginToken(current));


            return current;

        }



        public MutableData reserveMutableData()
        {
            if (null == _mutableDataPool)
            {
                _mutableDataPool = new MutableData[5];
            }

            if (_mutableDataPoolIndex >= _mutableDataPool.Length)
            {
                // revert to disposable MutableData objects
                _mutableDataPoolIndex++;

                return new MutableData();
            }

            if (null == _mutableDataPool[_mutableDataPoolIndex])
            {
                _mutableDataPool[_mutableDataPoolIndex] = new MutableData();
            }

            MutableData answer = _mutableDataPool[_mutableDataPoolIndex];

            _mutableDataPoolIndex++;

            return answer;
        }


        public void releaseMutableData(MutableData freedMutableData)
        {
            if (_mutableDataPoolIndex > _mutableDataPool.Length)
            {
                // release of disposable MutableData objects
                _mutableDataPoolIndex--;
                return;
            }

            _mutableDataPoolIndex--;

            _mutableDataPool[_mutableDataPoolIndex].clear();
            return;
        }

    }
}
