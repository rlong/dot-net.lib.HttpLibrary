// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.Http;
using dotnet.lib.Http.json_broker;
using dotnet.lib.CoreAnnex.exception;
using dotnet.lib.Http.json_broker.test;
using dotnet.lib.Http.headers.request;
using dotnet.lib.Http.headers;
using dotnet.lib.Http.server;
using dotnet.lib.CoreAnnex.auxiliary;



namespace dotnet.lib.Http.authentication.server
{
    class AuthIntRequestHandler : RequestHandler
    {
        private static Log log = Log.getLog(typeof(AuthIntRequestHandler));

        private static readonly String REQUEST_URI = "/_dynamic_/auth-int";

        /////////////////////////////////////////////////////////
        private Dictionary<String, RequestHandler> _processors;

        /////////////////////////////////////////////////////////
        // securityManager
        private HttpSecurityManager _securityManager;

        protected HttpSecurityManager SecurityManager
        {
            get { return _securityManager; }
            set { _securityManager = value; }
        }


        public AuthIntRequestHandler(HttpSecurityManager securityManager)
        {
            _processors = new Dictionary<String, RequestHandler>();
            _securityManager = securityManager;
        }



        public void addHttpProcessor(RequestHandler processor)
        {
            String requestUri = REQUEST_URI + processor.getProcessorUri();
            log.info(requestUri, "requestUri");
            _processors[requestUri] = processor;
        }


        private RequestHandler getHttpProcessor(String requestUri)
        {
            log.debug(requestUri, "requestUri");

            int indexOfQuestionMark = requestUri.IndexOf('?');
            if (-1 != indexOfQuestionMark)
            {
                requestUri = requestUri.Substring(0, indexOfQuestionMark);
            }

            if (!_processors.ContainsKey(requestUri))
            {
                return null;
            }

            RequestHandler answer = _processors[requestUri];
            return answer;

        }


        public String getProcessorUri()
        {
            return REQUEST_URI;
        }


        private Authorization getAuthorizationRequestHeader(HttpRequest request)
        {

            Authorization answer = null;


            String authorization = request.getHttpHeader("Authorization");
            if (null == authorization)
            {
                log.error("null == authorization");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            answer = Authorization.buildFromString(authorization);

            if (!"auth-int".Equals(answer.qop))
            {
                log.errorFormat("!\"auth-int\".Equals(answer.qop); answer.qop = '{0}'", answer.qop);
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            return answer;
        }



        // can return null
        private static DataEntity toDataEntity(Entity entity)
        {
            if (null == entity)
            {
                return null;
            }

            if (entity is DataEntity)
            {
                return (DataEntity)entity;
            }

            // limit of 64K
            if (entity.getContentLength() > 64 * 1024)
            {
                log.errorFormat("entity.getContentLength() > 64 * 1024; entity.getContentLength() = {0}", entity.getContentLength());
                throw HttpErrorHelper.requestEntityTooLarge413FromOriginator(typeof(AuthIntRequestHandler));
            }

            Data data = new Data(entity.getContent(), (int)entity.getContentLength());
            DataEntity answer = new DataEntity(data);
            return answer;
        }



        public HttpResponse processRequest(HttpRequest request)
        {

            RequestHandler httpProcessor;
            {
                String requestUri = request.RequestUri;
                httpProcessor = getHttpProcessor(requestUri);
                if (null == httpProcessor)
                {
                    log.errorFormat("null == httpProcessor; requestUri = {0}", requestUri);
                    throw HttpErrorHelper.notFound404FromOriginator(this);
                }
            }

            DataEntity entity = toDataEntity(request.Entity);

            // we *may* have just consumed the entity data from a stream ... we need to reset the entity in the request
            request.Entity = entity;

            HttpResponse answer = null;
            Authorization authorization = null;

            try
            {
                authorization = getAuthorizationRequestHeader(request);
                _securityManager.authenticateRequest(request.Method.Name, authorization, entity);
                answer = httpProcessor.processRequest(request);
                return answer;
            }
            catch (Exception e)
            {
                log.warn(e.Message);
                answer = HttpErrorHelper.toHttpResponse(e);
                return answer;
            }
            finally
            {
                HttpHeader header = _securityManager.getHeaderForResponse(authorization, answer.Status, answer.Entity);
                answer.putHeader(header.getName(), header.getValue());
            }
        }
    }
}
