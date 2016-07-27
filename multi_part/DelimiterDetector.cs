// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.CoreAnnex.auxiliary;
using dotnet.lib.CoreAnnex.log;

namespace dotnet.lib.Http.multi_part
{


    /// <summary>
    /// 
    /// as per http://www.w3.org/Protocols/rfc1341/7_2_Multipart.html
    /// </summary>
    internal class DelimiterDetector
    {

        // private static Log log = Log.getLog(typeof(DelimiterDetector));

        ///////////////////////////////////////////////////////////////////////
        // delimiterBytes
        private byte[] _delimiterBytes;


        ///////////////////////////////////////////////////////////////////////
        // closeDelimiterBytes
        private byte[] _closeDelimiterBytes;


        ///////////////////////////////////////////////////////////////////////
        // currentDelimiterBytes
        private byte[] _currentDelimiterBytes;


        int _currentMatchingDelimiterIndex;

        public DelimiterDetector(String boundary)
        {

            _delimiterBytes = StringHelper.ToUtfBytes("\r\n--" + boundary +
    "\r\n");

            _closeDelimiterBytes = StringHelper.ToUtfBytes("\r\n--" + boundary +
    "--\r\n");

            _currentDelimiterBytes = _delimiterBytes;
            _currentMatchingDelimiterIndex = 0;
        }




        private void reset()
        {
            _currentMatchingDelimiterIndex = 0;
            _currentDelimiterBytes = _delimiterBytes;
        }

        internal DelimiterIndicator update(byte[] buffer, int startingOffset, int endingOffset)
        {

            bool partialDelimiterMatched = false;
            if (0 != _currentMatchingDelimiterIndex)
            {
                partialDelimiterMatched = true;
            }

            for (int i = startingOffset; i < endingOffset; i++)
            {
                if (0 == _currentMatchingDelimiterIndex) //  not previously matched (previous byte != `\r`)
                {
                    // vvv function ? 

                    if (buffer[i] == _currentDelimiterBytes[_currentMatchingDelimiterIndex])
                    {
                        _currentMatchingDelimiterIndex = 1;
                    }

                    // ^^^ function ? 

                    continue;

                }


                bool resetRequired = false;
                
                if (4 > _currentMatchingDelimiterIndex) //  only during a `\r\n--` sequence
                {
                    if (buffer[i] == _currentDelimiterBytes[_currentMatchingDelimiterIndex])
                    {
                        _currentMatchingDelimiterIndex++;
                    }
                    else // reset
                    {
                        resetRequired = true;
                    }
                }
                else
                {
                    // determine if we might be looking at the closeDelimiter
                    if (_currentMatchingDelimiterIndex == _delimiterBytes.Length - 2) // at where the terminating `\r` should be on an standard (non-closing) delimiter
                    {
                        
                        if (_currentDelimiterBytes == _delimiterBytes)
                        {
                            if ('-' == buffer[i])
                            {
                                _currentDelimiterBytes = _closeDelimiterBytes;
                            }
                        }
                    }
                    if (buffer[i] == _currentDelimiterBytes[_currentMatchingDelimiterIndex])
                    {
                        _currentMatchingDelimiterIndex++;
                        if (_currentDelimiterBytes.Length == _currentMatchingDelimiterIndex) // reached the end of the delimiter
                        {

                            bool isCloseDelimiter = false;
                            if (_currentDelimiterBytes == _closeDelimiterBytes)
                            {
                                isCloseDelimiter = true;
                            }

                            int startOfDelimiter = i - (_currentDelimiterBytes.Length - 1); // `-1` to given us the index of the first character of the delimiter

                            if (partialDelimiterMatched)
                            {
                                startOfDelimiter = startingOffset;

                            }

                            int endOfDelimiter = i + 1; // after the last character of the delimiter


                            { // creating another answer object outside of the `PartialDelimiterCompleted` above, causes a compiler error
                                DelimiterFound answer = new DelimiterFound(startOfDelimiter, endOfDelimiter, isCloseDelimiter, partialDelimiterMatched);
                                reset(); // reset
                                return answer;
                            }

                        }
                    }
                    else // buffer[i] != _currentDelimiterBytes[_currentMatchingDelimiterIndex] ... reset
                    {
                        resetRequired = true;
                    }
                }


                if (resetRequired)
                {
                    reset();
                    partialDelimiterMatched = false;

                    // vvv function ? 

                    if (buffer[i] == _currentDelimiterBytes[_currentMatchingDelimiterIndex])
                    {
                        _currentMatchingDelimiterIndex = 1;
                    }

                    // ^^^ function ? 

                }

            }

            // finished with the loop but looks like we might have a partial match
            if (0 != _currentMatchingDelimiterIndex)
            {
                byte[] partialBytes = new byte[_currentMatchingDelimiterIndex];

                // vvv http://stackoverflow.com/questions/5099604/c-any-faster-way-of-copying-arrays
                Buffer.BlockCopy(_currentDelimiterBytes, 0, partialBytes, 0, _currentMatchingDelimiterIndex);
                // ^^^ http://stackoverflow.com/questions/5099604/c-any-faster-way-of-copying-arrays

                PartialDelimiterMatched answer = new PartialDelimiterMatched(partialBytes);
                return answer;
            }

            // no `match` partial, or complete
            return null;
        }
    }
}
