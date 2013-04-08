// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.common.log
{
    public class Log 
    {


        ////////////////////////////////////////////////////////////////////////////
	    //
	    private static LogDelegate _delegate = new TraceLogDelegate(true);

        public static LogDelegate getDelegate()
        {
            return _delegate;
        }

	    public static void setDelegate(LogDelegate logDelegate) 
        {
            _delegate = logDelegate;
        }


        ////////////////////////////////////////////////////////////////////////////
        //
        private Type _targetType;

        public Type TargetType()
        {
            return _targetType;
        }

        ////////////////////////////////////////////////////////////////////////////
        //
        String _callerClassName;

        public String CallerClassName()
        {
            return _callerClassName;
        }

        ////////////////////////////////////////////////////////////////////////////
        //
        private Log(Type type) {

            this._targetType = type;
            this._callerClassName = type.Name;

        }

        public static Log getLog(Type type) {
            return new Log( type );
        }


        private static String[] ToLogMessages(Exception e)
        {
            List<String> list = new List<String>();

            list.Add("message = " + e.Message);
            list.Add("className = " + e.GetType().Name);


            list.Add("stack = {");

            String[] stackTrace = ExceptionHelper.getStackTrace(e,true);
            foreach (String strackFrame in stackTrace)
            {
                list.Add("    " + strackFrame);
            }

            list.Add("}");
            return list.ToArray();
        }

        public static bool isDebugEnabled()
        {
            return _delegate.isDebugEnabled();
        }



        public void debug(String message)
        {
            _delegate.debug(this, message);
        }


        public void debug(bool value, String name)
        {
            _delegate.debug(this, value, name);
        }

        public void debug(int value, String name)
        {
            _delegate.debug(this, value, name);
        }


        public void debug(Loggable value, String name)
        {
            _delegate.debug(this, value, name);
        }

        public void debug(long value, String name)
        {
            _delegate.debug(this, value, name);
        }

        public void debug(Object value, String name)
        {
            _delegate.debug(this, value, name);
        }


        public void debug(String value, String name)
        {
            _delegate.debug(this, value, name);
        }

        public void debugFormat(String format, params Object[] values)
        {
            this.debug(LogDelegateHelper.FormatString(format, values));
        }


        public void enteredMethod()
        {
            _delegate.enteredMethod(this);
        }



        /////////////////////////////////////////////////////////////////////////


        public void info(String message)
        {
            _delegate.info(this, message);
        }


        public void info(Loggable value, String name)
        {

            this.info(LogDelegateHelper.toString(value, name));

        }

        public void info(long value, String name)
        {
            this.info(LogDelegateHelper.toString(value, name));
        }


        public void info(String value, String name)
        {
            this.info(LogDelegateHelper.toString(value, name));
        }

        public void infoFormat(String format, params Object[] values)
        {
            this.info(LogDelegateHelper.FormatString(format, values));
        }



        public void warn(String message)
        {
            _delegate.warn(this, message);
        }




        public void warn(Exception e)
        {
            String[] messages = null;
            if (e is Loggable)
            {
                Loggable loggable = (Loggable)e;
                messages = loggable.getLogMessages();
            }
            else
            {
                messages = ToLogMessages(e);
            }

            foreach (String message in messages)
            {
                warn(message);
            }
        }

        public void warn(bool value, String name)
        {
            this.warn(LogDelegateHelper.toString(value, name));
        }

        public void warn(int value, String name)
        {
            this.warn(LogDelegateHelper.toString(value, name));
        }


        public void warn(Loggable value, String name)
        {
            this.warn(LogDelegateHelper.toString(value, name));
        }


        public void warn(String value, String name)
        {
            this.warn(LogDelegateHelper.toString(value, name));
        }

        public void warn(ulong value, String name)
        {
            this.warn(LogDelegateHelper.toString(value, name));
        }

        public void warnFormat(String format, params Object[] values)
        {
            this.warn(LogDelegateHelper.FormatString(format, values));
        }



        public void error(String message)
        {
            _delegate.error(this, message);
        }

        public void error(Exception e)
        {

            String[] messages = null;
            if (e is Loggable)
            {
                Loggable loggable = (Loggable)e;
                messages = loggable.getLogMessages();
            }
            else
            {
                messages = ToLogMessages(e);
            }

            foreach (String message in messages)
            {
                error(message);
            }
        }


        public void error(int value, String name)
        {
            this.error(LogDelegateHelper.toString(value, name));
        }

        public void error(Loggable value, String name)
        {
            this.error(LogDelegateHelper.toString(value, name));
        }


        public void error(String value, String name)
        {
            this.error(LogDelegateHelper.toString(value, name));
        }


        public void errorFormat(String format, params Object[] values)
        {
            this.error(LogDelegateHelper.FormatString(format, values));
        }

    }
}
