// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet.lib.Http.server
{
    public class HttpMethod
    {

        public static readonly HttpMethod GET = new HttpMethod( "GET" );
        public static readonly HttpMethod POST = new HttpMethod( "POST" );
		public static readonly HttpMethod PUT = new HttpMethod( "PUT" );
        public static readonly HttpMethod OPTIONS = new HttpMethod( "OPTIONS" );


        ///////////////////////////////////////////////////////////////////////
        // name
        private String _name;

        public String Name
        {
            get { return _name; }
            protected set { _name = value; }
        }

        private HttpMethod(String name)
        {
            _name = name;
        }


        public Boolean Matches(String method)
        {
            if( _name.Equals( method ) )
            { 
                return true;
            }
            return false;
        }
    }
}
