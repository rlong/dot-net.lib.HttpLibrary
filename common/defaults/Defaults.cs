// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.json;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.defaults
{
    public class Defaults
    {

        private static Log log = Log.getLog(typeof(Defaults));

        ///////////////////////////////////////////////////////////////////////
        // environment
        private JsonObject _environment;

        //public JsonObject Environment
        //{
        //    get { return _environment; }
        //    set { _environment = value; }
        //}


        public Defaults(JsonObject environment)
        {
            _environment = environment;
        }

        public bool GetBoolean(String name, bool defaultValue)
        {
            return _environment.GetBoolean(name, defaultValue);
        }

        public int GetInt(String name, int defaultValue)
        {
            return _environment.GetInt(name, defaultValue);
        }

        public JsonObject GetJsonObject(String name, JsonObject defaultValue)
        {
            return _environment.GetJsonObject(name, defaultValue);
        }

    }
}