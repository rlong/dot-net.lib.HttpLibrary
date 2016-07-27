// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.Http.headers;

namespace dotnet.lib.Http.multi_part
{
    public interface MultiPartHandler
    {
        PartHandler FoundPartDelimiter();

        void HandleException(Exception e);

        void FoundCloseDelimiter();
    }
}
