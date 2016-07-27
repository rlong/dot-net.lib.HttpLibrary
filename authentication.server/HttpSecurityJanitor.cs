// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.CoreAnnex.log;
using System.Threading;

namespace dotnet.lib.Http.authentication.server
{
    public class HttpSecurityJanitor
    {
        private static readonly Log log = Log.getLog(typeof(HttpSecurityJanitor));

        HttpSecurityManager _httpSecurityManager;


        public HttpSecurityJanitor(HttpSecurityManager httpSecurityManager)
        {
            _httpSecurityManager = httpSecurityManager;
        }


        public void run()
        {

            log.enteredMethod();

            try
            {
                while (true)
                {

                    System.Threading.Thread.Sleep(2 * 60 * 1000);
                    _httpSecurityManager.runCleanup();
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
            thread.Name = "HttpSecurityJanitor";
            thread.IsBackground = true;
            thread.Start();
        }


    }
}
