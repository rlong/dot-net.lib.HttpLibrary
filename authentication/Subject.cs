// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.Http;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.Http.headers;
using dotnet.lib.Http.headers.request;
using dotnet.lib.Http.server;
using dotnet.lib.CoreAnnex.json.output;
using dotnet.lib.CoreAnnex.auxiliary;

namespace dotnet.lib.Http.authentication
{
    public class Subject
    {
        public static readonly String TEST_USER = "test";
        public static readonly String TEST_REALM = "test";
        public static readonly String TEST_PASSWORD = "12345678";

        public static readonly Subject TEST = new Subject(TEST_USER, TEST_REALM, TEST_PASSWORD, "Test User");


        private static Log log = Log.getLog(typeof(Subject));

        ///////////////////////////////////////////////////////////////////////
        //
        String _ha1;

        public String getHa1() {
            return _ha1;

        }

        ///////////////////////////////////////////////////////////////////////
        //
        String _username;

        public String Username
        {
            get { return _username; }
            protected set { _username = value; }
        }


        /////////////////////////////////////////////////////////
        private String _realm;

        public String Realm
        {
            get { return _realm; }
            protected set { _realm = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        //
        String _password;

        public String Password
        {
            get { return _password; }
            protected set { _password = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        //
        String _label;

        public String Label
        {
            get { return _label; }
            protected set { _label = value; }
        }


        ///////////////////////////////////////////////////////////////////////
        DateTime _born;
        public DateTime born
        {
            get { return _born; }
            protected set { _born = value; }
        }



        ///////////////////////////////////////////////////////////////////////
        public Subject(String username, String usersRealm, String password, String label)
        {
            _username = username;
            _realm = usersRealm;
            _password = password;
            _label = label;

            _born = DateTime.Now;

        }

        // sections 3.2.2.1-3.2.2.3 of RFC-2617
        public String ha1()
        {
            if (null == _ha1)
            {

                String a1 = String.Format("{0}:{1}:{2}", this.Username, _realm, this.Password);
                _ha1 = SecurityUtilities.md5HashOfString(a1);
                log.debug(_ha1, "_ha1");
            }
            return _ha1;
        }

        public void validateAuthorizationRequestHeader(Authorization authorizationRequestHeader)
        {
            String realm = authorizationRequestHeader.realm;

            // realm ... 
            if (null == realm)
            {
                log.error("null == realm");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            
            if (!_realm.Equals(realm))
            {
                log.errorFormat("!_realm.Equals(realm); _realm = '{0}'; realm = '{1}'", _realm, realm);
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            // username ...
            String username = authorizationRequestHeader.username;

            if (null == username)
            {
                log.error("null == username");
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }
            else if (!_username.Equals(username)) // someone is switching user names 
            {
                log.errorFormat("!_username.Equals(username); _username = '{0}'; username = '{1}'", _username, username);
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }
        }
    }
}
