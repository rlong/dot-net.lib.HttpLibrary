// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.http.headers.response
{
    public class AuthenticationInfo : HttpHeader
    {

        private static Log log = Log.getLog(typeof(AuthenticationInfo));


        ///////////////////////////////////////////////////////////////////////
        // cnonce
        String _cnonce;

        public String cnonce
        {
            get { return _cnonce; }
            set { _cnonce = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        // nextnonce
        String _nextnonce;

        public String nextnonce
        {
            get { return _nextnonce; }
            set { _nextnonce = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        // nc
        long _nc;

        public long nc
        {
            get { return _nc; }
            set { _nc = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        // qop
        String _qop;


        public String qop
        {
            get { return _qop; }
            set { _qop = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        // rspauth
        String _rspauth;


        public String rspauth
        {
            get { return _rspauth; }
            set { _rspauth = value; }
        }

        ///////////////////////////////////////////////////////////////////////

        public AuthenticationInfo() 
        {
            _qop = "auth-int";
        }

        ///////////////////////////////////////////////////////////////////////

        public static AuthenticationInfo buildFromString(String authInfo)
        {
            AuthenticationInfo answer = new AuthenticationInfo();

            AuthenticationHeaderScanner authenticationHeaderScanner = new AuthenticationHeaderScanner(authInfo);

            authenticationHeaderScanner.scanPastDigestString();

            String name = authenticationHeaderScanner.scanName();

            while (null != name)
            {
                if ("cnonce".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._cnonce = value;
                }
                else if ("nextnonce".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._nextnonce = value;
                    
                }
                else if ("nc".Equals(name))
                {
                    uint value = authenticationHeaderScanner.scanHexUInt32();

                    answer._nc = value;
                }
                else if ("qop".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanValue();

                    answer._qop = value;
                }
                else if ("rspauth".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._rspauth = value;

                } else {

                    // 'auth-param' is not permitted according to 3.2.3 of RFC-2617
                    // 'auth-param' in section 3.2.1 of RFC-2617 says ... 
                    // Any unrecognized directive MUST be ignored.
                    //
                    // For consistency, we mimic the behaviour specified in sections 3.2.1 & 3.2.2 in relation to 'auth-param'

                    String value = authenticationHeaderScanner.scanQuotedValue();
                    String warning = String.Format("unrecognised name-value pair. name = '{0}', value = '{1}'", name, value);
                    log.warn(warning);

                }

                name = authenticationHeaderScanner.scanName();

            }




            return answer;

        }

        ///////////////////////////////////////////////////////////////////////

        public String getName()
        {
            return "Authentication-Info";
        }


        public String getValue()
        {

            String answer = String.Format("nextnonce=\"{0}\", qop={1}, rspauth=\"{2}\", cnonce=\"{3}\", nc={4:x8}", _nextnonce, _qop, _rspauth, _cnonce, _nc);

            return answer;
        }

    }
}
