// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace dotnet.lib.Http
{
    public interface Entity
    {
        Stream getContent();

        long getContentLength();

        String getMimeType();

        // can return null. depends on the underlying object and how it was built
        String md5();


        void WriteTo(Stream destination, long offset, long length);


    }

}