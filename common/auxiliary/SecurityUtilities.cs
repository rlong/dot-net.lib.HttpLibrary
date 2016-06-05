// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography; 
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.auxiliary
{
    public class SecurityUtilities
    {
        private static readonly Log log = Log.getLog(typeof(SecurityUtilities));

        public static String generateNonce()
        {
            // TODO: FDB46896-A292-4F48-811C-88651F37B4F8
            // TODO: come up with a better nonce solution

            return RandomUtilities.generateUuid();
        }


        static String generateNumericUserPassword(byte[] bytes)
        {

            StringBuilder answer = new StringBuilder(bytes.Length);

            for (int i = 0, count = bytes.Length; i < count; i++)
            {
                int offset = bytes[i] % 10;
                if (0 > offset)
                {
                    offset = -offset;
                }
                log.debug(offset, "offset");
                answer.Append((char)('0' + offset));
            }

            return answer.ToString();

        }


        public static String generateNumericUserPassword()
        {
            byte[] bytes = new byte[8];
            RandomUtilities.random(bytes);
            return generateNumericUserPassword(bytes);
        }


        public static string md5HashOfBytes(byte[] input)
        {
            MD5CryptoServiceProvider md5Context = new MD5CryptoServiceProvider();

            byte[] hash = md5Context.ComputeHash(input);

            String answer = ByteHelper.ToHexString(hash);

            return answer;
        }

        public static string md5HashOfData(Stream input)
        {

            long position = input.Position;

            MD5CryptoServiceProvider md5Context = new MD5CryptoServiceProvider();

            byte[] hash = md5Context.ComputeHash(input);

            String answer = ByteHelper.ToHexString(hash);

            return answer;
        }

        public static string md5HashOfData(Data input)
        {
            return md5HashOfData(input.ToStream());
        }

        public static String md5HashOfString(String input)
        {
            UTF8Encoding utf8Encoding = new UTF8Encoding();
            byte[] utf8Value = utf8Encoding.GetBytes(input);

            MD5CryptoServiceProvider md5Context = new MD5CryptoServiceProvider();
            byte[] hash = md5Context.ComputeHash(utf8Value);

            String answer = ByteHelper.ToHexString(hash);

            return answer;

        }
    }
}
