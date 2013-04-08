// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.log
{
    public class ConsoleLogDelegate: LogDelegate 
    {
        private bool _isDebugEnabled;

        public ConsoleLogDelegate(bool isDebugEnabled)
        {
            _isDebugEnabled = isDebugEnabled;
        }

        public bool isDebugEnabled()
        {
            return _isDebugEnabled;
        }

        public void debug(Log origin, String message)
        {
            if (_isDebugEnabled)
            {
                String threadName = System.Threading.Thread.CurrentThread.Name;
                String methodName = LogDelegateHelper.getMethodName(origin);
                String line = String.Format("DBG {0} [{1} {2}] {3}", threadName, origin.CallerClassName(), methodName, message);
                Console.WriteLine(line);
            }
        }

        public void debug(Log origin, bool value, String name)
        {
            if (_isDebugEnabled)
            {
                debug(origin, LogDelegateHelper.toString(value, name));
            }
        }

        public void debug(Log origin, float value, String name)
        {
            if (_isDebugEnabled)
            {
                debug(origin, LogDelegateHelper.toString(value, name));
            }
        }

        public void debug(Log origin, int value, String name)
        {
            if (_isDebugEnabled)
            {
                debug(origin, LogDelegateHelper.toString(value, name));
            }
        }

        public void debug(Log origin, Loggable value, String name)
        {
            if (_isDebugEnabled)
            {
                debug(origin, LogDelegateHelper.toString(value, name));
            }
        }

        public void debug(Log origin, long value, String name)
        {
            if (_isDebugEnabled)
            {
                debug(origin, LogDelegateHelper.toString(value, name));
            }
        }

        public void debug(Log origin, Object value, String name)
        {
            if (_isDebugEnabled)
            {
                debug(origin, LogDelegateHelper.toString(value, name));
            }
        }

        public void debug(Log origin, String value, String name)
        {
            if (_isDebugEnabled)
            {
                debug(origin, LogDelegateHelper.toString(value, name));
            }
        }

        public void enteredMethod(Log origin)
        {

            if (_isDebugEnabled)
            {
                debug(origin, "entered");
            }
        }

        public void info(Log origin, String message)
        {
            String threadName = System.Threading.Thread.CurrentThread.Name;
            String methodName = LogDelegateHelper.getMethodName(origin);
            String line = String.Format("INF {0} [{1} {2}] {3}", threadName, origin.CallerClassName(), methodName, message);
            Console.WriteLine(line);

        }

        public void warn(Log origin, String message)
        {
            String threadName = System.Threading.Thread.CurrentThread.Name;
            String methodName = LogDelegateHelper.getMethodName(origin);
            String line = String.Format("WRN {0} [{1} {2}] {3}", threadName, origin.CallerClassName(), methodName, message);
            Console.WriteLine(line);
        }

        public void error(Log origin, String message)
        {
            String threadName = System.Threading.Thread.CurrentThread.Name;
            String methodName = LogDelegateHelper.getMethodName(origin);
            String line = String.Format("ERR {0} [{1} {2}] {3}", threadName, origin.CallerClassName(), methodName, message);
            Console.WriteLine(line);
        }
    }
}
