// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.auxiliary
{
    public class RandomUtilities
    {
        static readonly char[] _hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        // Instantiate random number generator using system-supplied value as seed.
        static readonly Random _randy = new Random();


        public static void random(byte[] bytes)
        {
            _randy.NextBytes(bytes);
        }


        public static String generateUuid()
        {
            byte[] uuid = new byte[16];

            _randy.NextBytes(uuid);

            return StringHelper.ToHexString(uuid);
        }

    }
}
