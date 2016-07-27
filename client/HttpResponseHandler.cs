// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.Http;

namespace dotnet.lib.Http.client
{
    public interface HttpResponseHandler
    {

        // the entity can be null ... in the case of a http 204 response
        void handleResponseEntity(System.Net.WebHeaderCollection headers, Entity responseEntity);

    }
}
