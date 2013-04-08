// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.exception;
using System.IO;
using jsonbroker.library.common.json.input;
using jsonbroker.library.common.json.handlers;
using jsonbroker.library.common.json.output;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.common.json
{
    public class JsonObject
    {

        private static readonly JsonObjectHandler JSON_OBJECT_HANDLER = JsonObjectHandler.getInstance();

        Dictionary<String, Object> _values;

        public JsonObject(int capacity)
        {
            _values = new Dictionary<string, object>(capacity);
        }

        public JsonObject()
        {
            _values = new Dictionary<string, object>();
        }



        public bool contains(String key)
        {
            if (_values.ContainsKey(key))
            {
                return true;
            }

            return false;
        }


        public bool getBoolean(String key)
        {
            if (!_values.ContainsKey(key))
            {
                String technicalError = String.Format("!_values.ContainsKey(key); key = {0}", key);
                throw new BaseException(this, technicalError);
            }

            Object blob = _values[key];

            if (null == blob)
            {
                String technicalError = String.Format("null == blob; key = {0}", key);
                throw new BaseException(this, technicalError);
            }

            if (!(blob is bool))
            {
                String technicalError = String.Format("!(blob is bool); key = {0}; blob.GetType().Name = {1}", key, blob.GetType().Name);
                throw new BaseException(this, technicalError);

            }

            return (bool)blob;

        }


        public bool getBoolean(String key, bool defaultValue )
        {
            if (!_values.ContainsKey(key))
            {
                return defaultValue;
            }

            Object blob = _values[key];

            if (null == blob)
            {
                return defaultValue;
            }

            if (!(blob is bool))
            {
                return defaultValue;
            }

            return (bool)blob;

        }

        public int getInteger(String key)
        {
            if (!_values.ContainsKey(key))
            {
                String technicalError = String.Format("!_values.ContainsKey(key); key = {0}", key);
                throw new BaseException(this, technicalError);
            }

            Object blob = _values[key];

            if (null == blob)
            {
                String technicalError = String.Format("null == blob; key = {0}", key);
                throw new BaseException(this, technicalError);
            }

            if (!(blob is int))
            {
                String technicalError = String.Format("!(blob is int); key = {0}; blob.GetType().Name = {1}", key, blob.GetType().Name);
                throw new BaseException(this, technicalError);

            }

            return (int)blob;

        }

        public int getInteger(String key, int defaultValue)
        {

            if (!_values.ContainsKey(key))
            {
                return defaultValue;
            }

            Object blob = _values[key];

            if (null == blob)
            {
                return defaultValue;
            }

            if (!(blob is int))
            {
                return defaultValue;
            }

            return (int)blob;

        }


        public JsonArray getJsonArray(String key)
        {

            if (!_values.ContainsKey(key))
            {
                String technicalError = String.Format("!_values.ContainsKey(key); key = {0}", key);
                throw new BaseException(this, technicalError);
            }


            Object blob = _values[key];


            if (null == blob)
            {
                return null;
            }


            if (!(blob is JsonArray))
            {
                String technicalError = String.Format("!(blob is JSONArray); key = {0}; blob.GetType().Name = {1}", key, blob.GetType().Name);

                throw new BaseException(this, technicalError);
            }

            return (JsonArray)blob;

        }

        public JsonArray getJSONArray(String key, JsonArray defaultValue)
        {
            if (!_values.ContainsKey(key))
            {
                return defaultValue;
            }


            Object blob = _values[key];


            if (null == blob)
            {
                return defaultValue;
            }


            if (!(blob is JsonArray))
            {
                return defaultValue;
            }

            return (JsonArray)blob;

        }


        public JsonObject getJsonObject(String key, JsonObject defaultValue)
        {
            if (!_values.ContainsKey(key))
            {
                return defaultValue;
            }


            Object blob = _values[key];


            if (null == blob)
            {
                return defaultValue;
            }


            if (!(blob is JsonObject))
            {
                return defaultValue;
            }

            return (JsonObject)blob;

        }



        public long getLong(String key)
        {
            if (!_values.ContainsKey(key))
            {
                String technicalError = String.Format("!_values.ContainsKey(key); key = {0}", key);
                throw new BaseException(this, technicalError);
            }

            Object blob = _values[key];

            if (null == blob)
            {
                String technicalError = String.Format("null == blob; key = {0}", key);
                throw new BaseException(this, technicalError);
            }

            if (blob is int)
            {
                return (int)blob;
            }

            if (blob is long)
            {
                return (long)blob;
            }

            {
                String technicalError = String.Format("!(blob is long); key = {0}; blob.GetType().Name = {1}", key, blob.GetType().Name);
                throw new BaseException(this, technicalError);
            }

        }


        public Object getObject(String key)
        {
            if (!_values.ContainsKey(key))
            {
                String technicalError = String.Format("!_values.ContainsKey(key); key = {0}", key);
                throw new BaseException(this, technicalError);
            }


            Object blob = _values[key];

            return blob;
        }

        public Object getObject(String key, Object fallback)
        {
            if (!_values.ContainsKey(key))
            {
                return fallback;
            }

            Object blob = _values[key];

            return blob;

        }

        public String getString(String key)
        {
            if (!_values.ContainsKey(key))
            {
                String technicalError = String.Format("!_values.ContainsKey(key); key = {0}", key);
                throw new BaseException(this, technicalError);
            }

            Object blob = _values[key];

            if (null == blob)
            {
                return null;
            }

            if (!(blob is String))
            {

                String technicalError = String.Format("!(blob is String); key = {0}; blob.GetType().Name = {1}", key, blob.GetType().Name);
                throw new BaseException(this, technicalError);
            }


            return (String)blob;


        }


        public String getString(String key, String defaultValue)
        {
            if (!_values.ContainsKey(key))
            {
                return defaultValue;
            }

            Object blob = _values[key];

            if (null == blob)
            {
                return defaultValue;
            }

            if (!(blob is String))
            {
                return defaultValue;
            }

            return (String)blob;

        }

        public void put(String key, bool value)
        {
            _values[key] = value;
        }

        public void put(String key, int value)
        {
            _values[key] = value;
        }

        public void put(String key, long value)
        {
            _values[key] = value;
        }

        public void put(String key, Object value)
        {
            _values[key] = value;
        }

        public void put(String key, String value)
        {
            _values[key] = value;
        }


        public Dictionary<String, Object>.Enumerator GetEnumerator()
        {
            return _values.GetEnumerator();
        }


        public int count()
        {
            return _values.Count;
        }



        public String ToString(JsonStringOutput jsonWriter)
        {
            JSON_OBJECT_HANDLER.WriteValue(this, jsonWriter);
            return jsonWriter.ToString();
        }

        public override String ToString()
        {
            JsonStringOutput writer = new JsonStringOutput();
            return this.ToString(writer);
        }

        public byte[] ToBytes()
        {
            String stringRepresentation = this.ToString();
            return StringHelper.ToUtfBytes(stringRepresentation);
        }


        public static JsonObject build(Stream inputStream, int length)
        {
            Data data = new Data(inputStream, length);
            JsonDataInput dataInput = new JsonDataInput(data);
            JsonObject answer = (JsonObject)JSON_OBJECT_HANDLER.readValue(dataInput);
            return answer;
        }

        public static JsonObject build(String jsonString)
        {

            Data data = new Data(StringHelper.ToUtfBytes(jsonString));
            JsonDataInput dataInput = new JsonDataInput(data);
            JsonObject answer = (JsonObject)JSON_OBJECT_HANDLER.readValue(dataInput);
            return answer;
        }
    }
}
