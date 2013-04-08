// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.common.http.headers.request
{




    public class Range
    {
        private static Log log = Log.getLog(typeof(Range));



        /////////////////////////////////////////////////////////
        // firstBytePosition
        private BytePosition _firstBytePosition;

        public BytePosition FirstBytePosition
        {
            get { return _firstBytePosition; }
            set { _firstBytePosition = value; }
        }
        

        /////////////////////////////////////////////////////////
        // lastBytePosition
        private BytePosition _lastBytePosition;

        public BytePosition LastBytePosition
        {
            get { return _lastBytePosition; }
            set { _lastBytePosition = value; }
        }

        ////////////////////////////////////////////////////////////////////////////
        String _toString;

        private Range(String value)
        {
            _toString = value;
        }

        public String toString()
        {
            return _toString;
        }


        public static Range buildFromString(String value)
        {
            Range answer = new Range(value);

            string[] tokens = value.Split(new char[] { '=', ',' });

            if (2 != tokens.Length)
            {
                String technicalError = String.Format("2 != tokens.Length; only support simple range values; value = '{0}'", value);
                throw new BaseException( typeof(Range), technicalError);
            }

            String bytes = tokens[0];
            if (!"bytes".Equals(bytes))
            {
                String technicalError = String.Format("!\"bytes\".Equals(bytes); bytes = '{0}'; value = '{1}'", bytes, value );
                throw new BaseException(typeof(Range), technicalError);
            }

            String range = tokens[1];

            log.debug(range, "range");

            int indexOfHyphen = range.IndexOf('-');
            if (-1 == indexOfHyphen)
            {
                String technicalError = String.Format("-1 == indexOfHyphen; range = '{0}'; value = '{1}'", range, value);
                throw new BaseException(typeof(Range), technicalError);
            }

            int secondHyphen = range.IndexOf('-', indexOfHyphen + 1);
            if (-1 != secondHyphen)
            {
                String technicalError = String.Format("-1 != secondHyphen; range = '{0}'; value = '{1}'", range, value);
                throw new BaseException(typeof(Range), technicalError);
            }

            // "bytes=-500"
            if (0 == indexOfHyphen)
            {
                long firstBytePosition = System.Int64.Parse(range);
                answer._firstBytePosition = new BytePosition(firstBytePosition);
                return answer;
            }

            // "bytes=9500-"
            int rangeLength = range.Length;
            if ('-' == range[rangeLength - 1])
            {
                range = range.Substring(0, rangeLength - 1);
                long firstBytePosition = System.Int64.Parse(range);
                answer._firstBytePosition = new BytePosition(firstBytePosition);

                return answer;

            }

            // "bytes=500-999"
            String startPosition = range.Substring(0, indexOfHyphen);
            // long firstBytePosition = System.Int64.Parse(startPosition); // c-sharp does not like this ... Error	1	Elements defined in a namespace cannot be explicitly declared as private, protected, or protected internal	H:\projects.windows\c-sharp\vlc_amigo.solution\jsonbroker.library\common\http\headers\request\Range.cs	11	21	jsonbroker.library
            answer._firstBytePosition = new BytePosition(System.Int64.Parse(startPosition));

            String endPosition = range.Substring(indexOfHyphen + 1);
            answer._lastBytePosition = new BytePosition(System.Int64.Parse(endPosition));

            return answer;

        }

        public String ToContentRange(long entityContentLength)
        {
            // http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.16
            long firstBytePosition = 0;
            if (null != _firstBytePosition)
            {
                firstBytePosition = _firstBytePosition.Value;

                // negative firstBytePosition is an offset from the end of the entityContentLength
                if (0 > firstBytePosition)
                {
                    firstBytePosition = entityContentLength + firstBytePosition;
                }
            }

            // http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.16
            long lastBytePosition = entityContentLength - 1;
            if (null != _lastBytePosition)
            {
                lastBytePosition = _lastBytePosition.Value;
            }

            String answer = String.Format("bytes {0}-{1}/{2}", firstBytePosition, lastBytePosition, entityContentLength);

            return answer;
        }


        public long getSeekPosition(long entityContentLength)
        {
            long firstBytePosition = 0;
            if (null != _firstBytePosition)
            {
                firstBytePosition = _firstBytePosition.Value;

                // negative firstBytePosition is an offset from the end of the entityContentLength
                if (0 > firstBytePosition)
                {
                    firstBytePosition = entityContentLength + firstBytePosition;
                }
            }
            long answer = firstBytePosition;
            return answer;
        }

        public long getContentLength(long entityContentLength)
        {

            long firstBytePosition = 0;
            if (null != _firstBytePosition)
            {
                firstBytePosition = _firstBytePosition.Value;

                // negative firstBytePosition is an offset from the end of the entityContentLength
                if (0 > firstBytePosition)
                {
                    firstBytePosition = entityContentLength + firstBytePosition;
                }
            }

            long lastBytePosition = entityContentLength;
            if (null != _lastBytePosition)
            {
                lastBytePosition = _lastBytePosition.Value;

                // vvv http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35.1

                // The last-byte-pos value gives the byte-offset of the last byte in the range; 
                // that is, the byte positions specified are inclusive

                // ^^^ http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35.1

                // so ... 
                lastBytePosition++;

                    
            }

            long answer = lastBytePosition - firstBytePosition;
            return answer;

        }

    }
}
