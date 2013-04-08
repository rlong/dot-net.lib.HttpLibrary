// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace jsonbroker.library.common.work
{
    class WorkQueue
    {

        private Queue<Job> _queue;

        public WorkQueue()
        {
            _queue = new Queue<Job>();
        }

        public void enqueue(Job job)
        {
            // vvv http://jaksa76.blogspot.com/2009/03/blocking-queue-in-c.html

            lock (_queue)
            {
                _queue.Enqueue(job);
                Monitor.Pulse(_queue);
            }

            // ^^^ http://jaksa76.blogspot.com/2009/03/blocking-queue-in-c.html
        }

        public Job dequeue()
        {
            Job answer;

            // vvv http://jaksa76.blogspot.com/2009/03/blocking-queue-in-c.html

            lock (_queue)
            {
                while (0 == _queue.Count)
                {
                    Monitor.Wait(_queue);
                }
                answer = _queue.Dequeue();
            }

            // ^^^ http://jaksa76.blogspot.com/2009/03/blocking-queue-in-c.html

            return answer;
        }
    }
}
