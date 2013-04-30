// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;
using jsonbroker.library.common.json;
using System.Configuration;

namespace jsonbroker.library.common.defaults
{
    public class DefaultsHelper
    {

        private static Log log = Log.getLog(typeof(DefaultsHelper));

        public static Defaults GetDefaults(String scope)
        {

            // command line ...
            {
                CommandLineArgs commandLineArgs = CommandLineArgs.getInstance();
                String argumentValue = commandLineArgs.GetArgument(scope);
                if (null != argumentValue)
                {
                    log.debug(argumentValue, "argumentValue");
                    JsonObject environment = JsonObjectHelper.FromString(argumentValue);
                    return new Defaults(environment);
                }
            }

            // environment ... 
            {
                // vvv http://stackoverflow.com/questions/2821043/allowed-characters-in-linux-environment-variable-names

                String environmentName = scope.Replace('.', '_');

                // ^^^ http://stackoverflow.com/questions/2821043/allowed-characters-in-linux-environment-variable-names
                String environmentValue = Environment.GetEnvironmentVariable(environmentName, EnvironmentVariableTarget.Process);
                if (null != environmentValue)
                {
                    log.debug(environmentValue, "environmentVariable");
                    JsonObject environment = JsonObjectHelper.FromString(environmentValue);
                    return new Defaults(environment);
                }
            }

            // settings ... 
            {

                String settingsValue = ConfigurationManager.AppSettings.Get(scope);
                if (null != settingsValue)
                {
                    log.debug(settingsValue, "settingsValue");
                    JsonObject environment = JsonObjectHelper.FromString(settingsValue);
                    return new Defaults(environment);
                }

            }

            // empty ...
            JsonObject emptyEnvironment = new JsonObject(); 
            //JsonObject environment = new JsonObject(); // Error	1	A local variable named 'environment' cannot be declared in this scope because it would give a different meaning to 'environment', which is already used in a 'child' scope to denote something else	H:\projects.windows\c-sharp\vlc_amigo.solution\jsonbroker.c_sharp.library\common\defaults\DefaultsHelper.cs	53	24	jsonbroker.library
            return new Defaults(emptyEnvironment);

        }
    }
}
