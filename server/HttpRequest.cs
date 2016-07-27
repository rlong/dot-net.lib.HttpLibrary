// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using dotnet.lib.Http;
using dotnet.lib.Http.headers.request;
using dotnet.lib.CoreAnnex.auxiliary;

namespace dotnet.lib.Http.server
{


    public class HttpRequest
    {


        /////////////////////////////////////////////////////////
        // created
        private long _created;

        public long Created
        {
            get { return _created; }
            set { _created = value; }
        }

        /////////////////////////////////////////////////////////
        // method
        private HttpMethod _method;

        public HttpMethod Method
        {
            get { return _method; }
            set { _method = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        private string _requestUri;
        public string RequestUri
        {
            get { return _requestUri; }
            set { _requestUri = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        private Dictionary<String,String> _headers;
        public Dictionary<String, String>  headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        /////////////////////////////////////////////////////////
        // entity
        private Entity _entity;

        public Entity Entity
        {
            get { return _entity; }
            set { _entity = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        // closeConnectionIndicated
        private BooleanObject _closeConnectionIndicated;


        ////////////////////////////////////////////////////////////////////////////
        Range _range;

        public Range getRange()
        {

            if (null != _range)
            {
                return _range;
            }

            if (!_headers.ContainsKey("range"))
            {
                return null;
            }

            String rangeValue = _headers["range"];
            _range = Range.buildFromString(rangeValue);
            return _range;
        }


        public HttpRequest()
        {
            // DateTime .Ticks Property ... 
            //A single tick represents one hundred nanoseconds or one ten-millionth of a second. There are 10,000 ticks in a millisecond.
            _created = DateTime.Now.Ticks;
            _method = HttpMethod.GET;
            _headers = new Dictionary<String,String>();
            
        }

        public void setHttpHeader(String headerName, String headerValue)
        {
            _headers[headerName] = headerValue;
        }

        // can return null
        public String getHttpHeader(String headerName)
        {
            if (!_headers.ContainsKey(headerName))
            {
                return null;
            }
            return _headers[headerName];
        }


        // vvv http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.10
        public bool isCloseConnectionIndicated() 
        {
            if (null != _closeConnectionIndicated)
            {
                return _closeConnectionIndicated.booleanValue();
            }

            if (!_headers.ContainsKey("connection"))
            {
                _closeConnectionIndicated = BooleanObject.FALSE;
            }
            else
            {
                if ("close".Equals(_headers["connection"]))
                {
                    _closeConnectionIndicated = BooleanObject.TRUE;
                }
                else
                {
                    _closeConnectionIndicated = BooleanObject.FALSE;
                }
            }
            return _closeConnectionIndicated.booleanValue();
        }
    
        // ^^^ http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.10


    }
}
