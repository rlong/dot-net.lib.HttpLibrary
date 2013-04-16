// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.json.output
{
    [TestFixture]
    //[Category("current")]
    public class JsonWriterUnitTest
    {
        private static Log log = Log.getLog(typeof(JsonWriterUnitTest));

        [Test]
        public void test1()
        {
            log.enteredMethod();
        }


        [Test]
        public void testEmptyObject()
        {
            JsonStringOutput output = new JsonStringOutput();
            {
                JsonWriter writer = new JsonWriter(output);

                JsonObject target;
                {
                    target = new JsonObject();
                }

                JsonWalker.walk(target, writer);
            }
            String actual = output.ToString();
            log.debug(actual, "actual");
            Assert.AreEqual("{}", actual);		

        }

        [Test]
        public void testEmptyArray()
        {

            JsonStringOutput output = new JsonStringOutput();
            {
                JsonWriter writer = new JsonWriter(output);

                JsonArray target;
                {
                    target = new JsonArray();
                }

                JsonWalker.walk(target, writer);
            }
            String actual = output.ToString();
            log.debug(actual, "actual");
            Assert.AreEqual("[]", actual);
        }

        [Test]
        public void testSimpleObject()
        {

            JsonStringOutput output = new JsonStringOutput();
            {
                JsonWriter writer = new JsonWriter(output);

                JsonObject target;
                {
                    target = new JsonObject();
                    target.put("nullKey", null);
                    target.put("booleanKey", true);
                    target.put("integerKey", 314);
                    target.put("stringKey", "value");
                }

                JsonWalker.walk(target, writer);
            }

            String actual = output.ToString();
            log.debug(actual, "actual");
            Assert.AreEqual("{\"nullKey\":null,\"booleanKey\":true,\"integerKey\":314,\"stringKey\":\"value\"}", actual);
        }

        [Test]
        public void testSimpleArray()
        {
            JsonStringOutput output = new JsonStringOutput();
            {
                JsonWriter writer = new JsonWriter(output);

                JsonArray target;
                {
                    target = new JsonArray();

                    target.Add((Object)null);
                    target.Add(true);
                    target.Add(314);
                    target.Add("value");
                }

                JsonWalker.walk(target, writer);
            }
            String actual = output.ToString();
            log.debug(actual, "actual");
            Assert.AreEqual("[null,true,314,\"value\"]", actual);

        }

    }
}
