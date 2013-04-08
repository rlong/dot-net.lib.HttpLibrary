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
    public class LogDelegateHelper
    {

        public static String getMethodName(Log origin)
        {
            StackTrace st = new StackTrace(2, true);
            for (int i = 0, count = st.FrameCount; i < count; i++)
            {
                StackFrame frame = st.GetFrame(i);
                Type frameType = frame.GetMethod().DeclaringType;
                if (origin.TargetType().IsAssignableFrom(frameType))
                {
                    return frame.GetMethod().Name;
                }
            }
            return "<undefined>";
        }

        public static String toString(bool value, String name)
        {
            return name + " = " + value;
        }

        public static String toString(float value, String name)
        {
            return name + " = " + value;
        }

        public static String toString(int value, String name)
        {
            return name + " = " + value;
        }

        public static String toString(Loggable value, String name)
        {
            if (null == value)
            {
                return name + " = NULL";
            }

            String[] messages = value.getLogMessages();
            if (0 == messages.Length)
            {
                return name + " = {}";
            }


            if (1 == messages.Length)
            {
                return name + " = " + messages[0];
            }

            StringBuilder answer = new StringBuilder(name);
            answer.Append(" = {");

            foreach( String message in messages ) { 
                answer.AppendFormat( "    {0}\n", message );
            }

            answer.Append("}");

            return answer.ToString();

        }

        public static String toString(long value, String name)
        {
            return name + " = " + value;
        }

        public static String toString(Object value, String name)
        {
            if (null == value)
            {
                return name + " = NULL";
            }
            return name + " = " + value.ToString();
        }

        public static String toString(String value, String name)
        {
            if (null == value)
            {
                return name + " = NULL";
            }
            return name + " = '" + value + "'";
        }


        public static String toIp4AddressString(int value, String name)
        {

            int ip1 = value & 0xFF;
            int ip2 = (value >> 8) & 0xFF;
            int ip3 = (value >> 16) & 0xFF;
            int ip4 = (value >> 24) & 0xFF;

            String answer = String.Format("{0} = {1}.{2}.{3}.{4}", name, ip1, ip2, ip3, ip4);
            return answer;
        }

        public static String FormatString(String format, params Object[] values)
        {
            return String.Format(format, values);
        }



    }
}
