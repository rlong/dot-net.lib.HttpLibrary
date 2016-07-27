// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.Http.json_broker;
using dotnet.lib.Http.json_broker.client;
using dotnet.lib.Http.json_broker.server;
using dotnet.lib.Http.json_broker.test;
using dotnet.lib.CoreAnnex.auxiliary;
using dotnet.lib.CoreAnnex.exception;

namespace dotnet.lib.Http.json_broker.service.test
{


    [TestFixture]
    [Category("integration")]
    public class TestServiceIntegrationTest
    {

        private static Log log = Log.getLog(typeof(TestServiceIntegrationTest));


        [Test]
        public void test1()
        {
            log.debug("test1");
        }


        TestProxy BuildProxy()
        {
            TestService describedService = new TestService();
            Service service = IntegrationTestUtilities.GetInstance().wrapService(describedService);

            TestProxy answer = new TestProxy(service);
            return answer;
        }


        [Test]
        public void testPing()
        {
            TestProxy proxy = BuildProxy();
            proxy.ping();
        }

        [Test]
        public void testRaiseError()
        {
            log.enteredMethod();

            TestProxy proxy = BuildProxy();

            try
            {
                proxy.raiseError();
                Assert.Fail("'BaseException' should have been thrown");
            }
            catch (BaseException e)
            {
                Assert.AreEqual("jsonbroker.TestService.RAISE_ERROR", e.ErrorDomain);
            }            
        }


		// [TestFixtureTearDown] 
		[OneTimeTearDown]
        public void fixtureTearDown()
        {
            log.enteredMethod();
        }

    }
}
