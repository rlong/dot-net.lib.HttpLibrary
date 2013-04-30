// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace jsonbroker.library.common.json.output
{
    public class JsonStreamOutput : JsonOutput
    {


        // using the default 'Encoding.UTF8' the '_streamWriter' will include a prefix Byte Order Mark of the bytes [239,187,191]
        // 
        // vvv http://social.msdn.microsoft.com/Forums/en-US/sqlxml/thread/1fd436bc-a465-4916-a403-361d0bb94f97/
        private readonly UTF8Encoding ENCODING = new UTF8Encoding(false);
        // ^^^ http://social.msdn.microsoft.com/Forums/en-US/sqlxml/thread/1fd436bc-a465-4916-a403-361d0bb94f97/

        StreamWriter _streamWriter;

        public JsonStreamOutput(Stream stream)
        {
            _streamWriter = new StreamWriter(stream, ENCODING);
        }

        public void append(char c)
        {
            _streamWriter.Write(c);
        }

        public void append(String str)
        {
            _streamWriter.Write(str);
        }

        public void Flush()
        {
            _streamWriter.Flush();
        }
    }
}
