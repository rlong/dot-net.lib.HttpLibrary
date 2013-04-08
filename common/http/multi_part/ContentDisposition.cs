// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.exception;
using jsonbroker.library.common.log;
using jsonbroker.library.common.http.headers;

namespace jsonbroker.library.common.http.multi_part
{
    /// <summary>
    /// as per http://www.w3.org/Protocols/rfc2616/rfc2616-sec19.html#sec19.5.1
    /// </summary>
    public class ContentDisposition : HttpHeader
    {

        private static Log log = Log.getLog(typeof(ContentDisposition));

        ///////////////////////////////////////////////////////////////////////
        // dispExtensionToken ... can be null, see http://www.w3.org/Protocols/rfc2616/rfc2616-sec19.html#sec19.5.1
        private String _dispExtensionToken;

        public String DispExtensionToken
        {
            get { return _dispExtensionToken; }
            protected set { _dispExtensionToken = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // value
        private String _value;

        public String getValue()
        {
            return _value;
        }


        ///////////////////////////////////////////////////////////////////////
        // dispositionParameters 
        private Dictionary<String, String> _dispositionParameters;


        ///////////////////////////////////////////////////////////////////////
        // 
        private ContentDisposition(String value)
        {
            _value = value;
            _dispositionParameters = new Dictionary<string, string>();
        }


        public String getName()
        {
            return "Content-Disposition";
        }

        public String getDispositionParameter(String parameterName, String defaultValue)
        {
            if (!_dispositionParameters.ContainsKey(parameterName))
            {
                return defaultValue;
            }
            return _dispositionParameters[parameterName];
        }


        public static ContentDisposition buildFromString(String value)
        {

            // see http://www.w3.org/Protocols/rfc2616/rfc2616-sec19.html#sec19.5.1 for rules

            ParametersScanner scanner = new ParametersScanner(0, value);
            String firstAttribute = scanner.NextAttribute();
            if (null == firstAttribute)
            {
                BaseException e = new BaseException(typeof(ContentDisposition), "null == firstAttribute; value = {0}", value);
                throw e;
            }
            String dispExtensionToken = null;
            if ("attachment".Equals(firstAttribute))
            {
                // we leave _dispExtensionToken as null
            }
            else
            {
                dispExtensionToken = firstAttribute;            
            }


            ContentDisposition answer = new ContentDisposition(value);

            answer._dispExtensionToken = dispExtensionToken;


            for( String attribute = scanner.NextAttribute(); null != attribute; attribute = scanner.NextAttribute() ) 
            {
                log.debug(attribute, "attribute");
                String attributeValue = scanner.nextValue();
                log.debug(attributeValue, "attributeValue");
                answer._dispositionParameters[attribute] = attributeValue;
            }

            return answer;
        }




    }
}
