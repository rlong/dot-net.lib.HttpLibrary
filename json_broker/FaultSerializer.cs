// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using dotnet.lib.CoreAnnex.exception;
using dotnet.lib.CoreAnnex.log;
using System.Reflection;
using dotnet.lib.CoreAnnex.auxiliary;
using dotnet.lib.CoreAnnex.json;

namespace dotnet.lib.Http.json_broker
{
    public class FaultSerializer
    {
        // private static readonly Log log = Log.getLog(typeof(FaultSerializer));

        private static String toString(StackFrame frame)
        {
            String answer = "";
            MethodBase method = frame.GetMethod();

            if( null != method ) 
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


            if( 0 != answer.Length ) {
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
                    fileName = fileName.Substring(lastSlash+1); // +1 to skip over the '\'
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


        public static JsonObject ToJsonObject(BaseException baseException)
        {
            JsonObject answer = new JsonObject();

            answer.put("errorDomain", baseException.ErrorDomain); // null is ok

            answer.put("faultCode", baseException.FaultCode);
            answer.put("faultMessage", baseException.Message);
            answer.put("underlyingFaultMessage", baseException.UnderlyingFaultMessage);

            answer.put("originator", baseException.getOriginator());

            JsonArray jsonStackTrace;
            {

                jsonStackTrace = new JsonArray();

                String[] stackTrace = ExceptionHelper.getStackTrace(baseException, true);

                for (int i = 0, count = stackTrace.Length; i < count; i++)
                {
                    jsonStackTrace.Add(stackTrace[i]);
                }

                stackTrace = ExceptionHelper.getStackTrace(baseException, false);

                for (int i = 0, count = stackTrace.Length; i < count; i++)
                {
                    jsonStackTrace.Add(stackTrace[i]);
                }
            }

            answer.put("stackTrace", jsonStackTrace);

            JsonObject faultContext = new JsonObject();
            answer.put("faultContext", faultContext);

            return answer;

        }

        public static JsonObject ToJsonObject(Exception exception)
        {
            if (exception is BaseException)
            {
                return ToJsonObject((BaseException)exception);
            }

            JsonObject answer = new JsonObject();

            answer.put("errorDomain", null); // null is ok

            answer.put("faultCode", BaseException.DEFAULT_FAULT_CODE); // 
            answer.put("faultMessage", exception.Message);
            answer.put("underlyingFaultMessage", ExceptionHelper.getUnderlyingFaultMessage(exception));

            String[] stackTrace = ExceptionHelper.getStackTrace(exception, true);
            JsonArray jsonStackTrace = new JsonArray(stackTrace.Length);

            for (int i = 0, count = stackTrace.Length; i < count; i++)
            {
                jsonStackTrace.Add(stackTrace[i]);
            }

            answer.put("stackTrace", jsonStackTrace);
            String originator;
            {
                StackTrace st = new StackTrace(exception, true);
                StackFrame frame0 = st.GetFrame(0);
                MethodBase method = frame0.GetMethod();
                String className = method.DeclaringType.Name;
                String methodName = method.Name;
                int lineNumber = frame0.GetFileLineNumber();
                if (0 != lineNumber)
                {
                    originator = String.Format("{0}:{1:x}", className, lineNumber);
                }
                else if (null != methodName)
                {
                    originator = String.Format("{0}:{1}", className, methodName);
                }
                else
                {
                    originator = String.Format("{0}:?", className);
                }                
            }
            answer.put("originator", originator);

            JsonObject faultContext = new JsonObject();
            answer.put("faultContext", faultContext);

            return answer;
        }

        public static BaseException toBaseException(JsonObject jsonObject)
        {

            String originator = jsonObject.GetString("originator", "NULL");
            String fault_string = jsonObject.GetString("faultMessage", "NULL");

            BaseException answer = new BaseException(originator, fault_string);

            {
                String errorDomain = jsonObject.GetString("errorDomain", null);
                answer.ErrorDomain = errorDomain;
            }

            int faultCode = jsonObject.GetInt("faultCode", BaseException.DEFAULT_FAULT_CODE);
            answer.FaultCode = faultCode;

            String underlyingFaultMessage = jsonObject.GetString("underlyingFaultMessage", null);
            answer.UnderlyingFaultMessage = underlyingFaultMessage;

            JsonArray stack_trace = jsonObject.GetJsonArray("stackTrace", null);
            if (null != stack_trace)
            {
                for (int i = 0, count = stack_trace.Count(); i < count; i++)
                {
                    String key = "cause[" + i + "]";
                    String value = stack_trace.GetString(i, "NULL");
                    answer.addContext(key, value);
                }
            }

            JsonObject fault_context = jsonObject.GetJsonObject("faultContext", null);
            if (null != fault_context)
            {
                foreach (KeyValuePair<string, object> kvp in fault_context)
                {
                    Object value = kvp.Value;
                    if (null != value && value is String)
                    {
                        String key = kvp.Key;
                        answer.addContext(key, (String)value);
                    }
                }
            }
            return answer;
        }
    }
}
