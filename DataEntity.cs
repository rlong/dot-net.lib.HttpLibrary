// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using dotnet.lib.CoreAnnex.auxiliary;
using dotnet.lib.CoreAnnex.io;

namespace dotnet.lib.Http
{
    public class DataEntity : Entity
    {

        /////////////////////////////////////////////////////////
        // data
        private Data _data;


        ///////////////////////////////////////////////////////////////////////
        // streamContent
        private Stream _streamContent;


        public Data Data
        {
            get { return _data; }
            protected set { _data = value; }
        }

        public DataEntity(Data data)
        {
            _data = data;
        }


        public Stream getContent()
        {

            if (null != _streamContent)
            {
                StreamHelper.close(_streamContent, false, this);
            }
            _streamContent = _data.ToStream();

            return _streamContent;
        }


        public long getContentLength()
        {
            return _data.getCount();
        }

        public String getMimeType()
        {
            return null;
        }

        public String md5()
        {
            return SecurityUtilities.md5HashOfData(_data);
        }


        public void WriteTo(Stream destination, long offset, long length)
        {
            Stream content = this.getContent();
            content.Seek(offset, SeekOrigin.Current);
            StreamHelper.write(length, content, destination);
            StreamHelper.close(_streamContent, false, this);
            _streamContent = null;
        }

    }
}
            
