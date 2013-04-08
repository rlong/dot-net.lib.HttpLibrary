// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.security;
using jsonbroker.library.server.broker;
using jsonbroker.library.service.test;
using jsonbroker.library.common.log;
using jsonbroker.library.common.network;
using jsonbroker.library.client.http;
using jsonbroker.library.client.broker;
using jsonbroker.library.common.work;
using jsonbroker.library.server.http;
using System.Threading;
using jsonbroker.library.server.http.security;
using jsonbroker.library.server.http.reqest_handler;

namespace jsonbroker.library.common.auxiliary
{
    public class IntegrationTestUtilities
    {
        private static readonly Log log = Log.getLog(typeof(IntegrationTestUtilities));

        public enum TestType { CO_LOCATED, EMBEDDED_OPEN_SERVER, EMBEDDED_AUTH_SERVER, EXTERNAL_OPEN_SERVER, EXTERNAL_AUTH_SERVER };



        private static void startServer(RequestHandler httpProcessor)
        {
            log.info("setting up work manager ... ");
            WorkManager.start();

            RootProcessor rootProcessor = new RootProcessor();
            log.info("setting up web '/_dynamic_/open' ... ");
            rootProcessor.addHttpProcessor(httpProcessor);

            log.info("starting web server ... ");
            WebServer webServer = new WebServer( rootProcessor);
            webServer.start();

            log.info("sleeping while waiting for server startup ... ");
            Thread.Sleep(500); // 1/2 seconds
            log.info("... awake again");

        }


        private static OpenRequestHandler buildOpenProcessor(DescribedService openService)
        {
            OpenRequestHandler answer = new OpenRequestHandler();
            ServicesRegistery servicesRegistery = new ServicesRegistery();

            servicesRegistery.addService(openService);
            if( !(openService is TestService) ) {
                servicesRegistery.addService(new TestService());
            }

            ////////////////////////////////////////////////////////////////////
            ServicesRequestHandler synchronousServiceProcessor = new ServicesRequestHandler(servicesRegistery);
            answer.addHttpProcessor(synchronousServiceProcessor);

            return answer;
        }


        public static HttpSecurityManager buildHttpSecurityManager()
        {
            HttpSecurityManager answer = new HttpSecurityManager(SecurityConfiguration.TEST);
            return answer;
        }

        private static AuthProcessor buildAuthProcessor(DescribedService authService)
        {
            AuthProcessor answer = new AuthProcessor(buildHttpSecurityManager());

            ServicesRegistery servicesRegistery = new ServicesRegistery();
            servicesRegistery.addService(authService);

            if (!(authService is TestService))
            {
                servicesRegistery.addService(new TestService());
            }

            ////////////////////////////////////////////////////////////////////
            ServicesRequestHandler synchronousServiceProcessor = new ServicesRequestHandler(servicesRegistery);
            answer.addHttpProcessor(synchronousServiceProcessor);

            return answer;

        }

        private static Service buildProxy(TestType testType)
        {
            log.info("setting up open proxy ... ");

            NetworkAddress networkAddress = new NetworkAddress("127.0.0.1", 8081);
            HttpDispatcher httpDispatcher = new HttpDispatcher(networkAddress);

            Service answer;

            if (TestType.EMBEDDED_AUTH_SERVER == testType || TestType.EXTERNAL_AUTH_SERVER == testType)
            {
                Authenticator authenticator = new Authenticator(false, SecurityConfiguration.TEST);
                answer = new ServiceHttpProxy(httpDispatcher, authenticator);
            }
            else
            {
                answer = new ServiceHttpProxy(httpDispatcher);
            }

            return answer;
        }

        public static Service setupService(TestType testType, DescribedService service)
        {

            if (testType == TestType.CO_LOCATED)
            {
                return service;
            }
            if (testType == TestType.EMBEDDED_OPEN_SERVER)
            {
                OpenRequestHandler openProcessor = buildOpenProcessor(service);
                startServer(openProcessor);
            }
            if (testType == TestType.EMBEDDED_AUTH_SERVER)
            {
                AuthProcessor authProcessor = buildAuthProcessor(service);
                startServer(authProcessor);
            }

            return buildProxy(testType);
        }

    }
}
