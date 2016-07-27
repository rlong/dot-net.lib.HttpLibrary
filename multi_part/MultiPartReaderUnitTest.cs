// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.CoreAnnex.auxiliary;
using System.IO;
using dotnet.lib.Http.headers;

namespace dotnet.lib.Http.multi_part
{

    [TestFixture]
    //[Category("current")]
    class MultiPartReaderUnitTest
    {

        private static Log log = Log.getLog(typeof(MultiPartReaderUnitTest));

        [Test]
        public void test1()
        {
            log.enteredMethod();
        }


        private static Entity buildEntity(String multipartString)
        {
            byte[] multiPartData = StringHelper.ToUtfBytes(multipartString.ToString());

            Stream multiPartStream = new MemoryStream(multiPartData, 0, multiPartData.Length);

            StreamEntity answer = new StreamEntity(multiPartData.Length, multiPartStream);
            return answer;

        }

        private static Entity buildEntity(String[] multipartLines)
        {

            StringBuilder multipartString = new StringBuilder();
            foreach( String line in multipartLines ) 
            { 
                multipartString.Append( line );
                multipartString.Append("\r\n");
            }

            return buildEntity(multipartString.ToString());


        }



        [Test]
        public void testDelimiterDetection()
        {

            log.enteredMethod();

            String[] multipartLines = {
                                          "",
                                          "-----------------------------114782935826962",
                                         "Content-Disposition: form-data; name=\"datafile\"; filename=\"test.txt\"",
                                         "Content-Type: text/plain",
                                         "",
                                         "0123",
                                         "4567",
                                         "89ab",
                                         "cdef",
                                         "-----------------------------114782935826962--",
                                     };

            Entity entity = buildEntity(multipartLines);
            MultiPartReader reader = new MultiPartReader("---------------------------114782935826962", entity);

            DelimiterIndicator indicator = reader.skipToNextDelimiterIndicator();
            Assert.NotNull(indicator);
            Assert.IsInstanceOf<DelimiterFound>(indicator);
            DelimiterFound delimiterFound = (DelimiterFound)indicator;

            Assert.AreEqual(0, delimiterFound.StartOfDelimiter);
            Assert.False(delimiterFound.IsCloseDelimiter);

            // Content-Disposition
            MutableData stringBuffer = new MutableData();
            String contentDisposition = reader.ReadLine(stringBuffer);
            Assert.AreEqual(multipartLines[2], contentDisposition);

            // Content-Type
            String contentType = reader.ReadLine(stringBuffer);
            Assert.AreEqual(multipartLines[3], contentType);

            // empty line
            String emptyLine = reader.ReadLine(stringBuffer);
            Assert.AreEqual("", emptyLine);

            // ending indicator
            indicator = reader.skipToNextDelimiterIndicator();
            Assert.NotNull(indicator);
            Assert.IsInstanceOf<DelimiterFound>(indicator);
            delimiterFound = (DelimiterFound)indicator;
            Assert.True(delimiterFound.IsCloseDelimiter);

        }


        class TestPartHandler : PartHandler
        {
            private static Log log = Log.getLog(typeof(TestPartHandler));

            internal MutableData _data = new MutableData();

            public void HandleHeader(String name, String value)
            {

                //log.debug(name, "name");
                //log.debug(value, "value");
            }

            public void HandleBytes(byte[] bytes, int offset, int length)
            {
                _data.Append(bytes, offset, length);
            }

            public void HandleException(Exception e)
            {
                log.error(e);
            }

            public void PartCompleted()
            {
            }

        }

        class TestMultiPartHandler : MultiPartHandler
        {
            private static Log log = Log.getLog(typeof(TestMultiPartHandler));

            internal List<TestPartHandler> _partHandlers = new List<TestPartHandler>();
            internal bool _foundCloseDelimiter = false;

            public PartHandler FoundPartDelimiter()
            {

                TestPartHandler answer = new TestPartHandler();
                _partHandlers.Add(answer);
                return answer;
            }


            public void HandleException(Exception e)
            {
                log.error(e);
            }

            public void FoundCloseDelimiter()
            {
                _foundCloseDelimiter = true;
            }

        };


        [Test]
        public void testSingleMultiPartForm()
        {

            log.enteredMethod();

            String[] multipartLines = {
                                          "",
                                          "-----------------------------114782935826962",
                                         "Content-Disposition: form-data; name=\"datafile\"; filename=\"test.txt\"",
                                         "Content-Type: text/plain",
                                         "",
                                         "0123",
                                         "4567",
                                         "89ab",
                                         "cdef",
                                         "-----------------------------114782935826962--",
                                     };

            Entity entity = buildEntity(multipartLines);
            MultiPartReader reader = new MultiPartReader("---------------------------114782935826962", entity);

            TestMultiPartHandler testMultiPartHandler = new TestMultiPartHandler();
            reader.Process(testMultiPartHandler);
            Assert.True(testMultiPartHandler._foundCloseDelimiter);

            Assert.AreEqual(1, testMultiPartHandler._partHandlers.Count);

            TestPartHandler firstTestPartHandler = testMultiPartHandler._partHandlers[0];
            Assert.AreEqual(16 + 6, firstTestPartHandler._data.Length);
            String expectedContent = "0123\r\n4567\r\n89ab\r\ncdef";
            String actualContent = DataHelper.ToUtf8String(firstTestPartHandler._data);
            Assert.AreEqual(expectedContent, actualContent );
        }

