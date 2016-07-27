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
    public class PartHandlerHelper
    {


        public static ContentDisposition getContentDisposition(String name, String value)
        {
            if ("content-disposition".Equals(name))
            {
                return ContentDisposition.buildFromString(value);
            }

            return null;
        }


        public static MediaType getContentType(String name, String value)
        {
            if ("content-type".Equals(name))
            {
                return MediaType.buildFromString(value);
            }

            return null;
        }

    }
}
