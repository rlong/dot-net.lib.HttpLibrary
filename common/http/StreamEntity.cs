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

namespace jsonbroker.library.common.http
{
    public class StreamEntity : Entity
    {

        /////////////////////////////////////////////////////////
        // contentLength
        private long _contentLength;

        public long ContentLength
        {
            get {
                if (-1 == _contentLength)
                {
                    _contentLength = FileUtilities.GetFileLength(_contentFilePath);
                }
                return _contentLength; 
            }
            protected set { _contentLength = value; }
        }

        /////////////////////////////////////////////////////////
        // content
        private Stream _content;

        public Stream Content
        {
            get {
                if (null == _content)
                {
                    _content = new FileStream(_contentFilePath, FileMode.Open, FileAccess.Read);
                }
                return _content; 
            }
            set { _content = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // mimeType
        private String _mimeType;

        public String MimeType
        {
            get { return _mimeType; }
            set { _mimeType = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // contentFilePath
        private String _contentFilePath;

        ///////////////////////////////////////////////////////////////////////
        // 
        public StreamEntity(long contentLength, Stream content)
        {
            _contentLength = contentLength;
            _content = content;
        }

        public StreamEntity(String contentSourceFilePath, String mimeType)
        {
            _contentLength = -1; // evaluate it lazily  
            _content = null; // evaluate it lazily 
            _contentFilePath = contentSourceFilePath;
            _mimeType = mimeType;
        }

        public Stream getContent()
        {
            return this.Content;
        }

        public long getContentLength()
        {
            return this.ContentLength;
        }

        public String getMimeType()
        {
            return _mimeType;
        }

        public String md5()
        {
            throw new BaseException(this, "unsupported");
        }

        public void TeardownForCaller(bool swallowErrors, Object caller)
        {
            StreamHelper.close(_content, swallowErrors, this);
        }


        public void WriteTo(Stream destination, long offset, long length)
        {
            _content.Seek(offset, SeekOrigin.Current);
            StreamHelper.write(length, _content, destination);
        }

    }
}
