// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.json.handlers;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.common.json.input
{
    public class JsonReader
    {

        private static void onNumber( int index, Object value, JsonDocumentHandler handler )
        {
            if( value is double ) { 
                handler.onNumber( index, (double)value );
                return;
            }


            if (value is float)
            {
                handler.onNumber(index, (float)value);
                return;
            }

            if (value is int)
            {
                handler.onNumber(index, (int)value);
                return;
            }


            if (value is long)
            {
                handler.onNumber(index, (long)value);
                return;
            }

        }

        private static void readArray(JsonInput input, JsonDocumentHandler handler)
        {


            int index = -1;

            for (byte nextTokenStart = JsonInputHelper.scanToNextToken(input); ']' != nextTokenStart; nextTokenStart = JsonInputHelper.scanToNextToken(input))
            {

                index++;

                if ('"' == nextTokenStart)
                {
                    String value = JsonStringHandler.readString(input);
                    handler.onString(index, value);
                    continue;
                }

                if ('0' <= nextTokenStart && nextTokenStart <= '9')
                {
                    Object value = JsonNumberHandler.readNumber(input);
                    onNumber(index, value, handler);
                    continue;
                }

                if ('[' == nextTokenStart)
                {
                    handler.onArrayStart(index);
                    input.nextByte(); // skip past the '['
                    readArray(input, handler);
                    input.nextByte(); // skip past the ']'
                    handler.onArrayEnd(index);
                    continue;
                }

                if ('{' == nextTokenStart)
                {
                    handler.onObjectStart(index);
                    input.nextByte(); // skip past the '{'
                    readObject(input, handler);
                    input.nextByte(); // skip past the '}'
                    handler.onObjectEnd(index);
                    continue;
                }

                if ('+' == nextTokenStart)
                {
                    Object value = JsonNumberHandler.readNumber(input);
                    onNumber(index, value, handler);
                    continue;
                }
                if ('-' == nextTokenStart)
                {
                    Object value = JsonNumberHandler.readNumber(input);
                    onNumber(index, value, handler);
                    continue;
                }

                if ('t' == nextTokenStart || 'T' == nextTokenStart)
                {
                    Boolean value = JsonBooleanHandler.readBoolean(input);
                    handler.onBoolean(index, value);
                    continue;
                }

                if ('f' == nextTokenStart || 'F' == nextTokenStart)
                {
                    Boolean value = JsonBooleanHandler.readBoolean(input);
                    handler.onBoolean(index, value);
                    continue;
                }

                if ('n' == nextTokenStart || 'N' == nextTokenStart)
                {
                    JsonNullHandler.readNull(input);
                    handler.onNull(index);
                    continue;
                }


                String technicalError = String.Format("bad tokenBeginning; nextTokenStart = {0} ({1})", nextTokenStart, (char)nextTokenStart);
                throw new BaseException(typeof(JsonHandler), technicalError);

            }

        }


        private static void onNumber(String key, Object value, JsonDocumentHandler handler)
        {
            if (value is double)
            {
                handler.onNumber(key, (double)value);
                return;
            }


            if (value is float)
            {
                handler.onNumber(key, (float)value);
                return;
            }

            if (value is int)
            {
                handler.onNumber(key, (int)value);
                return;
            }


            if (value is long)
            {
                handler.onNumber(key, (long)value);
                return;
            }
        }

        private static void readObject(JsonInput input, JsonDocumentHandler handler)
        {

            for (byte nextTokenStart = JsonInputHelper.scanToNextToken(input); '}' != nextTokenStart; nextTokenStart = JsonInputHelper.scanToNextToken(input))
            {

                String key = JsonStringHandler.readString(input);

                nextTokenStart = JsonInputHelper.scanToNextToken(input);

                if ('"' == nextTokenStart)
                {
                    String value = JsonStringHandler.readString(input);
                    handler.onString(key, value);
                    continue;
                }

                if ('0' <= nextTokenStart && nextTokenStart <= '9')
                {
                    Object value = JsonNumberHandler.readNumber(input);
                    onNumber(key, value, handler);
                    continue;
                }

                if ('[' == nextTokenStart)
                {
                    handler.onArrayStart(key);
                    input.nextByte(); // skip past the '['
                    readArray(input, handler);
                    input.nextByte(); // skip past the ']'
                    handler.onArrayEnd(key);
                    continue;
                }

                if ('{' == nextTokenStart)
                {
                    handler.onObjectStart(key);
                    input.nextByte(); // skip past the '{'
                    readObject(input, handler);
                    input.nextByte(); // skip past the '}'
                    handler.onObjectEnd(key);
                    continue;
                }

                if ('+' == nextTokenStart)
                {
                    Object value = JsonNumberHandler.readNumber(input);
                    onNumber(key, value, handler);
                    continue;
                }
                if ('-' == nextTokenStart)
                {
                    Object value = JsonNumberHandler.readNumber(input);
                    onNumber(key, value, handler);
                    continue;
                }

                if ('t' == nextTokenStart || 'T' == nextTokenStart)
                {
                    Boolean value = JsonBooleanHandler.readBoolean(input);
                    handler.onBoolean(key, value);
                    continue;
                }

                if ('f' == nextTokenStart || 'F' == nextTokenStart)
                {
                    Boolean value = JsonBooleanHandler.readBoolean(input);
                    handler.onBoolean(key, value);
                    continue;
                }

                if ('n' == nextTokenStart || 'N' == nextTokenStart)
                {
                    JsonNullHandler.readNull(input);
                    handler.onNull(key);
                    continue;
                }

                String technicalError = String.Format("bad tokenBeginning; nextTokenStart = {0} ({1})", nextTokenStart, (char)nextTokenStart);
                throw new BaseException(typeof(JsonHandler), technicalError);
			

            }
		
        }

        public static void read(JsonInput input, JsonDocumentHandler handler)
        {
            byte nextByte = input.currentByte();
            input.nextByte(); // skip past the '{' / '['
            if ('{' == nextByte)
            {
                handler.onObjectDocumentStart();
                readObject(input, handler);
                handler.onObjectDocumentEnd();
            }
            else
            {
                handler.onArrayDocumentStart();
                readArray(input, handler);
                handler.onArrayDocumentEnd();
            }
        }
    }
}
