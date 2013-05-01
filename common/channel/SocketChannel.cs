// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using jsonbroker.library.common.log;
using System.IO;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.common.channel
{
    public class SocketChannel : Channel
    {

        private static Log log = Log.getLog(typeof(SocketChannel));


        ////////////////////////////////////////////////////////////////////////////
        //
        private Socket _socket;



        public SocketChannel(Socket socket)
        {
            _socket = socket;
        }

        public SocketChannel(string hostname, int port)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(hostname, port);
            _socket = tcpClient.Client;
        }


        public void Close(bool ignoreErrors)
        {
            try
            {
                _socket.Close();
            }
            catch (Exception e)
            {
                if (ignoreErrors)
                {
                    log.warn(e);
                }
                else
                {
                    throw e;
                }
            }
            
        }

        // can return null
        public String ReadLine()
        {

            MutableData data = new MutableData();

            byte[] buffer = new byte[1];
            bool continueReading = true;

            while( continueReading ) 
            {
                int bytesRead = _socket.Receive(buffer);

                if (0 == bytesRead)
                {
                    return null;
                }

                if ('\n' == buffer[0])
                {
                    continueReading = false;
                }
                else
                {
                    data.Append(buffer[0]);
                }
            }

            return DataHelper.ToUtf8String(data);

        }


        public byte[] ReadBytes( int count )
        {
            byte[] answer = new byte[count];
            _socket.Receive(answer);

            return answer;
        }

        public void Write(byte[] bytes)
        {
            _socket.Send(bytes);                 
        }

        public void Write(String line)
        {
            this.Write(StringHelper.ToUtfBytes(line));
        }

        public void WriteLine(String line)
        {
            this.Write(line+'\n');
        }




    }
}
