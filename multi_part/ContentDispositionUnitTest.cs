// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using dotnet.lib.CoreAnnex.log;

namespace dotnet.lib.Http.multi_part
{

    [TestFixture]
    //[Category("current")]
    class ContentDispositionUnitTest
    {

        private static Log log = Log.getLog(typeof(ContentDispositionUnitTest));

        [Test]
        public void test1()
        {
            log.enteredMethod();
        }

        [Test]
        public void testMultiPartFormData()
        {
            ContentDisposition contentDisposition = ContentDisposition.buildFromString("form-data; name=\"datafile\"; filename=\"test.txt\"");
            Assert.AreEqual("form-data", contentDisposition.DispExtensionToken);
            Assert.AreEqual("datafile", contentDisposition.getDispositionParameter("name", null));
            Assert.AreEqual("test.txt", contentDisposition.getDispositionParameter("filename", null));
        }

    }
}
