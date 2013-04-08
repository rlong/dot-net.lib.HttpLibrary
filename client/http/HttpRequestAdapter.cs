// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.http;

namespace jsonbroker.library.client.http
{
    public class HttpRequestAdapter
    {

        /////////////////////////////////////////////////////////
        // requestUri
        private String _requestUri;

        public String RequestUri
        {
            get { return _requestUri; }
            set { _requestUri = value; }
        }


        /////////////////////////////////////////////////////////
        // requestHeaders
        private Dictionary<String, String> _requestHeaders;

        public Dictionary<String, String> RequestHeaders
        {
            get { return _requestHeaders; }
            set { _requestHeaders = value; }
        }

        /////////////////////////////////////////////////////////
        // requestEntity
        private Entity _requestEntity;

        public Entity RequestEntity
        {
            get { return _requestEntity; }
            set { _requestEntity = value; }
        }


        ////////////////////////////////////////////////////////////////////////////
        public HttpRequestAdapter(String requestUri)
        {

            _requestUri = requestUri;
            _requestHeaders = new Dictionary<String, String>();
        }

        ////////////////////////////////////////////////////////////////////////////


    }
}
