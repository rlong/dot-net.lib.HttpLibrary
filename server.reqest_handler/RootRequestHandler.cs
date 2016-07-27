// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.CoreAnnex.log;

namespace dotnet.lib.Http.server.reqest_handler
{
    public class RootRequestHandler : RequestHandler
    {

        private static Log log = Log.getLog(typeof(RootRequestHandler));


        ////////////////////////////////////////////////////////////////////////////
        //
        private List<RequestHandler> _httpProcessors;


        ////////////////////////////////////////////////////////////////////////////
        //
        private RequestHandler _defaultProcessor;

        ////////////////////////////////////////////////////////////////////////////
        //
        public RootRequestHandler()
        {
            _httpProcessors = new List<RequestHandler>();
        }


        public RootRequestHandler(RequestHandler defaultProcessor)
        {
            _httpProcessors = new List<RequestHandler>();
            _defaultProcessor = defaultProcessor;
        }


        public void AddRequestHandler(RequestHandler httpProcessor)
        {
            String requestUri = httpProcessor.getProcessorUri();
            log.debug(requestUri, "requestUri");

            _httpProcessors.Add(httpProcessor);
        }


        public String getProcessorUri()
        {
            return "/";
        }

        public HttpResponse processRequest(HttpRequest request)
        {
            String requestUri = request.RequestUri;

            foreach (RequestHandler httpProcessor in _httpProcessors)
            {
                String processorUri = httpProcessor.getProcessorUri();
                if (0 == requestUri.IndexOf(processorUri))
                {
                    return httpProcessor.processRequest(request);
                }
            }

            if (null != _defaultProcessor)
            {
                return _defaultProcessor.processRequest(request);
            }

            log.errorFormat("bad requestUri; requestUri = '{0}'", requestUri);
            throw HttpErrorHelper.notFound404FromOriginator(this);

        }

    }
}
