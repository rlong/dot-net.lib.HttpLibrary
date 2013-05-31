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
    public class ListenedJob : Job
    {
        ///////////////////////////////////////////////////////////////////////
        // delegate
        private Job _delegate;

        ///////////////////////////////////////////////////////////////////////
        // listener
        private JobListener _listener;

        public ListenedJob( Job jobDelegate, JobListener listener ) 
        {
            _delegate = jobDelegate;
            _listener = listener;    
        }


        public void execute()
        {
            try
            {
                _delegate.execute();
                _listener.jobCompleted(_delegate);

            }
            catch (BaseException e)
            {
                _listener.jobFailed(_delegate, e);
            }

        }

    }
}
