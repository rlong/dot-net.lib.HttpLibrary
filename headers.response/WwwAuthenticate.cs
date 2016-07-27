// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.CoreAnnex.log;

namespace dotnet.lib.Http.headers.response
{
    // see section 3.2.1 of RFC-2617
    public class WwwAuthenticate : HttpHeader
    {

        private static Log log = Log.getLog(typeof(WwwAuthenticate));

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
            protected set { _qop = value; }
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

        public WwwAuthenticate() 
        {
            _qop = "auth,auth-int";
        }


        public static WwwAuthenticate buildFromString(String challenge)
        {
            WwwAuthenticate answer = new WwwAuthenticate();

            AuthenticationHeaderScanner authenticationHeaderScanner = new AuthenticationHeaderScanner(challenge);

            authenticationHeaderScanner.scanPastDigestString();

            String name = authenticationHeaderScanner.scanName();

            while (null != name) 
            {
                if ("nonce".Equals(name))
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
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._qop = value;

                }
                else if ("realm".Equals(name))
                {
                    String value = authenticationHeaderScanner.scanQuotedValue();

                    answer._realm = value;

                }
                else
                {
                    // 'auth-param' in section 3.2.1 of RFC-2617 says ... 
                    // Any unrecognized directive MUST be ignored.

                    String value = authenticationHeaderScanner.scanValue();

                    String warning = String.Format("unrecognised name-value pair. name = '{0}', value = '{1}'", name, value);
                    log.warn(warning);

                }
                name = authenticationHeaderScanner.scanName();

            }

            return answer;
        }

        public String getName()
        {
            return "WWW-Authenticate";
        }


        public String getValue()
        {

            /*
 WWW-Authenticate: Digest
 realm="testrealm@host.com", 
 qop="auth,auth-int", 
 nonce="dcd98b7102dd2f0e8b11d0f600bfb0c093", 
 opaque="5ccc069c403ebaf9f0171e9517f40e41"
 */

            String answer = String.Format("Digest realm=\"{0}\", nonce=\"{1}\", opaque=\"{2}\", qop=\"{3}\"", _realm, _nonce, _opaque, _qop) ;

            return answer;
        }



    }
}
