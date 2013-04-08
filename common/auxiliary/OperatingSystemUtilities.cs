// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;
using System.IO;

namespace jsonbroker.library.common.auxiliary
{
    public class OperatingSystemUtilities
    {

        private static Log log = Log.getLog(typeof(OperatingSystemUtilities));


        private static BooleanObject _isWindows;
        private static BooleanObject _isOsx;

        public static bool isWindows()
        {
            if (null != _isWindows)
            {
                return _isWindows.booleanValue();
            }


            if ('\\' == Path.DirectorySeparatorChar) // windows machine ?
            {
                _isWindows = BooleanObject.TRUE;
            }
            else
            {
                _isWindows = BooleanObject.FALSE;
            }

            return _isWindows.booleanValue();
        }


        public static bool isOsx()
        {

            if (null == _isOsx)
            {

                if (Directory.Exists("/Applications") && Directory.Exists("/Users"))
                {
                    _isOsx = BooleanObject.TRUE;
                }
                else
                {
                    _isOsx = BooleanObject.FALSE;
                }
            }

            return _isOsx.booleanValue();
        }

    }
}
