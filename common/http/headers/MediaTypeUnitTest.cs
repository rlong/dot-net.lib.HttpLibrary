// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.http.headers
{
    [TestFixture]
    //[Category("current")]
    public class MediaTypeUnitTest
    {
        private static Log log = Log.getLog(typeof(MediaTypeUnitTest));

        [Test]
        public void test1()
        {
            log.enteredMethod();
        }


        [Test]
        public void testMultiPartFormData()
        {
            MediaType mediaType = MediaType.buildFromString("multipart/form-data; boundary=---------------------------114782935826962");
            Assert.AreEqual( "multipart", mediaType.Type);
            Assert.AreEqual( "form-data", mediaType.Subtype);
            Assert.AreEqual( "---------------------------114782935826962", mediaType.getParamaterValue( "boundary", null));
        }

        [Test]
        public void testTextPlain()
        {
            MediaType mediaType = MediaType.buildFromString("text/plain");
            Assert.AreEqual("text", mediaType.Type);
            Assert.AreEqual("plain", mediaType.Subtype);
        }

        //
        [Test]
        public void testApplicationXZipCompresssed()
        {
            MediaType mediaType = MediaType.buildFromString("application/x-zip-compressed");
            Assert.AreEqual("application", mediaType.Type);
            Assert.AreEqual("x-zip-compressed", mediaType.Subtype);
        }
    }
}
