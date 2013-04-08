// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.common.http
{
    public class DataEntity : Entity
    {

        /////////////////////////////////////////////////////////
        // data
        private Data _data;

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
            return _data.ToStream();
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

    }
}
