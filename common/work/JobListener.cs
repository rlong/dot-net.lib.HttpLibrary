// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.common.work
{
    public interface JobListener
    {
        void jobCompleted(Job job);
        void jobFailed(Job job, BaseException exception);
    }
}

