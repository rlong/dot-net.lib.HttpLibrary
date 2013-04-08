// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;


using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;


using System.Threading;
using jsonbroker.library.common.http;
using jsonbroker.library.common.log;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.server.http
{
    public class ConnectionHandler
    {
        private static Log log = Log.getLog(typeof(ConnectionHandler));


        ////////////////////////////////////////////////////////////////////////////
        private static int _connectionId = 1;


        ////////////////////////////////////////////////////////////////////////////
        private static List<RequestHandler> _httpProcessors = new List<RequestHandler>();

        public static void AddHttpProcessor(RequestHandler httpProcessor)
        {
            _httpProcessors.Add(httpProcessor);
        }


        ////////////////////////////////////////////////////////////////////////////
        private Socket _socket;

        ////////////////////////////////////////////////////////////////////////////
        private NetworkStream _networkStream;

        ////////////////////////////////////////////////////////////////////////////
        // 
        RequestHandler _httpProcessor;

        private ConnectionHandler(Socket socket, RequestHandler httpProcessor)
        {
            this._socket = socket;
            //
            {
                LingerOption linger = new LingerOption(true, 60);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, linger);
                Object langer = _socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
            }
            _httpProcessor = httpProcessor;

            _networkStream = new NetworkStream(_socket, FileAccess.ReadWrite, false);

        }

        private HttpRequest readRequest()
        {
            HttpRequest answer = null;
            try
            {
                answer = HttpRequestReader.readRequest(_networkStream);
            }
            catch (Exception e)
            {
                log.warn(e);

            }
            return answer;
        }


        public HttpResponse processRequest(HttpRequest request)
        {
            try
            {
                return _httpProcessor.processRequest(request);
            }
            catch (Exception e)
            {
                log.warn(e);
                return HttpErrorHelper.toHttpResponse(e);
            }
        }



        private bool writeResponse(HttpResponse response)
        {
            // socket closed while processing the request ...
            if (!_socket.Connected)
            {
                log.warn("!_socket.Connected");
                return false;
            }

            try
            {
                // write the response ... 
                HttpResponseWriter.writeResponse(response, _networkStream);
            }
            catch (BaseException e)
            {
                if (e.FaultCode == StreamUtilities.IOEXCEPTION_ON_STREAM_WRITE)
                {
                    log.warn("IOException raised while writing response (socket closed ?)");
                    return false;
                }
                else
                {
                    log.warn(e);
                    return false;
                }
            }
            catch (Exception e)
            {
                log.warn(e);
                return false;
            }

            return true;
        }

        private void cleanup(HttpResponse response)
        {

            // clean up 'entity' stream if it exists... 
            Entity entity = response.Entity;
            if (null != entity)
            {
                StreamUtilities.close(entity.getContent(), false, typeof(ConnectionHandler));
            }
        }

        private void logRequestResponse(HttpRequest request, HttpResponse response, bool writeResponseSucceded)
        {
            int statusCode = response.Status;

            long timeTaken = DateTime.Now.Ticks - request.Created;
            // DateTime .Ticks Property ... 
            //A single tick represents one hundred nanoseconds or one ten-millionth of a second. There are 10,000 ticks in a millisecond.
            timeTaken /= 10 * 1000;

            String payloadSize = "-";
            LongObject contentLength = response.getContentLength();
            if (null != contentLength)
            {
                payloadSize = contentLength.longValue().ToString();
            }

            String requestUri = request.RequestUri;
            if (writeResponseSucceded)
            {
                log.infoFormat("{0} {1} {2} T {3}", statusCode, timeTaken, payloadSize, requestUri);
            }
            else
            {
                log.infoFormat("{0} {1} {2} F {3}", statusCode, timeTaken, payloadSize, requestUri);
            }
        }


        private bool processRequest()
        {
            // get the request ... 
            HttpRequest request = readRequest();
            if (null == request)
            {
                log.debug("null == request");
                return false;
            }

            // process the request ... 
            HttpResponse response = processRequest(request);

            bool continueProcessingRequests = true;

                // vvv http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.10
            if( request.isCloseConnectionIndicated() ) {  // if( [request isCloseConnectionIndicated] ) {

                continueProcessingRequests = false;
            }
            // ^^^ http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.10


            int statusCode = response.Status;
            if (statusCode < 200 || statusCode > 299)
            {
                continueProcessingRequests = false;
            }

            if (continueProcessingRequests)
            {
                response.putHeader("Connection", "keep-alive");
            }
            else
            {
                response.putHeader("Connection", "close");
            }

            // write the response ... 
            bool writeResponseSucceded = writeResponse(response);

            cleanup(response);

            logRequestResponse(request, response, writeResponseSucceded);

            if (!writeResponseSucceded)
            {
                continueProcessingRequests = false;
            }

            // if the processing completed, we will permit more requests on this socket
            return continueProcessingRequests;
        }


        // no equivalent in objective-c
        private void processRequests()
        {
            try
            {
                while (processRequest())
                {
                }
            }
            catch (Exception e)
            {
                log.warn(e);
            }

            log.debug("finishing");


            try
            {
                if (_socket.Connected)
                {
                    StreamUtilities.flush(_networkStream, true, this);

                    // For connection-oriented protocols, it is recommended that you call Shutdown before calling the Close method. 
                    // This ensures that all data is sent and received on the connected socket before it is closed. 
                    //_socket.Shutdown(SocketShutdown.Both);

                    _socket.Close(60);
                    //_socket.Disconnect(true);
                    StreamUtilities.close(_networkStream, true, this);
                }
                else
                {
                    log.debug("!_socket.Connected");
                }

            }
            catch (Exception e)
            {
                log.warn(e.Message, "e.Message");
            }

        }

        public void run()
        {
            try
            {
                processRequests();
            }
            catch (Exception e)
            {
                log.error(e);
            }

            log.debug("finished");

        }

        public static void handleConnection(Socket socket, RequestHandler httpProcessor)
        {

            log.enteredMethod();

            ConnectionHandler connectionHandler = new ConnectionHandler(socket, httpProcessor);

            Thread thread = new Thread(new ThreadStart(connectionHandler.run));
            thread.IsBackground = true; // daemon thread
            thread.Name = "ConnectionHandler." + _connectionId++ + "." + socket.Handle.ToInt32();
            thread.Start();

        }
    }
}
