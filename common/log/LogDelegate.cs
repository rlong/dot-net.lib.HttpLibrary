// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.log
{
    public interface LogDelegate
    {
        bool isDebugEnabled();

        void debug(Log origin, String message);
        void debug(Log origin, bool value, String name);
        void debug(Log origin, float value, String name);
        void debug(Log origin, int value, String name);
        void debug(Log origin, Loggable value, String name);
        void debug(Log origin, long value, String name);
        void debug(Log origin, Object value, String name);
        void debug(Log origin, String value, String name);

        void enteredMethod(Log origin);

        void info(Log origin, String message);

        void warn(Log origin, String message);

        void error(Log origin, String message);
    }
}
