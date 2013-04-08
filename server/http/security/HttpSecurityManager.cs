// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using jsonbroker.library.common.log;
using jsonbroker.library.common.http;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.http.headers;
using jsonbroker.library.common.http.headers.request;
using jsonbroker.library.common.http.headers.response;
using jsonbroker.library.common.security;

namespace jsonbroker.library.server.http.security
{
    public class HttpSecurityManager
    {

        private static Log log = Log.getLog(typeof(HttpSecurityManager));


        ///////////////////////////////////////////////////////////////////////
        // httpSecurityConfiguration
        private ServerSecurityConfiguration _securityConfiguration;

        public ServerSecurityConfiguration getSecurityConfiguration()
        {
            return _securityConfiguration;
        }

        ///////////////////////////////////////////////////////////////////////
        // 
        Dictionary<String, HttpSecuritySession> _unauthenticatedSessions;

        ///////////////////////////////////////////////////////////////////////
        // 
        Dictionary<String, HttpSecuritySession> _authenticatedSessions;

        ///////////////////////////////////////////////////////////////////////
        // 
        SubjectGroup _unregisteredSubjects;

        public SubjectGroup getUnregisteredSubjects()
        {
            return _unregisteredSubjects;
        }


        ///////////////////////////////////////////////////////////////////////

        public HttpSecurityManager(ServerSecurityConfiguration httpSecurityConfiguration)
        {

            _securityConfiguration = httpSecurityConfiguration;

            _unauthenticatedSessions = new Dictionary<string,HttpSecuritySession>();
            _authenticatedSessions = new Dictionary<string,HttpSecuritySession>();

            _unregisteredSubjects = new SubjectGroup();

        }

