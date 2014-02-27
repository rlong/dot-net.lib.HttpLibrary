// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.json;
using System.Diagnostics;
using jsonbroker.library.common.log;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.common.exception
{
    public class BaseException : Exception, Loggable
    {

        private static readonly int BASE_ERROR_CODE = ErrorCodeUtilities.getBaseErrorCode("jsonbroker.BaseException");

        public static readonly int DEFAULT_FAULT_CODE = BASE_ERROR_CODE | 0x01;


        ///////////////////////////////////////////////////////////////////////
        // 
        private Dictionary<String, String> _context;

        /////////////////////////////////////////////////////////
        // errorDomain
        private String _errorDomain;

        public String ErrorDomain
        {
            get { return _errorDomain; }
            set { _errorDomain = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        // from XML-RPC
        private int _faultCode;
        public int FaultCode
        {
            get { return _faultCode; }
            set { _faultCode = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // 
        private Object _originator;
        protected Object Originator
        {
            get { return _originator; }
            set { _originator = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // underlyingFaultMessage
        private String _underlyingFaultMessage;

        public String UnderlyingFaultMessage
        {
            get { return _underlyingFaultMessage; }
            set { _underlyingFaultMessage = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        // 
        public BaseException(Object originatingObject, String format, params object[] formatParams)
            : base( String.Format( format, formatParams ) )
        {
            _originator = originatingObject;
            _faultCode = BaseException.DEFAULT_FAULT_CODE;
            _context = new Dictionary<String, String>();
        }

        public BaseException(Object originatingObject, String faultString)
            : base(faultString)
        {
            _originator = originatingObject;
            _faultCode = BaseException.DEFAULT_FAULT_CODE;
            _context = new Dictionary<String, String>();
        }



        public BaseException(Exception cause, Object originatingObject, String faultString)
            : base(faultString,cause)
        {
            _originator = originatingObject;
            _faultCode = BaseException.DEFAULT_FAULT_CODE;
            _context = new Dictionary<String, String>();
        }


        public BaseException(Object originatingObject, int faultCode, String faultString)
            : base(faultString)
        {
            _originator = originatingObject;
            _faultCode = faultCode;
            _context = new Dictionary<String, String>();
        }

        public BaseException(Object originatingObject, Exception cause) : base(cause.Message, cause )
        {
            _originator = originatingObject;
            _faultCode = BaseException.DEFAULT_FAULT_CODE;
            _context = new Dictionary<String, String>();
            _underlyingFaultMessage = ExceptionHelper.getUnderlyingFaultMessage(cause);
        }


        public String getOriginator()
        {
            Type originatingClass = null;
            if (_originator is Type)
            {
                originatingClass = (Type)_originator;
            }
            else
            {
                originatingClass = _originator.GetType();
            }

            int lineNumber = 0;
            String methodName = null;

            StackTrace st = new StackTrace(this);
            for (int i = 0, count = st.FrameCount; i < count; i++)
            {
                StackFrame frame = st.GetFrame(i);
                Type frameType = frame.GetMethod().DeclaringType;
                if (!frameType.IsAssignableFrom(originatingClass)) // is originatingClass (Derived) in the hierarchy of frameType (Base)
                {
                    continue;
                }                
                lineNumber = frame.GetFileLineNumber();
                methodName = frame.GetMethod().Name;
            }


            String originatingClassName = originatingClass.Name;

            if (0 != lineNumber)
            {
                return String.Format("{0}:{1}", originatingClassName, lineNumber);
            }

            if (null != methodName)
            {
                return String.Format("[{0} {1}]", originatingClassName, methodName);
            }

            return originatingClassName;
            
        }

        public void addContext(String key, long value)
        {
            _context[key] = value.ToString();
        }


        public void addContext(String key, String value)
        {
            _context[key] = value;
        }

        public String[] getLogMessages()
        {

            List<String> list = new List<String>();

            if (null != _errorDomain)
            {
                list.Add("errorDomain = '" + this.ErrorDomain + "'");
            }
            else
            {
                list.Add("errorDomain = null");
            }
            
            list.Add("faultCode = " + this.FaultCode);
            list.Add("faultMessage = " + this.Message);
            list.Add("underlyingFaultMessage = " + this._underlyingFaultMessage);
            list.Add("className = " + this.GetType().ToString());
            list.Add("originator = " + this.getOriginator());

            {
                list.Add("stack = {");

                String[] stackTrace = ExceptionHelper.getStackTrace(this, true);
                for (int i = 0, count = stackTrace.Length; i < count; i++)
                {
                    list.Add("    " + stackTrace[i]);
                }

                stackTrace = ExceptionHelper.getStackTrace(this, false);
                for (int i = 0, count = stackTrace.Length; i < count; i++)
                {
                    list.Add("    " + stackTrace[i]);
                }

                list.Add("}");
            }

            String[] answer = new String[list.Count];
            list.CopyTo(answer);
            return answer;

        }
    }
}
