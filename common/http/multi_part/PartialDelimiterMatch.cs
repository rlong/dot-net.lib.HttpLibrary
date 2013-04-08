// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.http.multi_part
{
    public class PartialDelimiterMatched : DelimiterIndicator
    {

        ///////////////////////////////////////////////////////////////////////
        // matchingBytes
        private byte[] _matchingBytes;

        public byte[] MatchingBytes
        {
            get { return _matchingBytes; }
            protected set { _matchingBytes = value; }
        }

        public PartialDelimiterMatched( byte[] matchingBytes)
        {
            _matchingBytes = matchingBytes;
        }

    }
}
