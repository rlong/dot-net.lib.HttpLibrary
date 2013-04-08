// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.http.headers
{
    public class AuthenticationHeaderScanner
    {

        private static Log log = Log.getLog(typeof(AuthenticationHeaderScanner));


        ///////////////////////////////////////////////////////////////////////
        String _authenticationHeader;


        ///////////////////////////////////////////////////////////////////////
        int _cursor;

        protected int cursor
        {
            get { return _cursor; }
            set { _cursor = value; }
        }


        ///////////////////////////////////////////////////////////////////////


        public AuthenticationHeaderScanner(String authenticationHeader)
        {
            _authenticationHeader = authenticationHeader;

            _cursor = 0;

        }

        // equivalent to [NSScanner scanString:]
        // public for test purposes
        // will skip over any trailing spaces 
        public void scanString(String expectedValue)
        {

            // expectedValue too long ?
            if( _cursor + expectedValue.Length > _authenticationHeader.Length ) {
                return;
            }

            int newCursorPosition = _cursor;

            for( int i = 0, count = expectedValue.Length; i < count; i++, newCursorPosition++ ) {

                // mismatch ? 
                if( expectedValue[i] != _authenticationHeader[newCursorPosition] ) {

                    // return without updating _cursor
                    return;
                }
            }

            _cursor = newCursorPosition;

            // skip past space characters 
            while (_cursor < _authenticationHeader.Length && ' ' == _authenticationHeader[_cursor])
            {
                _cursor++;
            }

        }

        // equivalent to [NSScanner scanUpToString:]
        // public for test purposes 
        // can return null if expectedValue is not found 
        public String scanUpToString(String target)
        {
            String answer = null;

            int indexOfTarget = _authenticationHeader.IndexOf(target, _cursor);
            if (-1 == indexOfTarget)
            {

                answer = _authenticationHeader.Substring(_cursor);
                _cursor = _authenticationHeader.Length;

                return answer;
            }
            int length = indexOfTarget - _cursor;
            answer = _authenticationHeader.Substring(_cursor, length);

            _cursor += length;

            return answer;

        }


        // equivalent to [NSScanner isAtEnd]
        // public for test purposes 
        public Boolean isAtEnd()
        {
            if (_cursor >= _authenticationHeader.Length)
            {
                return true;
            }
            return false;
        }

        public void scanPastDigestString()
        {
            scanString("Digest");
        }

        public String scanName()
        {
            if (isAtEnd())
            {
                return null;
            }

            String answer = scanUpToString("=");

            if (null != answer)
            {
                scanString("=");
            }

            return answer;

        }


        public String scanQuotedValue()
        {
            scanString("\""); // dispose of the opening '"'

            String answer = scanUpToString("\"");

            scanString("\""); // dispose of the closing '"'

            scanString(","); // discard any trailing ',' that *might* exist

            return answer;

        }

        private int convertHexChar(char c)
        {

            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }
            if (c >= 'a' && c <= 'f')
            {
                return (c - 'a') + 10;
            }
            if (c >= 'A' && c <= 'F')
            {
                return (c - 'A')+10;
            }

            //log.warn(c, "c");

            return -1;
        }

        // equivalent to [NSScanner scanHexLongLong:]
        private ulong scanHexULong()
        {

            if (isAtEnd()) {
                return 0;
            }

            ulong answer = 0;

            while( _cursor < _authenticationHeader.Length ) 
            {
                char c = _authenticationHeader[_cursor];
                int numericValue = convertHexChar(c);

                // not a hex digit ?
                if (-1 == numericValue) 
                {
                    return answer; // ... leave _cursor pointing at the non-numeric character
                }
                answer <<= 4; // shift left by one nibble
                answer |= (uint)numericValue & 0xF;

                _cursor++;

            }

            return answer;
        }

        public uint scanHexUInt32()
        {

            ulong answer = scanHexULong();

            if (answer > 0xFFFFFFFF) // overflow
            {
                log.warn("answer > 0xFFFFFFFF");

                return 0;
            }

            scanString(","); // discard any trailing ',' that *might* exist

            return (uint)answer;

        }

        public String scanValue()
        {
            String answer = null;

            answer = scanUpToString(",");

            scanString(","); // discard any trailing ',' that *might* exist

            return answer;

        }
    }
}
