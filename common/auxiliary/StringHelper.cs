// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace jsonbroker.library.common.auxiliary
{
    public class StringHelper
    {

        static readonly char[] _hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        public static byte[] ToUtfBytes( String str ) 
        {
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            byte[] answer = utf8Encoding.GetBytes(str);
            return answer;
        }


        public static String getUtf8String(Data data)
        {
            return data.getUtf8String(0, data.Length);
        }

        public static String ToHexString(byte[] bytes)
        {
            int count = bytes.Length;

            StringBuilder answer = new StringBuilder(count * 2);

            for (int i = 0; i < count; i++)
            {
                byte b = bytes[i];

                answer.Append(_hexDigits[b >> 4]);

                answer.Append(_hexDigits[b & 0xf]);
            }
            return answer.ToString();
        }

        public static String UrlEncode(String str)
        {
            return Uri.EscapeUriString( str); 
        }

    }
}