        [Test]
        public void TestDoubleMultiPartForm()
        {

            log.enteredMethod();

            String[] multipartLines = {
                                          "",
                                          "-----------------------------114782935826962",
                                         "Content-Disposition: form-data; name=\"datafile\"; filename=\"test1.txt\"",
                                         "Content-Type: text/plain",
                                         "",
                                         "0123",
                                         "4567",
                                         "89ab",
                                         "cdef",
                                         "-----------------------------114782935826962",
                                         "Content-Disposition: form-data; name=\"datafile\"; filename=\"test2.txt\"",
                                         "Content-Type: text/plain",
                                         "",
                                         "cdef",
                                         "89ab",
                                         "4567",
                                         "0123",
                                         "-----------------------------114782935826962--",
                                     };

            Entity entity = buildEntity(multipartLines);
            MultiPartReader reader = new MultiPartReader("---------------------------114782935826962", entity);

            TestMultiPartHandler testMultiPartHandler = new TestMultiPartHandler();
            reader.Process(testMultiPartHandler);
            Assert.True(testMultiPartHandler._foundCloseDelimiter);

            Assert.AreEqual(2, testMultiPartHandler._partHandlers.Count);

            {
                TestPartHandler firstTestPartHandler = testMultiPartHandler._partHandlers[0];
                Assert.AreEqual(16 + 6, firstTestPartHandler._data.Length);
                String expectedContent = "0123\r\n4567\r\n89ab\r\ncdef";
                String actualContent = DataHelper.ToUtf8String(firstTestPartHandler._data);
                Assert.AreEqual(expectedContent, actualContent);
            }

            {
                TestPartHandler secondTestPartHandler = testMultiPartHandler._partHandlers[1];
                Assert.AreEqual(16 + 6, secondTestPartHandler._data.Length);
                String expectedContent = "cdef\r\n89ab\r\n4567\r\n0123";
                String actualContent = DataHelper.ToUtf8String(secondTestPartHandler._data);
                Assert.AreEqual(expectedContent, actualContent);
            }
        }



        static String buildAttachment( String contentSource, int length)
        {

            StringBuilder answer = new StringBuilder();

            int remaining = length;
            while (remaining >= contentSource.Length)
            {
                answer.Append(contentSource);
                remaining -= contentSource.Length;
            }

            int modulus = length % contentSource.Length;

            for (int j = 0; j < modulus; j++)
            {
                answer.Append(contentSource[j]);
            }

            return answer.ToString();
        }


        static void testSingleAttachment(String boundary, String contentSource, int length) 
        {

            if (0 == length)
            {
                log.debugFormat(contentSource, "contentSource");
            }
            log.debug(length, "length");

            String attachment = buildAttachment(contentSource, length);
            Assert.AreEqual(length, attachment.Length);
            
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("\r\n--");
            stringBuilder.Append(boundary);
            stringBuilder.Append("\r\n");


            stringBuilder.Append("Content-Disposition: form-data; name=\"datafile\"; filename=\"test1.txt\"\r\n");
            stringBuilder.Append("Content-Type: text/plain\r\n");
            stringBuilder.Append("\r\n");

            stringBuilder.Append(attachment);

            stringBuilder.Append("\r\n--");
            stringBuilder.Append(boundary);
            stringBuilder.Append("--\r\n");


            Entity entity = buildEntity(stringBuilder.ToString());
            MultiPartReader reader = new MultiPartReader(boundary, entity);

            TestMultiPartHandler testMultiPartHandler = new TestMultiPartHandler();
            reader.Process(testMultiPartHandler);
            Assert.True(testMultiPartHandler._foundCloseDelimiter);

            Assert.AreEqual(1, testMultiPartHandler._partHandlers.Count);

            TestPartHandler firstTestPartHandler = testMultiPartHandler._partHandlers[0];
            Assert.AreEqual(attachment.Length, firstTestPartHandler._data.Length);
            String actualContent = DataHelper.ToUtf8String(firstTestPartHandler._data);
            Assert.AreEqual(attachment, actualContent);

        }


        private void testSingleAttachments(String boundary, String contentSource)
        {

            for (int i = 0; i < 10; i++)
            {
                testSingleAttachment(boundary, contentSource, i);
            }

            int lowerBound = (MultiPartReader.BUFFER_SIZE / 2) - 10;
            int upperBound = (MultiPartReader.BUFFER_SIZE / 2) + 10;

            for (int i = lowerBound; i < upperBound; i++)
            {
                testSingleAttachment(boundary, contentSource, i);
            }


            lowerBound = MultiPartReader.BUFFER_SIZE - 200;
            upperBound = MultiPartReader.BUFFER_SIZE + 10;

            for (int i = lowerBound; i < upperBound; i++)
            {
                testSingleAttachment(boundary, contentSource, i);
            }

            lowerBound = (MultiPartReader.BUFFER_SIZE * 2) - 200;
            upperBound = (MultiPartReader.BUFFER_SIZE * 2) + 10;

            for (int i = lowerBound; i < upperBound; i++)
            {
                testSingleAttachment(boundary, contentSource, i);
            }


        }

