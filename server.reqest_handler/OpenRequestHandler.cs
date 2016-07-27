// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.Http;
using dotnet.lib.CoreAnnex.log;

namespace dotnet.lib.Http.server.reqest_handler
{
    public class OpenRequestHandler : RequestHandler
    {


        private static Log log = Log.getLog(typeof(OpenRequestHandler));

        public static readonly String REQUEST_URI = "/_dynamic_/open";

        private Dictionary<String, RequestHandler> _processors;

        public OpenRequestHandler()
        {
            _processors = new Dictionary<String, RequestHandler>();
        }


        public void AddRequestHandler(RequestHandler processor)
        {
            String requestUri = REQUEST_URI + processor.getProcessorUri();
            log.debug(requestUri, "requestUri");
            _processors[requestUri] = processor;
        }


        private RequestHandler GetRequestHandler(String requestUri)
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


        public HttpResponse processRequest(HttpRequest request)
        {

            String requestUri = request.RequestUri;
            RequestHandler httpProcessor = GetRequestHandler(requestUri);


            if (null == httpProcessor)
            {
                log.errorFormat("null == httpProcessor; requestUri = {0}", requestUri);
                throw HttpErrorHelper.notFound404FromOriginator(this);
            }

            return httpProcessor.processRequest(request);

        }

    }
}
