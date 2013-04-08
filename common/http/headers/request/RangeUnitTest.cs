// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using jsonbroker.library.common.log;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.common.http.headers.request
{
    [TestFixture]
    class RangeUnitTest
    {
        private static Log log = Log.getLog(typeof(RangeUnitTest));

        [Test]
        public void test1()
        {
            log.enteredMethod();
        }


        [Test]
        // from http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35.1 
        public void testHyphen500()
        {

            Range range = Range.buildFromString("bytes=-500");
            Assert.AreEqual(-500, range.FirstBytePosition.Value);
            Assert.IsNull(range.LastBytePosition);

            Assert.AreEqual("bytes 9500-9999/10000", range.ToContentRange(10000));
            Assert.AreEqual(9500, range.getSeekPosition(10000));
            Assert.AreEqual(500, range.getContentLength(10000));

        }

        [Test]
        // from http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35.1
        public void test9500Hyphen()
        {
            Range range = Range.buildFromString("bytes=9500-");
            Assert.AreEqual(9500, range.FirstBytePosition.Value);
            Assert.IsNull(range.LastBytePosition);

            Assert.AreEqual("bytes 9500-9999/10000", range.ToContentRange(10000));
            Assert.AreEqual(9500, range.getSeekPosition(10000));
            Assert.AreEqual(500, range.getContentLength(10000));

        }

        [Test]
        // from http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35.1
        public void test500to999()
        {

            Range range = Range.buildFromString("bytes=500-999");
            Assert.AreEqual(500, range.FirstBytePosition.Value);
            Assert.AreEqual(999, range.LastBytePosition.Value);

            Assert.AreEqual("bytes 500-999/10000", range.ToContentRange(10000));
            Assert.AreEqual(500, range.getSeekPosition(10000));
            Assert.AreEqual(500, range.getContentLength(10000));

        }

        [Test]
        public void test0to499()
        {

            Range range = Range.buildFromString("bytes=0-499");
            Assert.AreEqual(0, range.FirstBytePosition.Value);
            Assert.AreEqual(499, range.LastBytePosition.Value);

            Assert.AreEqual("bytes 0-499/10000", range.ToContentRange(10000));
            Assert.AreEqual(0, range.getSeekPosition(10000));
            Assert.AreEqual(500, range.getContentLength(10000));

        }

        [Test]
        public void test0Hyphen()
        {

            Range range = Range.buildFromString("bytes=0-");
            Assert.AreEqual(0, range.FirstBytePosition.Value);
            Assert.IsNull(range.LastBytePosition);

            Assert.AreEqual("bytes 0-9999/10000", range.ToContentRange(10000));
            Assert.AreEqual(0, range.getSeekPosition(10000));
            Assert.AreEqual(10000, range.getContentLength(10000));
        }

        [Test]
        public void test0Hyphen1()
        {

            Range range = Range.buildFromString("bytes=0-1");
            Assert.AreEqual(0, range.FirstBytePosition.Value);
            Assert.AreEqual(1, range.LastBytePosition.Value);

            Assert.AreEqual("bytes 0-1/10000", range.ToContentRange(10000));
            Assert.AreEqual(0, range.getSeekPosition(10000));
            Assert.AreEqual(2, range.getContentLength(10000));
        }

        [Test]
        public void test0Hyphen0()
        {

            Range range = Range.buildFromString("bytes=0-0");
            Assert.AreEqual(0, range.FirstBytePosition.Value);
            Assert.AreEqual(0, range.LastBytePosition.Value);

            Assert.AreEqual("bytes 0-0/10000", range.ToContentRange(10000));
            Assert.AreEqual(0, range.getSeekPosition(10000));
            Assert.AreEqual(1, range.getContentLength(10000));
        }

        [Test]
        public void test0Hyphen9999()
        {

            Range range = Range.buildFromString("bytes=0-9999");
            Assert.AreEqual(0, range.FirstBytePosition.Value);
            Assert.AreEqual(9999, range.LastBytePosition.Value);

            Assert.AreEqual("bytes 0-9999/10000", range.ToContentRange(10000));
            Assert.AreEqual(0, range.getSeekPosition(10000));
            Assert.AreEqual(10000, range.getContentLength(10000));
        }

        [Test]
        public void testBadRange1()
        {

            try
            {
                Range.buildFromString("bytes=1-2-3");
                Assert.Fail(); // bad 
            }
            catch (BaseException)
            {
                // good 
            }
        }

        [Test]
        public void testUnhandledRange1()
        {

            try
            {
                log.info("valid but unhandled scenario");

                // from http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35.1
                Range.buildFromString("bytes=0-0,-1");
                Assert.Fail(); // bad 
            }
            catch (BaseException)
            {
                // good 
            }
        }

    }
}



