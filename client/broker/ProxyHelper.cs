// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.server.broker;
using jsonbroker.library.common.log;
using jsonbroker.library.common.broker;
using jsonbroker.library.client.broker;
using jsonbroker.library.common.network;
using jsonbroker.library.client.http;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.security;

namespace jsonbroker.library.client.broker
{
    public class ProxyHelper 
    {

        private static readonly Log log = Log.getLog(typeof(ProxyHelper));


        ////////////////////////////////////////////////////////////////////////////
        // host
        private String _host;

        public String Host
        {
            get { return _host; }
            protected set { _host = value; }
        }

        ////////////////////////////////////////////////////////////////////////////
        // port
        private int _port;

        public int Port
        {
            get { return _port; }
            protected set { _port = value; }
        }


        ////////////////////////////////////////////////////////////////////////////
        // openHttpProxy
        private ServiceHttpProxy _openHttpProxy;


        ////////////////////////////////////////////////////////////////////////////
        // authHttpProxy
        private ServiceHttpProxy _authHttpProxy;

        ////////////////////////////////////////////////////////////////////////////
        //
        public ProxyHelper()
        {

        }


        public ServiceHttpProxy getOpenHttpProxy()
        {
            if (null == _host)
            {
                BaseException e = new BaseException(this, "null == _host");
                throw e;
            }

            if (null != _openHttpProxy)
            {
                return _openHttpProxy;
            }

            NetworkAddress networkAddress = new NetworkAddress(_host, _port);
            HttpDispatcher httpDispatcher = new HttpDispatcher(networkAddress);
            _openHttpProxy = new ServiceHttpProxy(httpDispatcher);

            return _openHttpProxy;
        }


        public ServiceHttpProxy getAuthHttpProxy(ClientSecurityConfiguration clientSecurityConfiguration)
        {
            if (null == _host)
            {
                BaseException e = new BaseException(this, "null == _host");
                throw e;
            }

            if (null != _authHttpProxy && _authHttpProxy.Authenticator.getSecurityConfiguration() == clientSecurityConfiguration)
            {
                return _authHttpProxy;
            }

            NetworkAddress networkAddress = new NetworkAddress(_host, _port);
            HttpDispatcher httpDispatcher = new HttpDispatcher(networkAddress);
            Authenticator authenticator = new Authenticator(false, clientSecurityConfiguration);
            _authHttpProxy = new ServiceHttpProxy(httpDispatcher, authenticator);

            return _authHttpProxy;
        }

        ////////////////////////////////////////////////////////////////////////////
        // username, realm, password can be null
        public void initialize(String host, int port )
        {
            _host = host;
            _port = port;

            _openHttpProxy = null;
            _authHttpProxy = null;

        }


    }
}
