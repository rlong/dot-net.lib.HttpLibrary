// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using jsonbroker.library.common.log;
using jsonbroker.library.common.json.input;
using jsonbroker.library.common.auxiliary;
using System.IO;

namespace jsonbroker.library.common.json
{

    [TestFixture]
    //[Category("current")]
    public class JsonBuilderUnitTest
    {
        private static Log log = Log.getLog(typeof(JsonBuilderUnitTest));

        [Test]
        public void test1()
        {
            log.enteredMethod();
        }

        private static JsonInput toJsonInput(String input)
        {

            byte[] rawData = StringHelper.ToUtfBytes(input);
            Data data = new Data(rawData);
            JsonInput answer = new JsonDataInput(data);
            return answer;

        }

        [Test]
        public void testEmptyObject()
        {
            JsonInput input = toJsonInput("{}");
            JsonBuilder builder = new JsonBuilder();
            JsonReader.read(input, builder);
            Assert.NotNull(builder.getObjectDocument());            
        }

        [Test]
        public void testEmptyArray()
        {
            JsonInput input = toJsonInput("[]");
            JsonBuilder builder = new JsonBuilder();
            JsonReader.read(input, builder);
            Assert.NotNull(builder.getArrayDocument());
        }

        [Test]
        public void testSimpleObject()
        {
            JsonInput input = toJsonInput("{\"nullKey\":null,\"booleanKey\":true,\"integerKey\":314,\"stringKey\":\"value\"}");
            JsonBuilder builder = new JsonBuilder();
            JsonReader.read(input, builder);

            JsonObject jsonDocument = builder.getObjectDocument();
            Assert.NotNull(jsonDocument);
            Assert.Null(jsonDocument.getObject("nullKey", null));
            Assert.IsTrue(jsonDocument.getBoolean("booleanKey"));
            Assert.AreEqual(314, jsonDocument.getInteger("integerKey"));
            Assert.AreEqual("value", jsonDocument.getString("stringKey"));
        }


        private static JsonInput getJsonInputForFilePath(String filePath)
        {
            log.debug(Directory.GetCurrentDirectory(), "Directory.GetCurrentDirectory()");

            // vvv http://stackoverflow.com/questions/2030847/best-way-to-read-a-large-file-into-byte-array-in-c
            byte[] allBytes = File.ReadAllBytes(filePath);
            // ^^^ http://stackoverflow.com/questions/2030847/best-way-to-read-a-large-file-into-byte-array-in-c

            Data data = new Data( allBytes);
            return new JsonDataInput(data);

        }


        [Test]
        public void testStatusStopped()
        {
            JsonInput jsonInput = getJsonInputForFilePath("..\\..\\..\\jsonbroker.c_sharp.library\\_auxiliary\\unit_test\\JsonBuilderUnitTest\\status.paused.osx.vlc-2-0-4.json");
            JsonBuilder builder = new JsonBuilder();
            JsonReader.read(jsonInput, builder);
        }

        [Test]
        public void testStatusPaused()
        {
            JsonInput jsonInput = getJsonInputForFilePath("..\\..\\..\\jsonbroker.c_sharp.library\\_auxiliary\\unit_test\\JsonBuilderUnitTest\\status.paused.osx.vlc-2-0-4.json");
            JsonBuilder builder = new JsonBuilder();
            JsonReader.read(jsonInput, builder);

        }

        [Test]
        public void testStatusPlaying()
        {
            JsonInput jsonInput = getJsonInputForFilePath("..\\..\\..\\jsonbroker.c_sharp.library\\_auxiliary\\unit_test\\JsonBuilderUnitTest\\status.playing.osx.vlc-2-0-4.json");
            JsonBuilder builder = new JsonBuilder();
            JsonReader.read(jsonInput, builder);
        }
    }
}
