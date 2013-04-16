// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.json
{
    public class JsonWalker
    {


        private static void walkArray(JsonArray jsonArray, JsonDocumentHandler visitor)
        {
            for (int index = 0, count = jsonArray.Count(); index < count; index++)
            {
                Object value = jsonArray.GetObject(index, null);

                if (null == value)
                {
                    visitor.onNull(index);
                    continue;
                }

                if (value is bool)
                {
                    visitor.onBoolean(index, (bool)value);
                    continue;
                }

                // numbers ... 
                if (value is double)
                {
                    visitor.onNumber(index, (double)value);
                    continue;
                }

                if (value is float)
                {
                    visitor.onNumber(index, (float)value);
                    continue;
                }

                if (value is int)
                {
                    visitor.onNumber(index, (int)value);
                    continue;
                }

                if (value is long)
                {
                    visitor.onNumber(index, (long)value);
                    continue;
                }

                if (value is String)
                {
                    visitor.onString(index, (String)value);
                    continue;
                }

                if (value is JsonObject)
                {
                    visitor.onObjectStart(index);
                    walkObject((JsonObject)value, visitor);
                    visitor.onObjectEnd(index);
                    continue;
                }

                if (value is JsonArray)
                {
                    visitor.onArrayStart(index);
                    walkArray((JsonArray)value, visitor);
                    visitor.onArrayEnd(index);
                    continue;
                }
            }
        }

        private static void walkObject(JsonObject jsonObject, JsonDocumentHandler visitor)
        {

            for (Dictionary<String, Object>.Enumerator e = jsonObject.GetEnumerator(); e.MoveNext(); )
            {
                KeyValuePair<String, Object> entry = e.Current;
                String key = entry.Key;
                Object value = entry.Value;

                if (null == value)
                {
                    visitor.onNull(key);
                    continue;
                }

                if (value is bool)
                {
                    visitor.onBoolean(key, (bool)value);
                    continue;
                }

                // numbers ... 
                if (value is double)
                {
                    visitor.onNumber(key, (double)value);
                    continue;
                }

                if (value is float)
                {
                    visitor.onNumber(key, (float)value);
                    continue;
                }

                if (value is int)
                {
                    visitor.onNumber(key, (int)value);
                    continue;
                }

                if (value is long)
                {
                    visitor.onNumber(key, (long)value);
                    continue;
                }

                if (value is String)
                {
                    visitor.onString(key, (String)value);
                    continue;
                }

                if (value is JsonObject)
                {
                    visitor.onObjectStart(key);
                    walkObject((JsonObject)value, visitor);
                    visitor.onObjectEnd(key);
                    continue;
                }

                if (value is JsonArray)
                {
                    visitor.onArrayStart(key);
                    walkArray((JsonArray)value, visitor);
                    visitor.onArrayEnd(key);
                    continue;
                }

            }

        }

        public static void walk(JsonObject jsonObject, JsonDocumentHandler visitor)
        {
            visitor.onObjectDocumentStart();

            walkObject(jsonObject, visitor);

            visitor.onObjectDocumentEnd();

        }

        public static void walk(JsonArray jsonArray, JsonDocumentHandler visitor)
        {
            visitor.onArrayDocumentStart();

            walkArray(jsonArray, visitor);

            visitor.onArrayDocumentEnd();
        }

    }
}
