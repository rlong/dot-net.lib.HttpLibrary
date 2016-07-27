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
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.CoreAnnex.exception;



namespace dotnet.lib.Http.server
{

    public class WebServer
    {


        private static readonly int BASE = ErrorCodeUtilities.getBaseErrorCode("jsonbroker.ConnectionListener");

        private static readonly int SOCKET_BIND_FAILED = BASE | 0x01;

        private static Log log = Log.getLog(typeof(WebServer));

        //////////////////////////////////////////////////////////////////////
        //
        private int _port;

        public int getPort()
        {
            return _port;
        }

        //////////////////////////////////////////////////////////////////////
        //
        private RequestHandler _httpProcessor;

        //////////////////////////////////////////////////////////////////////
        //
        Socket _serverSocket;

        //////////////////////////////////////////////////////////////////////
        //
        public WebServer(int port, RequestHandler httpProcessor)
        {
            _port = port;
            _httpProcessor = httpProcessor;

        }


        public WebServer(RequestHandler httpProcessor)  : this( 8081, httpProcessor )
        {
        }


        //////////////////////////////////////////////////////////////////////
        //
        private void closeServerSocket()
        {

            // vvv http://stackoverflow.com/questions/541194/c-sharp-version-of-javas-synchronized-keyword
            lock (this)
            // ^^^ http://stackoverflow.com/questions/541194/c-sharp-version-of-javas-synchronized-keyword
            {
                if (null == _serverSocket)
                {
                    log.debug("null == _serverSocket");
                    return;
                }

                if (_serverSocket.IsBound)
                {

                    log.debug("_serverSocket.IsBound");
                }
                else
                {
                    log.debug("!_serverSocket.IsBound");
                    // fall through and `try` to close it anyway
                }

                try
                {
                    _serverSocket.Close();
                }
                catch (Exception e)
                {
                    log.warn(e);
                }

                _serverSocket = null;

            }
        }

        //////////////////////////////////////////////////////////////////////
        //
        public void run()
        {
            log.infoFormat("about to start listening for connections on port {0}", _port);

            try
            {
                while (null != _serverSocket)
                {

                    Socket serverSocket;
                    // make a local copy of the serverSocket on the stack
                    lock (this)
                    {
                        serverSocket = _serverSocket;
                    }

                    if (null != serverSocket)
                    {
                        // Accept a new connection from the net, blocking till one comes in
                        Socket clientSocket = _serverSocket.Accept();

                        HttpConnectionHandler.handleConnection(clientSocket, _httpProcessor);
                    }

                }
            }
            catch (Exception e)
            {
                log.warn(e);
            }
            finally
            {
                closeServerSocket();
            }

            log.infoFormat("stopped listening for connections on port {0}", _port);
        }

        public void Start()
        {


            // Create a new server socket, set up all the endpoints, bind the socket and then listen
            {
                /*
                 * 
                 * _serverSocket = new Socket(0, SocketType.Stream, ProtocolType.Tcp);
                 * 
                 * call to "new Socket(0, SocketType.Stream, ProtocolType.Tcp)" 
                 * results in an error "The system detected an invalid pointer address in attempting to use a pointer argument in a call [ConnectionListener:start 47cd01]"
                 * 
                 * for more information see "WSAEFAULT" at MS "Windows Sockets Error Codes" page (http://msdn.microsoft.com/en-us/library/windows/desktop/ms740668%28v=vs.85%29.aspx)
                 * 
                 * 47cd01 == ConnectionListener.SOCKET_BIND_FAILED
                 * 
                 * ticket-reference: 95428884-3192-46D7-B9CD-87DA71BEEEAC
                 * 
                 * */
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                

            }

            // vvv getting "Only one usage of each socket address" (jsonbroker:74F167C8-A8B9-4CB3-9D5D-E2D8B55E97ED)
            {
                _serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                log.info("ReuseAddress set to `true`");
            }
            // ^^^ getting "Only one usage of each socket address" (jsonbroker:74F167C8-A8B9-4CB3-9D5D-E2D8B55E97ED)


            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, _port);
            try
            {
                _serverSocket.Bind(endpoint);
            }
            catch (SocketException se)
            {
                BaseException be = new BaseException(this, se);
                be.FaultCode = SOCKET_BIND_FAILED;
                throw be;
            }
            _serverSocket.Blocking = true;
            _serverSocket.Listen(-1);
            log.debug(_port, "port");


            Thread thread = new Thread(new ThreadStart(this.run));
            thread.Name = "WebServer:" + _port;
            thread.IsBackground = true;
            thread.Start();
        }

        public void Stop()
        {
            log.warn("untested!");
            if (null == _serverSocket)
            {
                // shouldn't be calling stop on an instance that is not listening
                log.warn("null == _serverSocket");
                return;
            }

            closeServerSocket();
        }
    }


}