        private Subject registeredSubjectForAuthorizationRequestHeader(Authorization authorizationRequestHeader)
        {
            String realm = _securityConfiguration.getRealm();
            String authRealm = authorizationRequestHeader.realm;

            if (!realm.Equals(authRealm))
            {
                log.errorFormat("!realm.Equals(authRealm); realm = '{0}'; realm = '{1}'", realm, authRealm);
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            String clientUsername = authorizationRequestHeader.username;
            Subject client = _securityConfiguration.getClient(clientUsername);
            if (null == client)
            {
                log.errorFormat("null == client; clientUsername = '{0}'", clientUsername);
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }
            return client;

        }

        public void authenticateRequest(String method, Authorization authorizationRequestHeader, Entity entity)
        {
            String opaque = authorizationRequestHeader.opaque;
            log.debug(opaque, "opaque");

            if (null == opaque)
            {
                log.error("null == opaque");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            try
            {

                if (_unauthenticatedSessions.ContainsKey(opaque))
                {

                    HttpSecuritySession securitySession = _unauthenticatedSessions[opaque];
                    Subject registeredSubject = this.registeredSubjectForAuthorizationRequestHeader(authorizationRequestHeader);
                    securitySession.registeredSubject = registeredSubject;
                    securitySession.authorise(method, authorizationRequestHeader, entity);
                    securitySession.updateUsingAuthenticatedRequest(authorizationRequestHeader);
                    _authenticatedSessions[opaque] = securitySession;
                    _unauthenticatedSessions.Remove(opaque);

                    return;
                }

                if (_authenticatedSessions.ContainsKey(opaque))
                {

                    HttpSecuritySession securitySession = _authenticatedSessions[opaque];
                    securitySession.authorise(method, authorizationRequestHeader, entity);
                    securitySession.updateUsingAuthenticatedRequest(authorizationRequestHeader);

                    return;
                }

                log.errorFormat("bad opaque; opaque = '{0}'", opaque);
                throw HttpErrorHelper.unauthorized401FromOriginator(this);

            }
            catch (BaseException e) // if we catch a 401 ... clean up sesssions associated with the opaque
            {
                if (HttpStatus.UNAUTHORIZED_401 == e.FaultCode)
                {
                    _unauthenticatedSessions.Remove(opaque);
                    _authenticatedSessions.Remove(opaque);
                }

                // rethrow
                throw e;
            }

        }

        public void authenticateRequest(String method, Authorization authorization)
        {
            this.authenticateRequest(method, authorization, null);

        }


        public HttpHeader getHeaderForResponse(Authorization authorization, int responseStatusCode, Entity responseEntity)
        {
            if (null == authorization || 401 == responseStatusCode) // add a AuthenticateResponseHeader
            {
                HttpSecuritySession securitySession = new HttpSecuritySession(_securityConfiguration.getRealm());
                log.debug(securitySession.opaque, "securitySession.opaque");
                _unauthenticatedSessions[securitySession.opaque] = securitySession;
                WwwAuthenticate answer = securitySession.buildWwwAuthenticate();
                return answer;
            }
            else
            {
                String opaque = authorization.opaque;
                HttpSecuritySession securitySession = _authenticatedSessions[opaque];
                AuthenticationInfo answer = securitySession.buildAuthenticationInfo(authorization, responseEntity);
                return answer;
            }
        }

        public void addUnregisteredSubject(Subject unregisteredSubject)
        {
            log.enteredMethod();
            _unregisteredSubjects.addSubject(unregisteredSubject);
        }

        public void addRegisteredSubject(Subject registeredSubject)
        {
            log.enteredMethod();
            _securityConfiguration.addClient(registeredSubject);
        }

        public void removeSubject(String username)
        {

            _unregisteredSubjects.removeSubject(username);
            _securityConfiguration.removeClient(username);

            List<String> sessionsForRemoval = new List<String>();
            foreach (String opaque in _authenticatedSessions.Keys)
            {
                HttpSecuritySession authenticatedSession = _authenticatedSessions[opaque];
                Subject authenticatedSubject = authenticatedSession.registeredSubject;

                // authenticated session with the subject that we are removing 
                if (authenticatedSubject.Username.Equals(username))
                {
                    sessionsForRemoval.Add(opaque);
                }
            }

            foreach (String opaque in sessionsForRemoval)
            {
                log.info(opaque, "opaque");
                _authenticatedSessions.Remove(opaque);
            }
        }

        public void runCleanup()
        {
            log.enteredMethod();

            List<String> stale = new List<String>();

            /*
            * unauthenticated sessions 
            */
            foreach (String opaque in _unauthenticatedSessions.Keys)
            {
                HttpSecuritySession unauthenticatedSession = _unauthenticatedSessions[opaque];

                long idleTime = unauthenticatedSession.idleTime();

                if ((1 * 60) < idleTime)
                {
                    stale.Add(opaque);

                    if (Log.isDebugEnabled())
                    {
                        String message = String.Format("removing stale unauthenticatedSession session '{0}', age = {1}", opaque, idleTime);
                        log.debug(message);
                    }

                }

            }

            foreach (String opaque in stale)
            {
                _unauthenticatedSessions.Remove(opaque);
            }

            stale.Clear();

            /*
             * authenticated sessions 
             */

            foreach (String opaque in _authenticatedSessions.Keys)
            {
                HttpSecuritySession authenticatedSession = _authenticatedSessions[opaque];

                long idleTime = authenticatedSession.idleTime();

                if ((20 * 60) < idleTime)
                {
                    stale.Add(opaque);

                    if (Log.isDebugEnabled())
                    {
                        String message = String.Format("removing stale authenticated session '{0}', age = {1}", opaque, idleTime);
                        log.debug(message);
                    }

                }
            }

            foreach (String opaque in stale)
            {
                _authenticatedSessions.Remove(opaque);
            }

            stale.Clear();

            /*
            * registration requests 
            */

            List<String> staleRegistrationRequests = new List<String>();


            long now = DateTime.Now.Ticks;

            foreach (Subject unregisteredSubject in _unregisteredSubjects.subjects())
            {
                long elapsedTicks = now - unregisteredSubject.born.Ticks;

                // DateTime .Ticks Property ... 
                //A single tick represents one hundred nanoseconds or one ten-millionth of a second. There are 10,000 ticks in a millisecond.
                long objectAge = elapsedTicks / (10 * 1000 * 1000);

                if ((5 * 60) < objectAge)
                {
                    if (Log.isDebugEnabled())
                    {
                        String message = String.Format("removing stale registration request for user '{0}', age = {1}", unregisteredSubject.Username, objectAge);
                        log.debug(message);

                    }
                    staleRegistrationRequests.Add(unregisteredSubject.Username);

                }
            }

            foreach (String unregisteredUsername in staleRegistrationRequests)
            {
                _unregisteredSubjects.removeSubject(unregisteredUsername);
            }

        }



        public bool subjectHasAuthenticatedSession(Subject target)
        {
            foreach (String opaque in _authenticatedSessions.Keys)
            {
                HttpSecuritySession authenticatedSession = _authenticatedSessions[opaque];
                Subject candidate = authenticatedSession.registeredSubject;

                if (candidate.Username.Equals(target.Username))
                {
                    return true;
                }
            }

            return false;

        }

    

        public Subject approveSubject(String userName)
        {
            Subject approvedSubject = _unregisteredSubjects.getSubject(userName);
            _securityConfiguration.addClient(approvedSubject);
            _unregisteredSubjects.removeSubject(userName);
            return approvedSubject;
        }



    }
}
