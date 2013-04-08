// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace jsonbroker.library.common.log
{
    public class LogHelper
    {


        private static String getCallerClassName(Object caller)
        {
            Type callerClass;

            if (caller is Type)
            {
                callerClass = (Type)caller;
            }
            else
            {
                callerClass = caller.GetType();
            }

            return callerClass.Name;
        }


        public static void setupDebugLog(Object caller)
        {
            String message = String.Format("[LogHelper setupDebugConsoleLog] settting up debug log for '{0}'", getCallerClassName(caller));
            Console.WriteLine(message);
            Trace.WriteLine(message);

            ConsoleLogDelegate consoleLog = new ConsoleLogDelegate(true);
            Log.setDelegate(consoleLog);
        }

        public static void setupDebugLog(Object caller, LogDelegate logDelegate)
        {
            String message = String.Format("[LogHelper setupDebugConsoleLog] settting up debug log for '{0}'", getCallerClassName(caller));
            Console.WriteLine(message);
            Trace.WriteLine(message);

            Log.setDelegate(logDelegate);
        }


        public static void setupReleaseLog(Object caller)
        {
            String message = String.Format("[LogHelper setupReleaseTraceLog] settting up release log for '{0}'", getCallerClassName(caller));
            Console.WriteLine(message);
            Trace.WriteLine(message);

            TraceLogDelegate traceLog = new TraceLogDelegate(false);
            ReleaseLogDelegate releaseLogFilter = new ReleaseLogDelegate(traceLog);
            Log.setDelegate(releaseLogFilter);

        }

        public static void setupReleaseLog(Object caller, LogDelegate logDelegate)
        {
            String message = String.Format("[LogHelper setupReleaseTraceLog] settting up release log for '{0}'", getCallerClassName(caller));
            Console.WriteLine(message);
            Trace.WriteLine(message);


            TraceLogDelegate traceLog = new TraceLogDelegate(false);
            DoublingLogDelegate logDoubleDelegator = new DoublingLogDelegate(false, traceLog, logDelegate);
            ReleaseLogDelegate releaseLogFilter = new ReleaseLogDelegate(logDoubleDelegator);
            Log.setDelegate(releaseLogFilter);

        }


    }
}
