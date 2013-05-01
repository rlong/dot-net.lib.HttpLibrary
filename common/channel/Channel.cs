// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.channel
{
    public interface Channel
    {

        void Close(bool ignoreErrors);

        String ReadLine();
        byte[] ReadBytes(int count);


        void Write(byte[] bytes);
        void Write(String line);
        void WriteLine(String line);

    }
}
