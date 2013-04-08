// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.common.broker
{
    public class BrokerMessageType
    {
        public static readonly BrokerMessageType FAULT = new BrokerMessageType("fault");
        public static readonly BrokerMessageType NOTIFICATION = new BrokerMessageType("notification");
        public static readonly BrokerMessageType ONEWAY = new BrokerMessageType("oneway");
        public static readonly BrokerMessageType META_REQUEST = new BrokerMessageType("meta-request");
        public static readonly BrokerMessageType META_RESPONSE = new BrokerMessageType("meta-response");
        public static readonly BrokerMessageType REQUEST = new BrokerMessageType("request");
        public static readonly BrokerMessageType RESPONSE = new BrokerMessageType("response");

        ////////////////////////////////////////////////////////////////////////////
        String _identifier;

        public String getIdentifier()
        {
            return _identifier;
        }


        private BrokerMessageType(String identifier)
        {
            _identifier = identifier;
        }



        public static BrokerMessageType lookup(String identifier)
        {
            if( FAULT._identifier.Equals(identifier) ) {
			    return FAULT;
		    }
		
		    if( NOTIFICATION._identifier.Equals( identifier) ) {
			    return NOTIFICATION;
		    }

		    if( ONEWAY._identifier.Equals(identifier) ) {
			    return ONEWAY;
		    }

            if (META_REQUEST._identifier.Equals(identifier))
            {
                return META_REQUEST;
            }

            if (META_RESPONSE._identifier.Equals(identifier))
            {
                return META_RESPONSE;
            }

		
		    if( REQUEST._identifier.Equals(identifier) ) {
			    return REQUEST;
		    }


		    if( RESPONSE._identifier.Equals(identifier) ) {
			    return RESPONSE;
		    }

            String technicalError = String.Format("unexpected identifier; identifier = '{0}'", identifier);
		    throw new BaseException(typeof(BrokerMessageType), technicalError);


        }




    }
}
