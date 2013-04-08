// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.auxiliary
{
    public class Data : Loggable
    {

        private static readonly char[] HEXADECIMAL_DIGITS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };


        ///////////////////////////////////////////////////////////////////////
        protected MemoryStream _buffer;
        public MemoryStream buffer
        {
            get { return _buffer; }
            protected set { _buffer = value; }
        }


        ///////////////////////////////////////////////////////////////////////


        public Data()
        {
            _buffer = new MemoryStream(); 
        }

        public Data(Stream content, int contentLength)
        {
            _buffer = new MemoryStream(contentLength);
            StreamUtilities.write(contentLength,content, _buffer );
        }

        public Data(byte[] byteBuffer)
        {
            _buffer = new MemoryStream(byteBuffer, 0, byteBuffer.Length, true, true); // need to use this constructor to be able to call " _buffer.GetBuffer()[offset]" later
        }

        public Data(MemoryStream bytes)
        {
            _buffer = bytes;
        }

        public Data(uint capacity)
        {
            _buffer = new MemoryStream((int)capacity);
        }


        ///////////////////////////////////////////////////////////////////////


        public uint Length
        {
            
            get {
                seekToStart();
                return (uint)_buffer.Length;
            }
            protected set {}
        }

        ///////////////////////////////////////////////////////////////////////


        public void CopyTo( Stream destination ) {
            seekToStart();
            //_buffer.CopyTo(destination); // .NET 4.0
            StreamUtilities.write(_buffer, destination);            

        }

        public Stream ToStream()
        {
            return new MemoryStream(_buffer.GetBuffer(), 0, (int)_buffer.Length, false);
        }


        protected void seekToStart()
        {
            _buffer.Seek(0, SeekOrigin.Begin);
        }

        ///////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////

        public String getUtf8String(uint offset, uint length)
        {
            byte[] buffer = _buffer.GetBuffer();

            UTF8Encoding utf8Encoding = new UTF8Encoding();

            String answer = utf8Encoding.GetString(buffer, (int)offset, (int)length);

            return answer;

        }

        public void arraycopy(uint sourceOffset, byte[] destination, uint destinationOffset, uint length)
        {
            Array.Copy(_buffer.GetBuffer(), sourceOffset, destination, destinationOffset, length);
        }


        public byte getByte(uint offset)
        {
            return _buffer.GetBuffer()[offset];
        }


        public uint getCount()
        {
            seekToStart();
            return (uint)_buffer.Length;
        }


        public void writeTo(Stream destination)
        {
            //_buffer.CopyTo(destination); // .NET 4.0
            StreamUtilities.write(_buffer, destination);
            
        }

        private String getDataDebugLine(uint offset)
        {

            uint count = 16;
            uint upperBound = offset + 16;

            if (upperBound > getCount())
            {
                upperBound = getCount();
                count = getCount() - offset;
            }

            StringBuilder answer = new StringBuilder(80);

            for (uint i = offset; i < upperBound; i++)
            {

                byte b = getByte(i);

                answer.Append(HEXADECIMAL_DIGITS[(b >> 4) & 0xF]); // shift the offset to the right

                answer.Append(HEXADECIMAL_DIGITS[b & 0xF]); // mask off the high bits

                if (i > 0)
                {
                    if (0 == ((i + 1) % 4))
                    {
                        answer.Append(' ');
                    }
                }
            }

            for (uint i = count; i < 16; i++)
            {
                answer.Append("  ");

                if (i > 0)
                {
                    if (0 == ((i + 1) % 4))
                    {
                        answer.Append(' ');
                    }
                }
            }

            // ascii


            for (uint i = offset; i < upperBound; i++)
            {

                byte b = getByte(i);


                if ('!' <= b && '~' >= b)
                {
                    answer.Append((char)b);
                }
                else
                {
                    answer.Append('.');
                }

            }

            return answer.ToString();

        }


        public String[] getLogMessages()
        {

            List<String> arrayList = new List<String>();

            uint length = getCount();

            arrayList.Add("length = " + length);

            for (uint offset = 0; offset < length; offset += 16)
            {

                String message = getDataDebugLine(offset);
                arrayList.Add(message);
            }

            String[] answer = new String[arrayList.Count];
            arrayList.CopyTo(answer);            
            return answer;
        }


    }
}