        [Test]
        public void testSingleAttachments()
        {
            log.enteredMethod();

            String boundary = "---------------------------114782935826962";

            testSingleAttachments(boundary, "------");
            testSingleAttachments(boundary, "------\r\n");
            testSingleAttachments(boundary, "-----\r\n-");
            testSingleAttachments(boundary, "----\r\n--");
            testSingleAttachments(boundary, "---\r\n---");
            testSingleAttachments(boundary, "--\r\n----");
            testSingleAttachments(boundary, "-\r\n-----");
            testSingleAttachments(boundary, "\r\n------");
            testSingleAttachments(boundary, "\r\n\r\n\r\n");
        }


        static void testDoubleAttachments(String boundary, String contentSource, int length)
        {

            if (0 == length)
            {
                log.debugFormat(contentSource, "contentSource");
            }
            log.debug(length, "length");

            String attachment = buildAttachment(contentSource, length);
            Assert.AreEqual(length, attachment.Length);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("\r\n--");
            stringBuilder.Append(boundary);
            stringBuilder.Append("\r\n");


            stringBuilder.Append("Content-Disposition: form-data; name=\"datafile\"; filename=\"test1.txt\"\r\n");
            stringBuilder.Append("Content-Type: text/plain\r\n");
            stringBuilder.Append("\r\n");
            stringBuilder.Append(attachment);

            stringBuilder.Append("\r\n--");
            stringBuilder.Append(boundary);
            stringBuilder.Append("\r\n");

            stringBuilder.Append("Content-Disposition: form-data; name=\"datafile\"; filename=\"test2.txt\"\r\n");
            stringBuilder.Append("Content-Type: text/plain\r\n");
            stringBuilder.Append("\r\n");
            stringBuilder.Append(attachment);

            stringBuilder.Append("\r\n--");
            stringBuilder.Append(boundary);
            stringBuilder.Append("--\r\n");


            Entity entity = buildEntity(stringBuilder.ToString());
            MultiPartReader reader = new MultiPartReader(boundary, entity);

            TestMultiPartHandler testMultiPartHandler = new TestMultiPartHandler();
            reader.Process(testMultiPartHandler);
            Assert.True(testMultiPartHandler._foundCloseDelimiter);

            Assert.AreEqual(2, testMultiPartHandler._partHandlers.Count);

            {
                TestPartHandler firstTestPartHandler = testMultiPartHandler._partHandlers[0];
                Assert.AreEqual(attachment.Length, firstTestPartHandler._data.Length);
                String actualContent = DataHelper.ToUtf8String(firstTestPartHandler._data);
                Assert.AreEqual(attachment, actualContent);
            }

            {
                TestPartHandler secondTestPartHandler = testMultiPartHandler._partHandlers[1];
                Assert.AreEqual(attachment.Length, secondTestPartHandler._data.Length);
                String actualContent = DataHelper.ToUtf8String(secondTestPartHandler._data);
                Assert.AreEqual(attachment, actualContent);
            }


        }

        private void testDoubleAttachments(String boundary, String contentSource)
        {

            for (int i = 0; i < 10; i++)
            {
                testDoubleAttachments(boundary, contentSource, i);
            }

            int lowerBound = (MultiPartReader.BUFFER_SIZE / 2) - 10;
            int upperBound = (MultiPartReader.BUFFER_SIZE / 2) + 10;

            for (int i = lowerBound; i < upperBound; i++)
            {
                testDoubleAttachments(boundary, contentSource, i);
            }


            lowerBound = MultiPartReader.BUFFER_SIZE - 200;
            upperBound = MultiPartReader.BUFFER_SIZE + 10;

            for (int i = lowerBound; i < upperBound; i++)
            {
                testDoubleAttachments(boundary, contentSource, i);
            }

            lowerBound = (MultiPartReader.BUFFER_SIZE * 2) - 200;
            upperBound = (MultiPartReader.BUFFER_SIZE * 2) + 10;

            for (int i = lowerBound; i < upperBound; i++)
            {
                testDoubleAttachments(boundary, contentSource, i);
            }
        }

        [Test]
        public void testDoubleAttachments()
        {
            log.enteredMethod();

            String boundary = "---------------------------114782935826962";

            testDoubleAttachments(boundary, "------");
            testDoubleAttachments(boundary, "------\r\n");
            testDoubleAttachments(boundary, "-----\r\n-");
            testDoubleAttachments(boundary, "----\r\n--");
            testDoubleAttachments(boundary, "---\r\n---");
            testDoubleAttachments(boundary, "--\r\n----");
            testDoubleAttachments(boundary, "-\r\n-----");
            testDoubleAttachments(boundary, "\r\n------");
            testDoubleAttachments(boundary, "\r\n\r\n\r\n");
        }


    }
}

