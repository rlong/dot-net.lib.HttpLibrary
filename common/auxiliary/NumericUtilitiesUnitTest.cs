// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.common.auxiliary
{

    [TestFixture]
    public class NumericUtilitiesUnitTest
    {
        private static Log log = Log.getLog(typeof(NumericUtilitiesUnitTest));

        [Test]
        public void test1()
        {
            log.debug("test1");
        }


        [Test]
        public void testParse_15_650912()
        {
            string doubleString = "15.650912";
            float f = NumericUtilities.parseFloat(doubleString);
            log.debug(f, "f");

        }

        [Test]
        public void testBadFloat()
        {
            string doubleString = "badFloat";
            try
            {
                NumericUtilities.parseFloat(doubleString);
                Assert.Fail();

            }
            catch (BaseException e)
            {
                log.warn(e);
                // as expected 
            }


        }

    }
}
