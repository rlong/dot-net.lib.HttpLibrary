// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.common.auxiliary
{
    public class ExceptionHelper
    {
        private static String toString(StackFrame frame)
        {
            String answer = "";
            MethodBase method = frame.GetMethod();

            if (null != method)
            {
                answer += "[";
                Type declaringType = method.DeclaringType;
                if (null != declaringType)
                {
                    String moduleName = declaringType.Name;
                    if (null != moduleName)
                    {
                        answer += moduleName;
                    }
                }
                String methodName = method.Name;
                if (null != methodName)
                {
                    answer += " ";
                    answer += methodName;
                }
                answer += "]";
            }


            if (0 != answer.Length)
            {
                answer += " ";
            }

            String fileName = frame.GetFileName();
            if (null == fileName || 0 == fileName.Length)
            {
                fileName = "?";
            }
            else
            {
                int lastSlash = fileName.LastIndexOf('\\');
                if (-1 != lastSlash)
                {
                    fileName = fileName.Substring(lastSlash + 1); // +1 to skip over the '\'
                }
            }


            String lineNumber = "?";
            if (0 != frame.GetFileLineNumber())
            {
                lineNumber = String.Format("{0}", frame.GetFileLineNumber());
            }

            answer += String.Format("({0}:{1})", fileName, lineNumber);

            return answer;

        }

        public static Exception getRootInnerException(Exception t)
        {

            while (null != t.InnerException)
            {
                t = t.InnerException;
            }

            return t;
        }
        	
        // will return null if there is not underlying cause
        public static String getUnderlyingFaultMessage(Exception t)
        {
            Exception underlyingFault = getRootInnerException(t);

            // guard against chaining of 'BaseException's
            if (t is BaseException)
            {
                return null;
            }

            return underlyingFault.Message;

        }

        public static String[] getStackTrace(Exception t, bool rootCause)
        {

            if (rootCause)
            {
                t = getRootInnerException(t);
            }

            StackTrace st = new StackTrace(t, true);

            String[] answer = new String[st.FrameCount];

            for (int i = 0, count = st.FrameCount; i < count; i++)
            {
                StackFrame frame = st.GetFrame(i);
                answer[i] = toString(frame).Trim();
            }

            return answer;
        }

    }
}
