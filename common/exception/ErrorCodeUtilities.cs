// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.exception
{
    public class ErrorCodeUtilities
    {

                    
        private static Log log = Log.getLog(typeof(ErrorCodeUtilities));

        public static int getBaseErrorCode(String errorKey)
        {

            MD5CryptoServiceProvider digester = new MD5CryptoServiceProvider();

            byte[] input;
            {
                UTF8Encoding utf8Encoding = new UTF8Encoding();
                input = utf8Encoding.GetBytes(errorKey);
            }

            byte[] hash = digester.ComputeHash(input);

            int answer = 0xFF & hash[0];
            answer <<= 8;
            answer |= 0xFF & hash[1];
            answer <<= 4;
            answer |= (0xF0 & hash[2]) >> 4;
            answer <<= 8;

            log.infoFormat("'{0}' -> {1:x}", errorKey, answer);

            return answer;
        }
    }
}
