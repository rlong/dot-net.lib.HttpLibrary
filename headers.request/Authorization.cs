// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.CoreAnnex.log;

namespace dotnet.lib.Http.headers.request
{
    public class Authorization : HttpHeader
    {

        private static Log log = Log.getLog(typeof(Authorization));


        ///////////////////////////////////////////////////////////////////////
        // cnonce
        String _cnonce;

        public String cnonce
        {
            get { return _cnonce; }
            set { _cnonce = value; }
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
        // nonce
        String _nonce;

        public String nonce
        {
            get { return _nonce; }
            set { _nonce = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // opaque
        String _opaque;


        public String opaque
        {
            get { return _opaque; }
            set { _opaque = value; }
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
        // realm
        String _realm;

        public String realm
        {
            get { return _realm; }
            set { _realm = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // response
        String _response;

        public String response
        {
            get { return _response; }
            set { _response = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // uri

        String _uri;

        public String uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        // username

        String _username;

        public String username
        {
            get { return _username; }
            set { _username = value; }
        }

        ///////////////////////////////////////////////////////////////////////

        public Authorization() 
        {
            _qop = "auth-int";
        }


        ///////////////////////////////////////////////////////////////////////


        public static Authorization buildFromString( String credentials ) {

            Authorization answer = new Authorization();

            AuthenticationHeaderScanner authenticationHeaderScanner = new AuthenticationHeaderScanner(credentials);

            authenticationHeaderScanner.scanPastDigestString();


            String name = authenticationHeaderScanner.scanName();

            while (null != name)
            {
                if ("cnonce".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._cnonce = value;
                }
                else if ("nc".Equals(name))
                {
                    uint value = authenticationHeaderScanner.scanHexUInt32();

                    answer._nc = value;
                }
                else if ("nonce".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._nonce = value;

                }
                else if ("opaque".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._opaque = value;
                }
                else if ("qop".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanValue();

                    answer._qop = value;
                }
                else if ("realm".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._realm = value;

                }
                else if ("response".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._response = value;
                }
                else if ("uri".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._uri = value;

                }
                else if ("username".Equals(name))
                {

                    String value = authenticationHeaderScanner.scanQuotedValue();
                    log.debug(value, "value");

                    answer._username = value;

                }
                else
                {
                    // 'auth-param' is permitted according to 3.2.2 of RFC-2617
                    // 'auth-param' in section 3.2.1 of RFC-2617 says ... 
                    // Any unrecognized directive MUST be ignored.
                    // 

                    String value = authenticationHeaderScanner.scanValue();

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
            return "Authorization";
        }


        public String getValue()
        {

            String answer = String.Format("Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", response=\"{4}\", cnonce=\"{5}\", opaque=\"{6}\", qop={7}, nc={8:x8}", _username, _realm, _nonce, _uri, _response, _cnonce, _opaque, _qop, _nc);

            return answer;
        }

    }
}
