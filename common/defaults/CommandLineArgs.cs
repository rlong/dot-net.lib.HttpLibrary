// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.defaults
{
    internal class CommandLineArgs
    {

        private static Log log = Log.getLog(typeof(CommandLineArgs));


        private static CommandLineArgs _instance;

        Dictionary<String, String> _commandLineArgs;


        private CommandLineArgs()
        {
            _commandLineArgs = new Dictionary<String, String>();

            String[] commandLineArgs = Environment.GetCommandLineArgs();
            foreach (String commandLineArg in commandLineArgs)
            {
                log.debug(commandLineArg, "commandLineArg");
                int indexOfEquals = commandLineArg.IndexOf('=');
                if (-1 != indexOfEquals )
                {
                    String name = commandLineArg.Substring(0, indexOfEquals);
                    log.debug(name, "name");
                    String value = commandLineArg.Substring(indexOfEquals+1 ); // '+1' to skip over the '='
                    log.debug(value, "value");
                    _commandLineArgs[name] = value;
                }
            }
        }


        public static CommandLineArgs getInstance()
        {
            if( null != _instance ) { 
                return _instance;
            }


            _instance = new CommandLineArgs();
            return _instance;
        }

        public String GetArgument(String name)
        {
            if (!_commandLineArgs.ContainsKey(name))
            {
                return null;
            }
            return _commandLineArgs[name];
        }
    }
}
