// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.CoreAnnex.json.output;
using dotnet.lib.CoreAnnex.defaults;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.CoreAnnex.net;
using dotnet.lib.Http.json_broker;
using dotnet.lib.Http.json_broker.client;
using dotnet.lib.Http.json_broker.server;
using dotnet.lib.Http.client;
using dotnet.lib.Http.authentication;
using dotnet.lib.CoreAnnex.json;

namespace dotnet.lib.Http.json_broker.test
{
    public class IntegrationTestUtilities
    {
        private static Log log = Log.getLog(typeof(IntegrationTestUtilities));

        private static JsonObject _internalServerConfig = null;
        private static JsonObject _externalServerConfig = null;

        private static IntegrationTestUtilities _instance;

        ///////////////////////////////////////////////////////////////////////
        // internalWebServer
        private IntegrationTestServer _internalWebServer;

        //public IntegrationTestServer InternalWebServer
        //{
        //    get { return _internalWebServer; }
        //    set { _internalWebServer = value; }
        //}


        ///////////////////////////////////////////////////////////////////////
        // networkAddress
        private NetworkAddress _networkAddress;

        //public NetworkAddress NetworkAddress
        //{
        //    get { return _networkAddress; }
        //    set { _networkAddress = value; }
        //}


        static IntegrationTestUtilities()
        {
            Defaults defaults = DefaultsHelper.GetDefaults("jsonbroker.IntegrationTestUtilities");
            _internalServerConfig = defaults.GetJsonObject("internalServerConfig", null);
            _externalServerConfig = defaults.GetJsonObject("externalServerConfig", null);
        }


        private IntegrationTestUtilities()
        {
        }


        public static IntegrationTestUtilities GetInstance()
        {
            if (null != _instance)
            {
                return _instance;
            }

            _instance = new IntegrationTestUtilities();
            _instance.Setup();
            return _instance;

        }

        bool ConfiguredForExternalServer()
        {
            if (null != _externalServerConfig)
            {
                return true;
            }
            return false;
        }

        bool ConfiguredForInternalServer()
        {
            if (null != _internalServerConfig)
            {
                return true;
            }
            return false;
        }

        private void Setup()
        {
            log.enteredMethod();


            if (null != _internalServerConfig)
            {
                if (null != _internalWebServer)
                {
                    return;
                }
                _internalWebServer = new IntegrationTestServer();
                _internalWebServer.Start();

                _networkAddress = new NetworkAddress("127.0.0.1", 8081);

                return;

            }

            if (null != _externalServerConfig)
            {
                String hostIp4Address = _externalServerConfig.GetString("hostIp4Address");
                _networkAddress = new NetworkAddress(hostIp4Address, 8081);

                return;
            }

            // else co-located ...
            return;
        }

        private void Teardown() 
        {
            log.enteredMethod();

            if (null != _internalWebServer)
            {
                _internalWebServer.Stop();
            }
        }

        private ServiceHttpProxy buildServiceHttpProxy(bool useAuthService)
        {
            Authenticator authenticator = null;

            if (useAuthService)
            {

                SecurityConfiguration securityConfiguration = SecurityConfiguration.TEST;
                authenticator = new Authenticator(false, securityConfiguration);


            }

            HttpDispatcher httpDispatcher = new HttpDispatcher(_networkAddress);

            ServiceHttpProxy answer = new ServiceHttpProxy(httpDispatcher, authenticator);
            return answer;

        }

        public Service wrapService(DescribedService service)
        {
            if (null != _internalServerConfig)
            {
                // add the service to the running webserver ...
                _internalWebServer.AddService(service);

                bool useAuthService = _internalServerConfig.GetBoolean("useAuthService", false);
                return this.buildServiceHttpProxy(useAuthService);
            }

            if (null != _externalServerConfig)
            {
                bool useAuthService = _externalServerConfig.GetBoolean("useAuthService", false);
                return this.buildServiceHttpProxy(useAuthService);
            }

            // else co-located ...
            return service;
        }
    }
}
