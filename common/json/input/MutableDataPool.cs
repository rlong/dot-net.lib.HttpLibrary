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
    public class MutableDataPool
    {
     
        ////////////////////////////////////////////////////////////////////////////

        MutableData[] _mutableDataPool;
        int _mutableDataPoolIndex;

        ////////////////////////////////////////////////////////////////////////////

        public MutableDataPool()
        {

            _mutableDataPool = null; // just to explicit about our intent
            _mutableDataPoolIndex = 0; // the next free MutableData


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
