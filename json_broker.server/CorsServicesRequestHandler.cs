// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.Http.json_broker.server;
using dotnet.lib.Http;
using dotnet.lib.Http.server;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.CoreAnnex.auxiliary;

namespace dotnet.lib.Http.json_broker.server
{
    public class CorsServicesRequestHandler : RequestHandler
    {
        private static Log log = Log.getLog(typeof(CorsServicesRequestHandler));


        private ServicesRegistery _servicesRegistery;


        public CorsServicesRequestHandler()
        {
            _servicesRegistery = new ServicesRegistery();
        }

        public CorsServicesRequestHandler(ServicesRegistery servicesRegistery)
        {
            _servicesRegistery = servicesRegistery;
        }

        public void addService(DescribedService service)
        {
            log.infoFormat("adding service '{0}'", service.getServiceDescription().getServiceName());

            _servicesRegistery.addService(service);
        }


        private HttpResponse buildOptionsResponse(HttpRequest request)
        {
            HttpResponse answer = new HttpResponse(HttpStatus.NO_CONTENT_204);

            // vvv http://www.w3.org/TR/cors/#access-control-allow-methods-response-header
            answer.putHeader("Access-Control-Allow-Methods", "OPTIONS, POST");
            // ^^^ http://www.w3.org/TR/cors/#access-control-allow-methods-response-header


            String accessControlAllowOrigin = request.getHttpHeader("origin");
            if (null == accessControlAllowOrigin)
            {
                accessControlAllowOrigin = "*";
            }
            answer.putHeader("Access-Control-Allow-Origin", accessControlAllowOrigin);

            String accessControlRequestHeaders = request.getHttpHeader("access-control-request-headers");
            if (null != accessControlRequestHeaders)
            {
                answer.putHeader("Access-Control-Allow-Headers", accessControlRequestHeaders);
            }
            
            return answer;
        }

        public HttpResponse processRequest(HttpRequest request)
        {

            // vvv `chrome` (and possibly others) preflights any CORS requests
            if (HttpMethod.OPTIONS == request.Method)
            {
                return buildOptionsResponse(request);
            }
            // ^^^ `chrome` (and possibly others) preflights any CORS requests


            HttpResponse answer = ServicesRequestHandler.processPostRequest(_servicesRegistery, request);

            // vvv without echoing back the 'origin' for CORS requests, chrome (and possibly others) complains "Origin http://localhost:8081 is not allowed by Access-Control-Allow-Origin."
            {
                String origin = request.getHttpHeader("origin");
                if (null != origin)
                {
                    answer.putHeader("Access-Control-Allow-Origin", origin);
                }
            }
            // ^^^ without echoing back the 'origin' for CORS requests, chrome (and possibly others) complains "Origin http://localhost:8081 is not allowed by Access-Control-Allow-Origin."

            return answer;
        }

        public String getProcessorUri()
        {
            return "/cors_services";
        }

    }
}
