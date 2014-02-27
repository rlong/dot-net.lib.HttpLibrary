using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using jsonbroker.library.common.log;
using jsonbroker.library.common.json.output;
using jsonbroker.library.common.auxiliary;
using jsonbroker.library.common.json.input;

namespace jsonbroker.library.common.json.handlers
{

        
    [TestFixture]
    [Category("current")]
    public class JsonStringHandlerUnitTest
    {
        private static Log log = Log.getLog(typeof(JsonStringHandlerUnitTest));

        [Test]
        public void test1()
        {
            log.enteredMethod();
        }


        private String EncodeJsonStringValue(String input)
        {

            JsonStringOutput output = new JsonStringOutput();
            JsonStringHandler handler = JsonStringHandler.INSTANCE;

            handler.WriteValue(input, output);

            return output.ToString();

        }

        private String DecodeJsonStringValue(String input)
        {


            MutableData data = new MutableData();
            data.Append(StringHelper.ToUtfBytes(input));

            JsonDataInput jsonDataInput = new JsonDataInput(data);

            String answer = JsonStringHandler.readString(jsonDataInput);
            return answer;

        }


        [Test]
        public void testWriteABC()
        {

            String actual = EncodeJsonStringValue("ABC");
            Assert.AreEqual("\"ABC\"", actual);

        }

        [Test]
        public void testReadABC()
        {

            String actual = DecodeJsonStringValue("\"ABC\"");
            Assert.AreEqual("ABC", actual);
        }


        [Test]
        public void testReadWriteSlashes()
        {

            {
                String encodedValue = EncodeJsonStringValue("\\");
                Assert.AreEqual("\"\\\\\"", encodedValue);
                String decodedValue = DecodeJsonStringValue(encodedValue);
                Assert.AreEqual("\\", decodedValue);
            }

            {
                String encodedValue = EncodeJsonStringValue("/");
                Assert.AreEqual("\"\\/\"", encodedValue);
                String decodedValue = DecodeJsonStringValue(encodedValue);
                Assert.AreEqual("/", decodedValue);
            }


        }


    }
}
