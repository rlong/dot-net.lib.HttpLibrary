// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet.lib.Http.multi_part
{
    internal class DelimiterFound : DelimiterIndicator
    {


        ///////////////////////////////////////////////////////////////////////
        // startOfDelimiter
        private int _startOfDelimiter;

        public int StartOfDelimiter
        {
            get { return _startOfDelimiter; }
            protected set { _startOfDelimiter = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // endOfDelimiter
        private int _endOfDelimiter;

        public int EndOfDelimiter
        {
            get { return _endOfDelimiter; }
            protected set { _endOfDelimiter = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // isCloseDelimiter
        private bool _isCloseDelimiter;

        public bool IsCloseDelimiter
        {
            get { return _isCloseDelimiter; }
            protected set { _isCloseDelimiter = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // completesPartialMatch
        private bool _completesPartialMatch;

        public bool CompletesPartialMatch
        {
            get { return _completesPartialMatch; }
            protected set { _completesPartialMatch = value; }
        }

        public DelimiterFound(int startOfDelimiter, int endOfDelimiter, bool isCloseDelimiter, bool completesPartialMatch)
        {
            _startOfDelimiter = startOfDelimiter;
            _endOfDelimiter = endOfDelimiter;
            _isCloseDelimiter = isCloseDelimiter;
            _completesPartialMatch = completesPartialMatch;
        }


    }
}
