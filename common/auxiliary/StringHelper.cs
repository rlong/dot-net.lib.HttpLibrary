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


        public static byte[] ToUtfBytes( String str ) 
        {
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            byte[] answer = utf8Encoding.GetBytes(str);
            return answer;
        }






        public static String UrlEncode(String str)
        {
            return Uri.EscapeUriString( str); 
        }

    }
}
