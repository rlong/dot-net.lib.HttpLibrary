// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.common.http.headers
{
    // http://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.6
    public class ParametersScanner
    {

        private static Log log = Log.getLog(typeof(ParametersScanner));

        private static bool[] tokenChars = new bool[128];

        static ParametersScanner()
        {
            for (int i = 0; i < 128; i++)
            {
                tokenChars[i] = true;
            }

            // http://www.w3.org/Protocols/rfc2616/rfc2616-sec2.html#sec2.2
            // CTL chars ... 
            {
                for (int i = 0; i < 32; i++)
                {
                    tokenChars[i] = false;
                }

                tokenChars[127] = false; // 'DEL'
            }

            // http://www.w3.org/Protocols/rfc2616/rfc2616-sec2.html#sec2.2
            // separators ...  
            {
                tokenChars['('] = false;
                tokenChars[')'] = false;
                tokenChars['<'] = false;
                tokenChars['>'] = false;
                tokenChars['@'] = false;

                tokenChars[','] = false;
                tokenChars[';'] = false;
                tokenChars[':'] = false;
                tokenChars['\\'] = false;
                tokenChars['"'] = false;

                tokenChars['/'] = false;
                tokenChars['['] = false;
                tokenChars[']'] = false;
                tokenChars['?'] = false;
                tokenChars['='] = false;

                tokenChars['{'] = false;
                tokenChars['}'] = false;
                tokenChars[' '] = false;
                tokenChars[9] = false;  // HT
            }
        }

        int _offset;
        String _value;

        public ParametersScanner(int offset, String value)
        {

            _offset = offset;
            _value = value;

        }

        // http://www.w3.org/Protocols/rfc2616/rfc2616-sec2.html#sec2.2
        // only covers the octets 0-31 and 127 ... does not include ' ' (space)
        private static bool isTokenCharacter(char c)
        {
            if (c > 128)
            {
                return false;
            }
            return tokenChars[c];
        }



        /// <summary>
        ///
        /// </summary>
        /// <returns>true if another token was found</returns>
        private bool moveToStartOfNextToken(bool quotesIsTokenCharacter)
        {

            int length = _value.Length;

            while (_offset < length && !isTokenCharacter(_value[_offset]))
            {
                if (quotesIsTokenCharacter && '"' == _value[_offset])
                {
                    return true;
                }
                _offset++;
            }

            // run out of string ? 
            if (_offset == length)
            {
                return false;
            }

            return true;

        }

        private void moveToEndOfToken()
        {


            int length = _value.Length;


            while (_offset < length && isTokenCharacter(_value[_offset]))
            {
                _offset++;
            }


        }

        // http://www.w3.org/Protocols/rfc2616/rfc2616-sec2.html#sec2.2
        private String nextToken(bool quotesIsTokenCharacter)
        {

            if (!moveToStartOfNextToken(quotesIsTokenCharacter))
            {
                return null;
            }

            int startOfToken = _offset;
            log.debug(startOfToken, "startOfToken");

            moveToEndOfToken();

            int length = _offset - startOfToken;
            String answer = _value.Substring(startOfToken, length); // Subtring( int start, int length )

            return answer;

        }

        // returns null when there are no more keys
        // http://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.6
        public String NextAttribute()
        {
            if (_offset >= _value.Length)
            {
                return null;
            }

            String answer = nextToken(false);
            return answer;

        }

        private void moveToEndOfQuotedString()
        {
            bool lastCharWasAnEscape = false;

            int length = _value.Length;

            while(_offset < length && (lastCharWasAnEscape || '"' != _value[_offset]) )
            {
                char lastChar = _value[_offset];
                _offset++;

                if (lastCharWasAnEscape)
                {
                    lastCharWasAnEscape = false;
                    continue;
                }

                if ('\\' == lastChar)
                {
                    lastCharWasAnEscape = true;
                    continue;
                }
            }
        }


        public String nextValue()
        {
            if (!moveToStartOfNextToken(true))
            {
                BaseException e = new BaseException( this, "!moveToStartOfNextToken(true); _value = '{0}'", _value );
                throw e;
            }

            bool isQuotedString = false;
            int startOfToken = _offset;

            // http://www.w3.org/Protocols/rfc2616/rfc2616-sec3.html#sec3.6
            if ('"' == _value[_offset])
            {
                isQuotedString = true;
                _offset++; // move past the quotes
                startOfToken = _offset;
                if (_offset >= _value.Length)
                {
                    BaseException e = new BaseException(this, "_offset >= _value.Length; _offset = {0}; _value.Length = {1}; _value = '{2}'", _offset, _value.Length, _value);
                    throw e;
                }
                moveToEndOfQuotedString();
            }
            else
            {
                moveToEndOfToken();
            }

            int length = _offset - startOfToken;
            String answer = _value.Substring(startOfToken, length); // Subtring( int start, int length )

            if (isQuotedString) // move past the closing quotes
            {
                _offset++;
            }

            return answer;

        }
    }
}
