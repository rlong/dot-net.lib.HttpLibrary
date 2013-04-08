// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using jsonbroker.library.common.http;
using jsonbroker.library.common.log;
using jsonbroker.library.common.http.headers.request;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.server.http
{
    class HttpResponseWriter
    {
        private static Log log = Log.getLog(typeof(HttpResponseWriter));


        public static void writeResponse(HttpResponse response, Stream outputStream)
        {
            log.enteredMethod();

            int statusCode = response.Status;
            String statusString = HttpStatus.getReason(statusCode);

            StringBuilder statusLineAndHeaders = new StringBuilder();
            statusLineAndHeaders.AppendFormat("HTTP/1.1 {0} {1}\r\n", statusCode, statusString);

            Dictionary<String, String> headers = response.headers;
            foreach (KeyValuePair<String, String> dictionaryEntry in headers)
            {
                statusLineAndHeaders.AppendFormat("{0}: {1}\r\n", dictionaryEntry.Key, dictionaryEntry.Value);
            }

            Entity entity = response.Entity;


            //////////////////////////////////////////////////////////////////
            // no entity

            if (null == entity)
            {
                if (204 != statusCode)
                {
                    log.warnFormat("null == entity && 204 != statusCode; statusCode = {0}", statusCode);
                    statusLineAndHeaders.Append("Content-Length: 0\r\n");
                }
                else
                {
                    // from ...
                    // http://stackoverflow.com/questions/912863/is-an-http-application-that-sends-a-content-length-or-transfer-encoding-with-a-2
                    // ... it would 'appear' safest to not include 'Content-Length' on a 204
                }

                statusLineAndHeaders.Append("Accept-Ranges: bytes\r\n\r\n");

                byte[] utfBytes = StringHelper.ToUtfBytes(statusLineAndHeaders.ToString());
                outputStream.Write(utfBytes, 0, utfBytes.Length);

                return; // our work is done
            }

            //////////////////////////////////////////////////////////////////
            // has entity 

            long entityContentLength = entity.getContentLength();
            long seekPosition = 0;
            long amountToWrite = entityContentLength;

            //////////////////////////////////////////////////////////////////
            // headers relevant to range support
            Range range = response.Range;

            if (null == range)
            {
                statusLineAndHeaders.Append("Accept-Ranges: bytes\r\n");

                if (HttpStatus.PARTIAL_CONTENT_206 == statusCode)
                {
                    log.warn("null == range && HttpStatus.PARTIAL_CONTENT_206 == statusCode");
                }
            }
            else
            {
                String contentRangeHeader = String.Format("Content-Range: {0}\r\n", range.ToContentRange(entityContentLength));
                statusLineAndHeaders.Append(contentRangeHeader);

                amountToWrite = range.getContentLength(entityContentLength);
                seekPosition = range.getSeekPosition(entityContentLength);

                if (HttpStatus.PARTIAL_CONTENT_206 != statusCode)
                {
                    log.warn("null != range && HttpStatus.PARTIAL_CONTENT_206 != statusCode");
                }
            }


            //////////////////////////////////////////////////////////////////
            // content-length and final newline

            statusLineAndHeaders.Append(String.Format("Content-Length: {0}\r\n\r\n", amountToWrite));


            //////////////////////////////////////////////////////////////////
            // write the headers

            byte[] headersUtf8Bytes = StringHelper.ToUtfBytes(statusLineAndHeaders.ToString()); // C# compiler does not like reuse of the name 'utfBytes'
            outputStream.Write(headersUtf8Bytes, 0, headersUtf8Bytes.Length);



            //////////////////////////////////////////////////////////////////
            // write the entity

            Stream entityStream = entity.getContent();
            entityStream.Seek(seekPosition, SeekOrigin.Current);
            StreamUtilities.write(amountToWrite, entityStream, outputStream);
            StreamUtilities.flush(outputStream, false, typeof(HttpResponseWriter));
        }

    }
}
