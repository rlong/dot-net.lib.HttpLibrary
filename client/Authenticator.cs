// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.Http.headers;
using dotnet.lib.CoreAnnex.log;
using System.IO;
using dotnet.lib.Http.headers.request;
using dotnet.lib.Http.headers.response;
using dotnet.lib.Http.authentication;
using dotnet.lib.CoreAnnex.auxiliary;

namespace dotnet.lib.Http.client
{
    public class Authenticator 
    {

        private static Log log = Log.getLog(typeof(Authenticator));


        ///////////////////////////////////////////////////////////////////////
        //
        String _ha1;

        ///////////////////////////////////////////////////////////////////////
        //
        ClientSecurityConfiguration _securityConfiguration;

        public ClientSecurityConfiguration getSecurityConfiguration()
        {
            return _securityConfiguration;
        }

        /////////////////////////////////////////////////////////
        // authInt
        private bool _authInt;

        public bool AuthInt
        {
            get { return _authInt; }
            protected set { _authInt = value; }
        }

        /////////////////////////////////////////////////////////
        // authorizationRequestHeader
        private Authorization _authorization;

        public Authorization AuthorizationRequestHeader
        {
            get { return _authorization; }
            protected set { _authorization = value; }
        }



        /////////////////////////////////////////////////////////
        public Authenticator(bool authInt, ClientSecurityConfiguration clientSecurityConfiguration)
        {
            _authInt = authInt;
            _securityConfiguration = clientSecurityConfiguration;
        }


        // can return null 
        private String getEntityBodyHash(Entity requestEntity)
        {

            if (null == requestEntity)
            {
                byte[] emptyEntity = { };
                return SecurityUtilities.md5HashOfBytes(emptyEntity);
            }

            if( !(requestEntity is DataEntity) ) {
                log.warnFormat("!(requestEntity is DataEntity; requestEntity.GetType().Name = '{0}'", requestEntity.GetType().Name);
                return null;
            }

            DataEntity dataEntity = (DataEntity)requestEntity;
            Data data = dataEntity.Data;

            return SecurityUtilities.md5HashOfData(data);
        }

        // can return null 
        private String getHa2(String method, String requestUri, Entity requestEntity)
        {
            // RFC-2617 3.2.2.3 A2
            String a2;

            if (_authInt)
            {
                String entityBodyHash = getEntityBodyHash(requestEntity);
                if (null == entityBodyHash)
                {
                    return null;
                }
                log.debug(entityBodyHash, "entityBodyHash");

                a2 = String.Format("{0}:{1}:{2}", method, requestUri, entityBodyHash);
            }
            else
            {
                a2 = String.Format("{0}:{1}", method, requestUri);
            }

            String ha2 = SecurityUtilities.md5HashOfString(a2);
            log.debug(ha2, "ha2");
            String answer = ha2;
            return answer;

        }

        // can return null 
        public String getRequestAuthorization(String method, String requestUri, Entity requestEntity)
        {
            log.enteredMethod();

            if (null == _authorization)
            {
                log.info("null == _authorization");
                return null;
            }

            if (null == _ha1) // should not happen, but ... 
            { 
                log.warn("null == _ha1");
                return null;
            }


            String ha2 = getHa2(method, requestUri, requestEntity);
            if (null == ha2)
            {
                log.warn("null == ha2");
                return null;
            }


            _authorization.uri = requestUri;

            if (_authInt)
            {
                _authorization.qop = "auth-int";
            }
            else
            {
                _authorization.qop = "auth";
            }

            String response;
            {
                String unhashedResponse = String.Format("{0}:{1}:{2:x8}:{3}:{4}:{5}",
                        _ha1, _authorization.nonce, _authorization.nc,
                        _authorization.cnonce, _authorization.qop, ha2);

                response = SecurityUtilities.md5HashOfString(unhashedResponse);
            }

            _authorization.response = response;

            return _authorization.ToString();

        }


        public void handleHttpResponseHeaders(System.Net.WebHeaderCollection headers)
        {
            log.enteredMethod();


            String wwwAuthenticate = headers.Get("WWW-Authenticate");


            if (null != wwwAuthenticate) 
            {

                log.warn(wwwAuthenticate);

                WwwAuthenticate authenticateResponseHeader = WwwAuthenticate.buildFromString(wwwAuthenticate);

                String serverRealm = authenticateResponseHeader.realm;
                Subject server = _securityConfiguration.getServer(serverRealm);

                if (null == server)
                {
                    log.warnFormat("null == _subject; serverRealm = '{0}'", serverRealm);
                    _authorization = null;
                    _ha1 = null;
                    return;
                }

                _ha1 = server.getHa1();

                _authorization = new Authorization();
                _authorization.nc = 1;
                _authorization.cnonce = SecurityUtilities.generateNonce();
                _authorization.nonce = authenticateResponseHeader.nonce;
                _authorization.opaque = authenticateResponseHeader.opaque;
                _authorization.qop = authenticateResponseHeader.qop;
                _authorization.realm = server.Realm;
                _authorization.username = server.Username;

                return; // our work here is done
            }

            String authenticationInfo = headers.Get("Authentication-Info");

            if (null != authenticationInfo)
            {

                AuthenticationInfo authenticationInfoHeader = AuthenticationInfo.buildFromString( authenticationInfo );

                long nc = _authorization.nc + 1;
                _authorization.nc = nc;
                _authorization.nonce = authenticationInfoHeader.nextnonce;

                return; // our work here is done

            }

            log.warn("did not find a 'WWW-Authenticate' or a 'Authentication-Info'");
            
        }
    }
}
