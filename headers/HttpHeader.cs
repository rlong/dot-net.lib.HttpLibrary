// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet.lib.Http.headers
{
    public interface HttpHeader
    {

        String getName();
        String getValue();

    }
}
