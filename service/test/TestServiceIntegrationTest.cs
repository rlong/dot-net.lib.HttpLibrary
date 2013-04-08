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

namespace jsonbroker.library.service.test
{


    [TestFixture]
    [Category("integration")]
    public class TestServiceIntegrationTest
    {

        private static Log log = Log.getLog(typeof(TestServiceIntegrationTest));

        private static TestServiceProxy _proxy;

        static TestServiceIntegrationTest() 
        {
            DescribedService namedservice = new TestService();

            Service service = IntegrationTestUtilities.setupService(IntegrationTestUtilities.TestType.EMBEDDED_AUTH_SERVER, namedservice);

            _proxy = new TestServiceProxy(service);

        }

        [Test]
        public void test1()
        {
            log.debug("test1");
        }

        [Test]
        public void testPing()
        {
            _proxy.ping();
        }

        [TestFixtureTearDown]
        public void fixtureTearDown()
        {
            log.enteredMethod();
        }

    }
}
