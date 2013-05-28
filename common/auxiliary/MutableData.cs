// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace jsonbroker.library.common.auxiliary
{
    // modelled after NSMutableData ... a resizeable memory buffer where we can access the raw buffer
    public class MutableData : Data
    {
        ///////////////////////////////////////////////////////////////////////

        public MutableData()
        {
        }

        public MutableData(byte[] byteBuffer) : base(byteBuffer)
        {
        }

        public MutableData(MemoryStream bytes) : base(bytes) 
        {
        }

        public MutableData(uint capacity) : base(capacity)
        {
        }


        protected void seekToEnd()
        {
            _buffer.Seek(0, SeekOrigin.End);
        }

        public void Append(Stream source)
        {
            seekToEnd();
            //source.CopyTo(_buffer); // .NET 4.0
            StreamHelper.write(source, _buffer);
        }

        //
        public void Append(Stream source, uint length)
        {
            seekToEnd();
            //source.CopyTo(_buffer, (int)length); // .NET 4.0
            StreamHelper.write(length,source, _buffer); 
                 
        }

        public void Append(byte[] buffer)
        {
            this.Append(buffer, 0, buffer.Length);
        }

        public void Append(byte[] buffer, int offset, int count)
        {
            seekToEnd();
            _buffer.Write(buffer, offset, count);
        }



        ///////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////


        public void append(byte[] buffer, int offset, int count)
        {
            _buffer.Write(buffer, offset, count);

        }

        public void Append(byte b)
        {
            _buffer.WriteByte(b);
        }


        public void append(Stream source, uint length)
        {
            StreamHelper.write(length,source, _buffer);
        }

        public virtual void clear()
        {
            _buffer.SetLength(0);
        }

    }
}
