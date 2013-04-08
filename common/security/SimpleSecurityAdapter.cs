// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.common.security
{
    public class SimpleSecurityAdapter : SecurityAdapter
    {

        public static readonly String BUNDLE_NAME = "jsonbroker.SimpleSecurityAdapter";


        private static Log log = Log.getLog(typeof(SimpleSecurityAdapter));

        public String getIdentifier()
        {

            String answer = SecurityUtilities.generateNonce();
            log.debug(answer, "answer");

            return answer;
        }

    }
}
