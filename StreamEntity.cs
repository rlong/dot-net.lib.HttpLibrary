// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using dotnet.lib.CoreAnnex.exception;
using dotnet.lib.CoreAnnex.auxiliary;
using dotnet.lib.CoreAnnex.io;

namespace dotnet.lib.Http
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


        public void WriteTo(Stream destination, long offset, long length)
        {

            getContent(); // force the stream to be opened if it has not already been


            _content.Seek(offset, SeekOrigin.Current);
            StreamHelper.write(length, _content, destination);
            StreamHelper.close(_content, false, this);
            _content = null;
        }
    }
}
