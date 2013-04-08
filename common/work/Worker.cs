// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.work
{
    class Worker
    {

        private static readonly Log log = Log.getLog(typeof(Worker));

        /////////////////////////////////////////////////////////
        // name
        private String _name;

        /////////////////////////////////////////////////////////
        // workQueue
        WorkQueue _workQueue;


        /////////////////////////////////////////////////////////
        public Worker(String name, WorkQueue workQueue)
        {
            _name = name;
            _workQueue = workQueue;
        }


	    public void run() {

		    log.enteredMethod();

            try
            {
                while (true)
                {

                    Job job = _workQueue.dequeue();
                    try
                    {
                        job.execute();
                    }
                    catch (Exception e)
                    {
                        log.error(e);
                    }
                }
            }
            finally
            {
                log.info("leaving");
            }
        }


        public void start()
        {
            Thread thread = new Thread(new ThreadStart(this.run));
            thread.Name = _name;
            thread.IsBackground = true;
            thread.Start();
        }


    }
}
