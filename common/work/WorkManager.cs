// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.work
{
    public class WorkManager
    {

        private static readonly Log log = Log.getLog(typeof(WorkManager));

        ////////////////////////////////////////////////////////////////////////////
        static WorkQueue _workQueue = new WorkQueue();

        ////////////////////////////////////////////////////////////////////////////
        static Worker[] _workers = null;

        public static void start()
        {

            log.enteredMethod();

            if (null != _workers)
            {
                log.warn("null != _workers");
                return;
            }
            _workers = new Worker[5];

            for (int i = 0; i < _workers.Length; i++)
            {

                String name = String.Format("worker.{0}", i + 1);

                _workers[i] = new Worker(name, _workQueue);
                _workers[i].start();
            }		
        }

        public static void enqueue(Job job)
        {
            if (null == _workers)
            {
                log.warn("null == _workers");
            }

            _workQueue.enqueue(job);
        }

        public static void enqueue(Job job, JobListener jobListener)
        {

            if (null == _workers)
            {
                log.warn("null == _workers");
            }

            ListenedJob listenedJob = new ListenedJob(job, jobListener);
            _workQueue.enqueue(listenedJob);
        }

    }
}
