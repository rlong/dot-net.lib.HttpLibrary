// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace jsonbroker.library.common.auxiliary
{
    public class FileUtilities
    {

        public static long GetFileLength(String path)
        {
            // instances of 'FileInfo' represent a snapshot of the file state
            // when the instance was created, it will *NOT* be updated 
            // when a file grows ...
            FileInfo fileInfo = new System.IO.FileInfo(path);
            return fileInfo.Length;
        }
    }
}
