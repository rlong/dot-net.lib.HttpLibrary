// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.log
{


    public class ReleaseLogDelegate : LogDelegate
    {
        private LogDelegate _delegate;

        public ReleaseLogDelegate(LogDelegate logDelegate)
        {
            _delegate = logDelegate;
        }


        public bool isDebugEnabled()
        {
            return false;
        }

        public void debug(Log origin, String message)
        {
        }

        public void debug(Log origin, bool value, String name)
        {
        }

        public void debug(Log origin, float value, String name)
        {
        }

        public void debug(Log origin, int value, String name)
        {
        }

        public void debug(Log origin, Loggable value, String name)
        {
        }

        public void debug(Log origin, long value, String name)
        {
        }

        public void debug(Log origin, Object value, String name)
        {
        }

        public void debug(Log origin, String value, String name)
        {
        }

        public void enteredMethod(Log origin)
        {
        }

        public void info(Log origin, String message)
        {
            _delegate.info(origin, message);
        }


        public void warn(Log origin, String message)
        {
            _delegate.warn(origin, message);
        }

        public void error(Log origin, String message)
        {
            _delegate.error(origin, message);
        }

    }
}
