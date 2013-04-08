// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.http;
using jsonbroker.library.common.log;
using jsonbroker.library.common.http.headers;
using jsonbroker.library.common.http.headers.request;
using jsonbroker.library.common.http.headers.response;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.security;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.server.http.security
{
    public class HttpSecuritySession
    {
        private static Log log = Log.getLog(typeof(HttpSecuritySession));


        /////////////////////////////////////////////////////////
        // usersRealm
        private String _usersRealm;

        String UsersRealm
        {
            get { return _usersRealm; }
            set { _usersRealm = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // cnonce;
        String _cnonce;

        String cnonce
        {
            get { return _cnonce; }
            set { _cnonce = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        // cnoncesUsed;
        Dictionary<String, String> _cnoncesUsed;

        Dictionary<String, String> cnoncesUsed
        {
            get { return _cnoncesUsed; }
            set { _cnoncesUsed = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // nc;
        long _nc;

        long nc
        {
            get { return _nc; }
            set { _nc = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        // nonce;
        String _nonce;
        String nonce
        {
            get { return _nonce; }
            set { _nonce = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        // opaque;
        String _opaque;
        public String opaque
        {
            get { return _opaque; }
            set { _opaque = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // registeredSubject;
        Subject _registeredSubject;
        public Subject registeredSubject
        {
            get { return _registeredSubject; }
            set { _registeredSubject = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // idleSince;
        DateTime _idleSince;
        DateTime idleSince
        {
            get { return _idleSince; }
            set { _idleSince = value; }
        }


        ///////////////////////////////////////////////////////////////////////

        public HttpSecuritySession(String usersRealm)
        {
            _usersRealm = usersRealm;

            _idleSince = DateTime.Now;

            _nonce = SecurityUtilities.generateNonce();

            _cnoncesUsed = new Dictionary<string, string>();

            _opaque = SecurityUtilities.generateNonce();
        }


        ///////////////////////////////////////////////////////////////////////


        // section 3.2.2.3 of RFC-2671
        // entity can be null
        public static String getHa2(String method, String requestUri, Entity entity)
        {
            String entityBodyHash;
            if (null == entity)
            {
                byte[] emptyEntity = { };
                entityBodyHash = SecurityUtilities.md5HashOfBytes(emptyEntity);
            }
            else
            {
                if( !(entity is DataEntity) ) 
                {
                    String technicalError = String.Format("!(entity is DataEntity); entity.GetType().Name = '{0}'", entity.GetType().Name);
                    throw new BaseException(typeof(HttpSecuritySession), technicalError);
                }
                DataEntity dataEntity = (DataEntity)entity;
                Data data = dataEntity.Data;
                entityBodyHash = SecurityUtilities.md5HashOfData(data);
            }
            log.debug(entityBodyHash, "entityBodyHash");

            String a2 = String.Format("{0}:{1}:{2}", method, requestUri, entityBodyHash);

            String ha2 = SecurityUtilities.md5HashOfString(a2);
            return ha2;
        }

        // section 3.2.2.3 of RFC-2671
        public static String getHa2(String method, String requestUri)
        {
            String a2 = String.Format("{0}:{1}", method, requestUri);
            log.debug(a2, "a2");


            String ha2 = SecurityUtilities.md5HashOfString(a2);
            return ha2;

        }

        // entity can be null
        public static String getHa2(String method, String qop, String requestUri, Entity entity)
        {
            String ha2;

            if ("auth".Equals(qop))
            {
                ha2 = getHa2(method, requestUri);
            }
            else if ("auth-int".Equals(qop))
            {
                ha2 = getHa2(method, requestUri, entity);
            }
            else
            {
                log.errorFormat("unhandled qop; qop = '%s'", qop);
                throw HttpErrorHelper.unauthorized401FromOriginator(typeof(HttpSecuritySession));
            }

            return ha2;

        }

        private void validateAuthorization(Authorization authorization)
        {

            if (null == authorization)
            {
                log.error("null == authorization");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            if (null == _registeredSubject) // should not happen, but ...
            {
                log.error("null == _registeredSubject");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            // username & realm ... 
            _registeredSubject.validateAuthorizationRequestHeader(authorization);

            // cnonce ... 
            String submittedCnonce = authorization.cnonce;

            if (null == submittedCnonce)
            {
                log.error("null == submittedCnonce");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            // cnonce & nc ...
            if (submittedCnonce.Equals(_cnonce)) // client is re-using cnonce
            {
                if ((_nc + 1) != authorization.nc)
                {

                    log.errorFormat("(_nc + 1) != authorization.nc; _nc = {0}; authorization.nc = {1}", _nc, authorization.nc);
                    throw HttpErrorHelper.unauthorized401FromOriginator(this);
                }

            }
            else // client has a new cnonce
            {
                if (1 != authorization.nc)
                {
                    log.errorFormat("1 != authorization.nc; authorization.nc = {0}", authorization.nc);
                    throw HttpErrorHelper.unauthorized401FromOriginator(this);
                }

                if (_cnoncesUsed.ContainsKey(submittedCnonce))
                {
                    log.errorFormat("_cnoncesUsed.ContainsKey(submittedCnonce); submittedCnonce = '{0}'", submittedCnonce);
                    throw HttpErrorHelper.unauthorized401FromOriginator(this);
                }

            }


            // nonce ...
            if (null == authorization.nonce)
            {
                log.errorFormat("null == authorization.nonce");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            if (!_nonce.Equals(authorization.nonce))
            {
                log.errorFormat("!_nonce.Equals(authorization.nonce); _nonce = {0}; authorization.nonce = {1}", _nonce, authorization.nonce);
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            // opaque ...
            if (!_opaque.Equals(authorization.opaque)) // should not happen but ...
            {
                log.errorFormat("!_opaque.Equals(authorization.opaque); _opaque = '{0}'; authorization.opaque = '{1}'", _opaque, authorization.opaque);
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            // qop ...
            if (null == authorization.qop)
            {
                log.error("null == authorization.qop");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            if (("auth".Equals(authorization.qop) || "auth-int".Equals(authorization.qop)))
            {
                // ok 
            }
            else
            {
                log.errorFormat("unsupported qop; authorization.qop = '{0}'", authorization.qop);
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            // response
            if (null == authorization.response)
            {
                log.error("null == authorization.response");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }


            // uri
            if (null == authorization.uri)
            {
                log.error("null == authorization.uri");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

        }

        // this method has no effect on the state of 'self' ...
        // sections 3.2.2.1-3.2.2.3 of RFC-2617
        // entity can be null
        public void authorise(String method, Authorization authorization, Entity entity)
        {
            this.validateAuthorization(authorization);

            {
                String ha1 = _registeredSubject.ha1();

                String ha2 = getHa2(method, authorization.qop, authorization.uri, entity);

                String unhashedResponse = String.Format("{0}:{1}:{2:x8}:{3}:{4}:{5}", 
                    ha1, authorization.nonce, authorization.nc, authorization.cnonce, authorization.qop, ha2);

                String expectedResponse = SecurityUtilities.md5HashOfString(unhashedResponse);

                if (!expectedResponse.Equals(authorization.response))
                {
                    log.errorFormat("!expectedResponse.Equals(authorization.response); expectedResponse = '{0}'; authorization.response = '{1}'", expectedResponse, authorization.response);
                    throw HttpErrorHelper.unauthorized401FromOriginator(this);
                }

            }

            _idleSince = DateTime.Now;

        }


        public void authorise(String method, Authorization authorization)
        {
            authorise(method, authorization, null);
        }



        public void updateUsingAuthenticatedRequest(Authorization authorizationRequestHeader)
        {
            _cnonce = authorizationRequestHeader.cnonce;

            _cnoncesUsed[_cnonce] = _cnonce;

            _nc = authorizationRequestHeader.nc;

            _nonce = SecurityUtilities.generateNonce();

        }


        public WwwAuthenticate buildWwwAuthenticate()
        {
            WwwAuthenticate answer = new WwwAuthenticate();

            answer.nonce = _nonce;
            answer.opaque = _opaque;
            answer.realm = _usersRealm;

            return answer;
        }

        public AuthenticationInfo buildAuthenticationInfo(Authorization authorization, Entity responseEntity)
        {
            log.enteredMethod();

            AuthenticationInfo answer = new AuthenticationInfo();

            answer.cnonce = _cnonce;
            answer.nc = _nc;
            answer.nextnonce = _nonce;
            answer.qop = authorization.qop;

            /*
            * rspauth field
            */
            String ha1 = _registeredSubject.ha1();


            // from RFC-2617, section 3.2.3, we leave the method out ...   
            String ha2 = getHa2("", authorization.qop, authorization.uri, responseEntity);

            String unhashedRspauth = String.Format("{0}:{1}:{2:x8}:{3}:{4}:{5}",
                ha1, authorization.nonce, authorization.nc,
                authorization.cnonce, authorization.qop, ha2);

            String rspauth = SecurityUtilities.md5HashOfString(unhashedRspauth);
            answer.rspauth = rspauth;

            return answer;
        }


        public long idleTime()
        {
            long now = DateTime.Now.Ticks;

            long idleTicks = now - _idleSince.Ticks;

            // DateTime .Ticks Property ... 
            //A single tick represents one hundred nanoseconds or one ten-millionth of a second. There are 10,000 ticks in a millisecond.
            long idleSeconds = idleTicks / (10 * 1000 * 1000); // 

            return idleSeconds;
        }


    }
}
