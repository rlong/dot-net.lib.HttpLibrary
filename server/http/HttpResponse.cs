// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using jsonbroker.library.common.log;
using System.Net;
using jsonbroker.library.common.http.headers;
using jsonbroker.library.common.http;
using jsonbroker.library.common.http.headers.request;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.server.http
{
    public class HttpResponse
    {
        private static Log log = Log.getLog(typeof(HttpResponse));


        //////////////////////////////////////////////////////////////////////
        // status
        private int _status;

        public int Status
        {
            get { return _status; }
            protected set { _status = value; }
        }

        //////////////////////////////////////////////////////////////////////
        private Dictionary<String,String> _headers;
        public Dictionary<String, String> headers
        {
            get { return _headers; }
            set { _headers = value; }
        }


        //////////////////////////////////////////////////////////////////////
        // range
        private Range _range;

        public Range Range
        {
            get { return _range; }
            set { _range = value; }
        }

        //////////////////////////////////////////////////////////////////////
        // entity
        private Entity _entity;

        public Entity Entity
        {
            get { return _entity; }
            protected set { _entity = value; }
        }


        //////////////////////////////////////////////////////////////////////
        public HttpResponse(int status)
        {
            _status = status;
            _headers = new Dictionary<String, String>();

        }

        public HttpResponse(int status, Entity entity)
        {
            _status = status;
            _headers = new Dictionary<String, String>();
            _entity = entity;
        }





        //////////////////////////////////////////////////////////////////////

        public void setContentType(String contentType)
        {
            _headers.Add("Content-Type", contentType);
        }

        public void putHeader(String name, String value)
        {
            _headers.Add(name, value);
        }

        // can return null
        public String getHeader(String name)
        {
            if (_headers.ContainsKey(name))
            {
                return _headers[name]; 
            }
            return null;
        }

        public LongObject getContentLength()
        {
            if (null == _entity)
            {
                return null;
            }

            long entityContentLength = _entity.getContentLength();
            if (null == _range)
            {
                return new LongObject(_entity.getContentLength());
            }

            long rangedContentLength = _range.getContentLength(entityContentLength);
            LongObject answer = new LongObject(rangedContentLength);
            return answer;
        }
    }
}
