// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Net.Sockets;
using jsonbroker.library.common.http;
using jsonbroker.library.common.log;
using jsonbroker.library.common.http.headers;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.server.http
{
    public class HttpRequestReader
    {


        private static readonly int LINE_LENGTH_UPPER_BOUND = 512;
        private static readonly int NUMBER_HEADERS_UPPER_BOUND = 32;

        private static readonly bool[] INVALID_CHARS = new bool[256];

        static HttpRequestReader()
        {
            // valid chars are 'cr', 'nl', and all the chars between 'space' and '~'
            for (int i = 0; i < 256; i++)
            {
                INVALID_CHARS[i] = true;
            }

            INVALID_CHARS[0x0d] = false; // 0x0d = 'cr'
            INVALID_CHARS[0x0a] = false; // 0x0a = 'nl'

            for (int i = 0x20; i <= 0x7e; i++) // 0x20 = 'space'; 0x7e = '~'
            {
                INVALID_CHARS[i] = false;
            }
        }

        private static Log log = Log.getLog(typeof(HttpRequestReader));


        private static void setOperationDetailsForRequest(HttpRequest request, String line)
        {
            // If the request line is null, then the other end has hung up on us.  A well
            // behaved client will do this after 15-60 seconds of inactivity.
            if (line == null)
            {
                throw HttpErrorHelper.badRequest400FromOriginator(typeof(HttpRequestReader));
            }


            // HTTP request lines are of the form:
            // [METHOD] [Encoded URL] HTTP/1.?
            string[] tokens = line.Split(new char[] { ' ' });
            if (tokens.Length != 3)
            {
                log.errorFormat("tokens.Length != 3; tokens.Length = {0}; line = '{1}'", tokens.Length, line);
                throw HttpErrorHelper.badRequest400FromOriginator(typeof(HttpRequestReader));
            }

            /*
            * HTTP method ... 
            */
            String method = tokens[0];

            if (HttpMethod.GET.Name.Equals(method))
            {
                request.Method = HttpMethod.GET;
            }
            else if (HttpMethod.POST.Name.Equals(method))
            {
                request.Method = HttpMethod.POST;
            }
            else if (HttpMethod.OPTIONS.Name.Equals(method))
            {
                request.Method = HttpMethod.OPTIONS;
            }
            else
            {
                log.errorFormat("unknown HTTP method; method = '{0}'; line = '{1}'", method, line);
                throw HttpErrorHelper.methodNotImplemented501FromOriginator(typeof(HttpRequestReader));
            }

            /*
             * HTTP request-uri ...
             */
            String requestUri = tokens[1];
            request.RequestUri = requestUri;

        }

        // null corresponds to the end of a stream
        private static String readLine(Stream inputStream, MutableData buffer)
        {


            int byteRead = inputStream.ReadByte();

            if (-1 == byteRead)
            {
                return null;
            }

            int i = 0;

            do 
            {
                if (-1 != byteRead)
                {
                    if (INVALID_CHARS[byteRead])
                    {
                        log.errorFormat("INVALID_CHARS[ byteRead ]; byteRead = 0x{0:x}", byteRead);
                        throw HttpErrorHelper.badRequest400FromOriginator(typeof(HttpRequestReader));
                    }
                }

                // end of stream or end of the line
                if (-1 == byteRead || '\n' == byteRead)
                {
                    String answer = StringHelper.getUtf8String(buffer);
                    return answer;
                }

                // filter out '\r'
                if ('\r' != byteRead)
                {
                    buffer.Append((byte)byteRead);
                }

                byteRead = inputStream.ReadByte();
                i++;

            } while( i < LINE_LENGTH_UPPER_BOUND );

            // line is too long
            log.errorFormat("line too long; i = {0}", i);
            throw HttpErrorHelper.badRequest400FromOriginator(typeof(HttpRequestReader));

        }

        private static void addHeader(String header, HttpRequest request)
        {
         
            String name;
            String value;

            int firstColon = header.IndexOf(":");
            if (-1 == firstColon)
            {

                log.errorFormat("-1 == firstColon; header = '{0}'", header);
                throw HttpErrorHelper.badRequest400FromOriginator(typeof(HttpRequestReader));
            }

            name = header.Substring(0, firstColon).ToLower(); // headers are case insensitive
            value = header.Substring(firstColon + 1).Trim();

            if (Log.isDebugEnabled())
            {
                if ("authorization".Equals(name))
                {
                    log.debug(value, name);
                }
            }
            request.headers[name] = value;

        }


        public static HttpRequest readRequest(Stream inputStream)
        {
            MutableData buffer = new MutableData();

            String firstLine = readLine(inputStream, buffer);
            log.debug(firstLine, "firstLine");

            // null corresponds to the end of a stream
            if (null == firstLine)
            {
                return null;
            }

            HttpRequest answer = new HttpRequest();
            setOperationDetailsForRequest(answer, firstLine);

            int i = 0;
            do 
            {
                buffer.clear();
                String line = readLine(inputStream, buffer);
                if (0 == line.Length)
                {
                    break;
                }
                else
                {
                    addHeader(line, answer);
                }

            } while (i < NUMBER_HEADERS_UPPER_BOUND);

            if (i > NUMBER_HEADERS_UPPER_BOUND)
            {
                log.errorFormat("i > NUMBER_HEADERS_UPPER_BOUND; i = {0}", i);
                throw HttpErrorHelper.badRequest400FromOriginator(typeof(HttpRequestReader));
            }

            String contentLengthString = null;

            if (answer.headers.ContainsKey("content-length"))
            {
                contentLengthString = answer.headers["content-length"];
            }

            // no body ?
            if (null == contentLengthString)
            {
                log.debug("null == contentLengthString");
                return answer;
            }


            long contentLength = Int64.Parse(contentLengthString);
            log.debug(contentLength, "contentLength");

            Entity body = new StreamEntity(contentLength, inputStream);
            answer.Entity = body;

            return answer;

        }
    }
}
