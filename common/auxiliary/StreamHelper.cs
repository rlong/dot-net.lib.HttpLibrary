// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.auxiliary
{
    public class StreamHelper
    {

        private static readonly Log log = Log.getLog(typeof(StreamHelper));


        private static readonly int BASE = ErrorCodeUtilities.getBaseErrorCode("jsonbroker.StreamUtilities");

        public static readonly int IOEXCEPTION_ON_STREAM_WRITE = BASE | 0x01;

        public static void close(StreamReader streamReader, bool swallowException, Object caller)
        {
            if (null == streamReader)
            {
                return;
            }

            try
            {
                streamReader.Close();
            }
            catch (Exception e)
            {
                if (swallowException)
                {
                    log.warn(e.Message, "e.Message");
                }
                else
                {
                    throw new BaseException(caller, e);
                }
            }
        }

        public static void close(Stream stream, bool swallowException, Object caller)
        {
            if (null == stream)
            {
                return;
            }

            try
            {
                stream.Close();

            }
            catch (Exception e)
            {
                if (swallowException)
                {
                    log.warn(e.Message, "e.Message");
                }
                else
                {
                    throw new BaseException(caller, e);
                }
            }


        }

        public static void flush(Stream stream, bool swallowException, Object caller)
        {
            if (null == stream)
            {
                return;
            }

            try
            {
                stream.Flush();
            }
            catch (Exception e)
            {
                if (swallowException)
                {
                    log.warn(e.Message, "e.Message");
                }
                else
                {
                    throw new BaseException(caller, e);
                }
            }
            
        }



        // vvv http://stackoverflow.com/questions/1933742/how-is-the-stream-copytostream-method-implemented-in-net-4
        public static void write(Stream source, Stream destination)
        {

            int num;

            byte[] buffer = new byte[0x1000];

            while ((num = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, num);
            }
        }

        public static void write(long numOctets, Stream source, Stream destination )
        {

            // vvv http://stackoverflow.com/questions/1933742/how-is-the-stream-copytostream-method-implemented-in-net-4

            byte[] buffer = new byte[0x1000];


            while (numOctets > 0)
            {
                int amountToRead = buffer.Length;
                if (amountToRead > numOctets)
                {
                    amountToRead = (int)numOctets;
                }
                int bytesRead = source.Read(buffer, 0, amountToRead);
                if (0 == bytesRead)
                {
                    throw new BaseException(typeof(StreamHelper), "0 == bytesRead");
                }

                try
                {
                    destination.Write(buffer, 0, bytesRead);
                }
                catch (IOException ioe)
                {
                    BaseException e = new BaseException(typeof(StreamHelper), ioe);
                    e.FaultCode = IOEXCEPTION_ON_STREAM_WRITE;
                    throw e;
                }
                
                numOctets -= bytesRead;
            }
        }

        // ^^^ http://stackoverflow.com/questions/1933742/how-is-the-stream-copytostream-method-implemented-in-net-4


    }
}
