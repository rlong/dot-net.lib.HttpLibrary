// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using jsonbroker.library.common.log;
using jsonbroker.library.common.http;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.network;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.client.http
{
    public class HttpDispatcher
    {
        //
        private static Log log = Log.getLog(typeof(HttpDispatcher));


        /////////////////////////////////////////////////////////
        // networkAddress
        private NetworkAddress _networkAddress;


        public HttpDispatcher(NetworkAddress networkAddress)
        {
            _networkAddress = networkAddress;
        }


        private int dispatch(HttpWebRequest webRequest, Authenticator authenticator, HttpResponseHandler responseHandler)
        {

            HttpWebResponse webResponse;
            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException e)
            {
                webResponse = (HttpWebResponse)e.Response;
            }

            Stream entityStream = null;
            try
            {
                entityStream = webResponse.GetResponseStream();

                WebHeaderCollection responseHeaders = webResponse.Headers;
                if (null != authenticator)
                {
                    authenticator.handleHttpResponseHeaders(responseHeaders);
                }

                int statusCode = (int)webResponse.StatusCode;

                if (statusCode >= 200 && statusCode < 300)
                {
                    // all is well
                    if (null == entityStream) // e.g. http 204 
                    {
                        responseHandler.handleResponseEntity(responseHeaders, null);
                    }
                    else
                    {
                        long contentLength = webResponse.ContentLength;
                        log.debug(contentLength, "contentLength");

                        StreamEntity responseEntity = new StreamEntity(contentLength, entityStream);
                        responseHandler.handleResponseEntity(responseHeaders, responseEntity);
                    }
                }
                return statusCode;

            }
            finally
            {
                if (null != entityStream)
                {
                    StreamUtilities.close(entityStream, false, this);
                }
            }
        }



        ////////////////////////////////////////////////////////////////////////////
            
        // authenticator can be null 
        private HttpWebRequest buildGetRequest(HttpRequestAdapter requestAdapter, Authenticator authenticator)
        {

            String host = _networkAddress.getHostAddress();
            int port = _networkAddress.Port;

            String requestUri = requestAdapter.RequestUri;


            String uri = String.Format("http://{0}:{1}{2}", host, port, requestUri);
            log.debug(uri, "uri");

            HttpWebRequest answer = (HttpWebRequest)HttpWebRequest.Create(uri);
            answer.Method = "GET";


            // extra headers ... 
            {
                Dictionary<String, String> requestHeaders = requestAdapter.RequestHeaders;
                foreach (KeyValuePair<String, String> kvp in requestHeaders)
                {
                    answer.Headers[kvp.Key] = kvp.Value;

                }
                
            }

            if (null != authenticator)
            {
                String authorization = authenticator.getRequestAuthorization(answer.Method, requestUri, null);
                log.debug(authorization, "authorization");
                if (null != authorization)
                {
                    answer.Headers["Authorization"] = authorization;
                }

            }

            // vvv http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx 

            answer.ServicePoint.Expect100Continue = false;

            // ^^^ http://msdn.microsoft.com/en-us/library/system.net.webrequest.getrequeststream.aspx#Y600 

            return answer;


        }


        ////////////////////////////////////////////////////////////////////////////

        public void get(HttpRequestAdapter requestAdapter, Authenticator authenticator, HttpResponseHandler responseAdapter)
        {
            HttpWebRequest request = buildGetRequest(requestAdapter, authenticator);
            int statusCode = dispatch(request, authenticator, responseAdapter);

            if (401 == statusCode)
            {
                request = buildGetRequest(requestAdapter, authenticator);
                statusCode = dispatch(request, authenticator, responseAdapter);
            }

            if (statusCode < 200 || statusCode > 299)
            {
                BaseException e = new BaseException(this, HttpStatus.getReason(statusCode));
                e.FaultCode = statusCode;
                String requestUri = requestAdapter.RequestUri;
                e.addContext("requestUri", requestUri);
                throw e;
            }

        }

        public void get(HttpRequestAdapter requestAdapter, HttpResponseHandler responseAdapter)
        {
            HttpWebRequest request = buildGetRequest(requestAdapter, null);
            int statusCode = dispatch(request, null, responseAdapter);

            if (statusCode < 200 || statusCode > 299)
            {
                BaseException e = new BaseException(this, HttpStatus.getReason(statusCode));
                e.FaultCode = statusCode;
                String requestUri = requestAdapter.RequestUri;
                e.addContext("requestUri", requestUri);
                throw e;
            }

        }


        ////////////////////////////////////////////////////////////////////////////

        // authenticator can be null
        private HttpWebRequest buildPostRequest(HttpRequestAdapter requestAdapter, Authenticator authenticator)
        {
            String host = _networkAddress.getHostAddress();
            int port = _networkAddress.Port;

            String requestUri = requestAdapter.RequestUri;


            String uri = String.Format("http://{0}:{1}{2}", host, port, requestUri);
            log.debug(uri, "uri");

            HttpWebRequest answer = (HttpWebRequest)HttpWebRequest.Create(uri);
            answer.Method = "POST";


            // extra headers ... 
            {
                Dictionary<String, String> requestHeaders = requestAdapter.RequestHeaders;
                foreach (KeyValuePair<String, String> kvp in requestHeaders)
                {
                    answer.Headers[kvp.Key] = kvp.Value;

                }

            }

            if (null != authenticator)
            {
                String authorization = authenticator.getRequestAuthorization(answer.Method, requestUri, null);
                log.debug(authorization, "authorization");
                if (null != authorization)
                {
                    answer.Headers["Authorization"] = authorization;
                }

            }

            Entity requestEntity = requestAdapter.RequestEntity;
            Stream destinationStream = answer.GetRequestStream();
            bool failed = true;
            try
            {
                // vvv http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx 

                answer.ServicePoint.Expect100Continue = false;

                Entity entity = requestAdapter.RequestEntity;
                StreamUtilities.write(entity.getContentLength(), entity.getContent(), answer.GetRequestStream());

                // ^^^ http://msdn.microsoft.com/en-us/library/system.net.webrequest.getrequeststream.aspx#Y600 

                failed = false;
            }
            finally
            {
                bool swallowExceptions = false;
                if( failed ) { 
                    swallowExceptions = true;
                }

                StreamUtilities.close(requestEntity.getContent(), swallowExceptions, this);
                StreamUtilities.close(destinationStream, swallowExceptions, this);
            }

            return answer;
        }



        public void post(HttpRequestAdapter requestAdapter, Authenticator authenticator, HttpResponseHandler responseAdapter)
        {
            HttpWebRequest request = buildPostRequest(requestAdapter, authenticator);
            int statusCode = dispatch(request, authenticator, responseAdapter);

            if (401 == statusCode)
            {
                request = buildGetRequest(requestAdapter, authenticator);
                statusCode = dispatch(request, authenticator, responseAdapter);
            }

            if (statusCode < 200 || statusCode > 299)
            {
                BaseException e = new BaseException(this, HttpStatus.getReason(statusCode));
                e.FaultCode = statusCode;
                String requestUri = requestAdapter.RequestUri;
                e.addContext("requestUri", requestUri);
                throw e;
            }

        }


        public void post(HttpRequestAdapter requestAdapter, HttpResponseHandler responseAdapter)
        {
            HttpWebRequest request = buildPostRequest(requestAdapter, null);
            int statusCode = dispatch(request, null, responseAdapter);

            if (statusCode < 200 || statusCode > 299)
            {
                BaseException e = new BaseException(this, HttpStatus.getReason(statusCode));
                e.FaultCode = statusCode;
                String requestUri = requestAdapter.RequestUri;
                e.addContext("requestUri", requestUri);
                throw e;
            }

        }


    }
}
