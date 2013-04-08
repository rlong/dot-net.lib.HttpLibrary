// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.json.handlers;

namespace jsonbroker.library.common.json.output
{




    public class JsonWriter : JsonDocumentHandler
    {
        JsonOutput _output;

        bool _objectStarted;

        public JsonWriter(JsonOutput output)
        {
            _output = output;
            _objectStarted = false;
        }


        ////////////////////////////////////////////////////////////////////////////
        // document

        public void onObjectDocumentStart()
        {

            _output.append('{');
            _objectStarted = true;

        }

        public void onObjectDocumentEnd()
        {
            _output.append('}');
            _objectStarted = false;
        }

        public void onArrayDocumentStart()
        {
            _output.append('[');

        }

        public void onArrayDocumentEnd()
        {
            _output.append(']');
        }


        ////////////////////////////////////////////////////////////////////////////
        // array

        public void onArrayStart(int index)
        {
            if (0 != index)
            {
                _output.append(',');
            }

            _output.append('[');

        }


        public void onArrayEnd(int index)
        {
            _output.append(']');

        }



        public void onBoolean(int index, bool value)
        {
            if (0 != index)
            {
                _output.append(',');
            }

            JsonBooleanHandler.WriteBoolean(value, _output);

        }


        public void onNull(int index)
        {
            if (0 != index)
            {
                _output.append(',');
            }

            JsonNullHandler.WriteNull(_output);

        }

        public void onNumber(int index, double value)
        {
            if (0 != index)
            {
                _output.append(',');
            }

            JsonNumberHandler.WriteNumber(value, _output);


        }


        public void onNumber(int index, int value)
        {
            if (0 != index)
            {
                _output.append(',');
            }

            JsonNumberHandler.WriteNumber(value, _output);
        }


        public void onNumber(int index, float value)
        {
            if (0 != index)
            {
                _output.append(',');
            }

            JsonNumberHandler.WriteNumber(value, _output);
        }


        public void onNumber(int index, long value)
        {
            if (0 != index)
            {
                _output.append(',');
            }

            JsonNumberHandler.WriteNumber(value, _output);
        }



        public void onObjectStart(int index)
        {
            if (0 != index)
            {
                _output.append(',');
            }


            _output.append('{');
            _objectStarted = true;

        }


        public void onObjectEnd(int index)
        {
            _output.append('}');
            _objectStarted = false;

        }


        public void onString(int index, String value)
        {
            if (0 != index)
            {
                _output.append(',');
            }

            JsonStringHandler.WriteString(value, _output);

        }




        ////////////////////////////////////////////////////////////////////////////
        // object

        public void onArrayStart(String key)
        {
            if (_objectStarted)
            {
                _objectStarted = false;
            }
            else
            {
                _output.append(',');
            }

            JsonStringHandler.WriteString(key, _output);
            _output.append(":[");


        }


        public void onArrayEnd(String key)
        {
            _output.append(']');
        }



        public void onBoolean(String key, bool value)
        {
            if (_objectStarted)
            {
                _objectStarted = false;
            }
            else
            {
                _output.append(',');
            }

            JsonStringHandler.WriteString(key, _output);
            _output.append(':');

            JsonBooleanHandler.WriteBoolean(value, _output);

        }


        public void onNull(String key)
        {
            if (_objectStarted)
            {
                _objectStarted = false;
            }
            else
            {
                _output.append(',');
            }

            JsonStringHandler.WriteString(key, _output);
            _output.append(':');

            JsonNullHandler.WriteNull(_output);

        }


        public void onNumber(String key, double value)
        {
            if (_objectStarted)
            {
                _objectStarted = false;
            }
            else
            {
                _output.append(',');
            }

            JsonStringHandler.WriteString(key, _output);
            _output.append(':');

            JsonNumberHandler.WriteNumber(value, _output);

        }

        public void onNumber(String key, int value)
        {
            if (_objectStarted)
            {
                _objectStarted = false;
            }
            else
            {
                _output.append(',');
            }

            JsonStringHandler.WriteString(key, _output);
            _output.append(':');

            JsonNumberHandler.WriteNumber(value, _output);

        }


        public void onNumber(String key, float value)
        {
            if (_objectStarted)
            {
                _objectStarted = false;
            }
            else
            {
                _output.append(',');
            }

            JsonStringHandler.WriteString(key, _output);
            _output.append(':');

            JsonNumberHandler.WriteNumber(value, _output);
        }


        public void onNumber(String key, long value)
        {
            if (_objectStarted)
            {
                _objectStarted = false;
            }
            else
            {
                _output.append(',');
            }

            JsonStringHandler.WriteString(key, _output);
            _output.append(':');

            JsonNumberHandler.WriteNumber(value, _output);
        }



        public void onObjectStart(String key)
        {
            if (_objectStarted)
            {
                _objectStarted = false;
            }
            else
            {
                _output.append(',');
            }

            JsonStringHandler.WriteString(key, _output);
            _output.append(":{");
            _objectStarted = true;

        }


        public void onObjectEnd(String key)
        {
            _output.append('}');
            _objectStarted = false;

        }

        public void onString(String key, String value)
        {
            if (_objectStarted)
            {
                _objectStarted = false;
            }
            else
            {
                _output.append(',');
            }

            JsonStringHandler.WriteString(key, _output);
            _output.append(':');

            JsonStringHandler.WriteString(value, _output);

        }



    }
}
