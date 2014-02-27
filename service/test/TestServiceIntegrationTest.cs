// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using jsonbroker.library.common.log;
using jsonbroker.library.server.broker;
using jsonbroker.library.common.auxiliary;
using jsonbroker.library.common.exception;
using jsonbroker.library.test;

namespace jsonbroker.library.service.test
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


        [TestFixtureTearDown]
        public void fixtureTearDown()
        {
            log.enteredMethod();
        }

    }
}
