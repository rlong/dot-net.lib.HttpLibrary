// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.auxiliary;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.http.multi_part
{

    
    /// <summary>
    /// 
    /// as per http://www.w3.org/Protocols/rfc1341/7_2_Multipart.html
    /// </summary>
    public class MultiPartReader
    {

        internal static readonly int BUFFER_SIZE = 8 * 1024;

        private static Log log = Log.getLog(typeof(MultiPartReader));

        ///////////////////////////////////////////////////////////////////////
        // boundary
        private String _boundary;

        public String Boundary
        {
            get { return _boundary; }
            set { _boundary = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // content
        private Stream _content;

        ///////////////////////////////////////////////////////////////////////
        // contentRemaining
        private long _contentRemaining;

        ///////////////////////////////////////////////////////////////////////
        // buffer
        private byte[] _buffer;


        ///////////////////////////////////////////////////////////////////////
        // currentOffset
        private int _currentOffset;

        public int CurrentOffset
        {
            get { return _currentOffset; }
            internal set { _currentOffset = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // bufferEnd
        private int _bufferEnd;

        public int BufferEnd
        {
            get { return _bufferEnd; }
            set { _bufferEnd = value; }
        }



        ///////////////////////////////////////////////////////////////////////
        // 
        public MultiPartReader(String boundary, Entity entity)
        {
            _boundary = boundary;
            _content = entity.getContent();
            _contentRemaining = entity.getContentLength();

            _buffer = new byte[BUFFER_SIZE];
            _currentOffset = 0;
            _bufferEnd = 0;
        }




        private void FillBuffer()
        {
            _currentOffset = 0;

            long amountToRead = _contentRemaining;

            if (amountToRead > _buffer.Length)
            {
                amountToRead = _buffer.Length;
            }
            int bytesRead = _content.Read(_buffer, 0, (int)amountToRead);
            if (0 == bytesRead && 0 != amountToRead) // `0 == bytesRead` marks the end of the stream
            {
                BaseException e = new BaseException(this, "0 == bytesRead && 0 != amountToRead; amountToRead = {0}; _contentRemaining = {1}", amountToRead, _contentRemaining);
                throw e;
            }
            _contentRemaining -= bytesRead;
            _bufferEnd = bytesRead;
        }

        private byte ReadByte()
        {
            if (_currentOffset == _bufferEnd)
            {
                FillBuffer();
            }
            byte answer = _buffer[_currentOffset];
            _currentOffset++;
            return answer;
        }

        internal String ReadLine(MutableData stringBuffer)
        {
            byte lastChar = 88; // 'X'

            stringBuffer.clear();
            
            while (true)
            {
                byte currentChar = ReadByte();

                if (0x0d == lastChar) // '\r'
                {
                    if (0x0a == currentChar) // `\n`
                    {
                        return stringBuffer.getUtf8String(0, stringBuffer.getCount());                        
                    }
                    else
                    {
                        stringBuffer.Append( lastChar); // add the previous '\r' 
                        
                    }
                }
                if (0x0d != currentChar) // '\r'
                {
                    stringBuffer.Append(currentChar); 
                    
                }

                lastChar = currentChar;

            }
        }


        // used for testing only 
        internal DelimiterIndicator skipToNextDelimiterIndicator()
        {
            DelimiterDetector detector = new DelimiterDetector(_boundary);

            if (_currentOffset == _bufferEnd)
            {
                FillBuffer();
            }
            
            DelimiterIndicator indicator = detector.update(_buffer, _currentOffset, _bufferEnd);

            if (null == indicator)
            {
                return null;
            }

            if( indicator is DelimiterFound ) {
                DelimiterFound delimiterFound = (DelimiterFound)indicator;
                _currentOffset = delimiterFound.EndOfDelimiter;
            }
            
            return indicator;

        }


        // can return null if not indicator was found (partial or complete)
        private DelimiterFound findFirstDelimiterIndicator(DelimiterDetector detector)
        {

            if (_currentOffset == _bufferEnd)
            {
                FillBuffer();
            }

            DelimiterIndicator indicator = detector.update(_buffer, _currentOffset, _bufferEnd);

            if (null == indicator)
            {
                throw new BaseException(this, "null == indicator, could not find first delimiter; _boundary = '{0}'", _boundary);
            }

            if (!(indicator is DelimiterFound) )
            {
                log.error("unimplemented: support for `DelimiterIndicator` types that are not `DelimiterFound`");
                throw new BaseException(this, "!(indicator is DelimiterFound); indicator.GetType().Name", indicator.GetType().Name);
            }

            return (DelimiterFound)indicator;

        }


        // will accept `null` values 
        private void WritePartialDelimiter(PartialDelimiterMatched partialDelimiterMatched, PartHandler partHandler)
        {
            if (null == partialDelimiterMatched)
            {
                return;
            }

            byte[] previouslyMatchingBytes = partialDelimiterMatched.MatchingBytes;
            partHandler.HandleBytes(previouslyMatchingBytes, 0, previouslyMatchingBytes.Length);


        }

        private DelimiterFound TryProcessPart(PartHandler partHandler, DelimiterDetector detector)
        {
            MutableData stringBuffer = new MutableData();
            String line = ReadLine(stringBuffer);

            while ( 0 != line.Length )
            {
                
                int firstColon = line.IndexOf(":");
                if (-1 == firstColon)
                {
                    throw new BaseException(this, "-1 == firstColon; line = '{0}'", line);
                }

                String name = line.Substring(0, firstColon).ToLower(); // headers are case insensitive
                String value = line.Substring(firstColon + 1).Trim();

                partHandler.HandleHeader(name, value);

                line = ReadLine(stringBuffer);
            }

            PartialDelimiterMatched partialDelimiterMatched = null;

            bool partCompleted = false;
            while (!partCompleted)
            {

                DelimiterIndicator delimiterIndicator = detector.update(_buffer, _currentOffset, _bufferEnd);

                // nothing detected ? 
                if (null == delimiterIndicator)
                {
                    // write existing partial match (if it exists)
                    {
                        WritePartialDelimiter(partialDelimiterMatched, partHandler);
                        partialDelimiterMatched = null;
                    }

                    int length = _bufferEnd - _currentOffset;
                    partHandler.HandleBytes(_buffer, _currentOffset, length);
                    FillBuffer();
                    continue;
                }

                if (delimiterIndicator is DelimiterFound)
                {
                    DelimiterFound delimiterFound = (DelimiterFound)delimiterIndicator;
                    // more content to add ? 
                    if (!delimiterFound.CompletesPartialMatch)
                    {
                        // write existing partial match (if it exists)
                        {
                            WritePartialDelimiter(partialDelimiterMatched, partHandler);
                            partialDelimiterMatched = null;
                        }

                        int length = delimiterFound.StartOfDelimiter - _currentOffset;
                        partHandler.HandleBytes(_buffer, _currentOffset, length);
                    }
                    else // delimiterFound completesPartialMatch
                    {
                        partialDelimiterMatched = null;
                    }

                    _currentOffset = delimiterFound.EndOfDelimiter;

                    partHandler.PartCompleted();
                    partCompleted = true; // not required, but signalling intent
                    return delimiterFound;
                }

                if (delimiterIndicator is PartialDelimiterMatched)
                {
                    // write existing partial match (if it exists)
                    {
                        WritePartialDelimiter(partialDelimiterMatched, partHandler);                        
                    }
                    partialDelimiterMatched = (PartialDelimiterMatched)delimiterIndicator;
                    byte[] matchingBytes = partialDelimiterMatched.MatchingBytes;
                    int startOfMatch = _bufferEnd - matchingBytes.Length;
                    if (startOfMatch < _currentOffset)
                    {
                        // can happen when the delimiter straddles 2 distinct buffer reads of size `BUFFER_SIZE`
                        startOfMatch = _currentOffset;
                    }
                    else
                    {
                        int length = startOfMatch - _currentOffset;
                        partHandler.HandleBytes(_buffer, _currentOffset, length);
                    }
                    FillBuffer();
                }
            }

            // will never happen ... we hope
            throw new BaseException(this, "unexpected code path followed");
        }
        
        private DelimiterFound ProcessPart(MultiPartHandler multiPartHandler, DelimiterDetector detector)
        {

            PartHandler partHandler = multiPartHandler.FoundPartDelimiter();

            try
            {
                return TryProcessPart(partHandler, detector);
            }
            catch (Exception e)
            {
                partHandler.HandleException(e);
                throw e;
            }

        }


        private void TryProcess(MultiPartHandler multiPartHandler, bool skipFirstCrNl)
        {
            DelimiterDetector detector = new DelimiterDetector(_boundary);

            if (skipFirstCrNl)
            {
                byte[] crnl = new byte[]{0x0d,0x0a}; // 0x0d = cr; 0x0a = nl
                detector.update(crnl, 0, 2);
            }

            DelimiterIndicator indicator = findFirstDelimiterIndicator(detector);

            if (null == indicator)
            {
                BaseException e = new BaseException(this, "null == indicator; expected delimiter at start of stream");
                throw e;
            }

            if (!(indicator is DelimiterFound))
            {
                log.error("unimplemented: support for `DelimiterIndicator` types that are not `DelimiterFound`");
                throw new BaseException(this, "!(indicator is DelimiterFound); indicator.GetType().Name", indicator.GetType().Name);
            }


            DelimiterFound delimiterFound = (DelimiterFound)indicator;

            _currentOffset = delimiterFound.EndOfDelimiter;


            while (!delimiterFound.IsCloseDelimiter)
            {
                delimiterFound = ProcessPart(multiPartHandler, detector);
            }

            multiPartHandler.FoundCloseDelimiter();

            while (0 != _contentRemaining)
            {
                FillBuffer();
            }

        }



        public void Process( MultiPartHandler multiPartHandler, bool skipFirstCrNl ) 
        {

            try
            {
                TryProcess(multiPartHandler, skipFirstCrNl);                
            }
            catch (Exception e)
            {
                multiPartHandler.HandleException(e);
            }
        }

        public void Process(MultiPartHandler multiPartHandler)
        {
            this.Process(multiPartHandler, false);
        }

    }
}
