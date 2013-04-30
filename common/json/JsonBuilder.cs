// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;

namespace jsonbroker.library.common.json
{
    public class JsonBuilder : JsonDocumentHandler 
    {

        JsonStack _stack;

        ////////////////////////////////////////////////////////////////////////////
        //
        JsonObject _objectDocument;

        public JsonObject getObjectDocument()
        {
            return _objectDocument;
        }

        ////////////////////////////////////////////////////////////////////////////
        //
        JsonArray _arrayDocument;

        public JsonArray getArrayDocument()
        {
            return _arrayDocument;
        }

        ////////////////////////////////////////////////////////////////////////////
        //
        public JsonBuilder()
        {
            _stack = new JsonStack();
        }

        ////////////////////////////////////////////////////////////////////////////        
        // document
        public void onObjectDocumentStart()
        {
            _objectDocument = new JsonObject();
            _stack.push(_objectDocument);
        }


        public void onObjectDocumentEnd()
        {
            _stack.pop();
        }



        public void onArrayDocumentStart()
        {
            _arrayDocument = new JsonArray();
            _stack.push(_arrayDocument);
        }


        public void onArrayDocumentEnd()
        {
            _stack.pop();
        }



        ////////////////////////////////////////////////////////////////////////////
        // array

        public void onArrayStart(int index)
        {

            JsonArray jsonArray = new JsonArray();
            _stack.getCurrentArray().Add(jsonArray);
            _stack.push(jsonArray);

        }


        public void onArrayEnd(int index)
        {
            _stack.pop();

        }



        public void onBoolean(int index, bool value)
        {
            _stack.getCurrentArray().Add(value);
        }


        public void onNull(int index)
        {
            _stack.getCurrentArray().Add(null);
        }

        public void onNumber(int index, double value)
        {
            _stack.getCurrentArray().Add(value);
        }


        public void onNumber(int index, int value)
        {
            _stack.getCurrentArray().Add(value);
        }


        public void onNumber(int index, float value)
        {
            _stack.getCurrentArray().Add(value);
        }


        public void onNumber(int index, long value)
        {
            _stack.getCurrentArray().Add(value);
        }



        public void onObjectStart(int index)
        {
            JsonObject jsonObject = new JsonObject();
            _stack.getCurrentArray().Add(jsonObject);
            _stack.push(jsonObject);

        }


        public void onObjectEnd(int index)
        {
            _stack.pop();

        }


        public void onString(int index, String value)
        {
            _stack.getCurrentArray().Add(value);
        }

        ////////////////////////////////////////////////////////////////////////////
        // object

        public void onArrayStart(String key)
        {

            JsonArray jsonArray = new JsonArray();
            _stack.getCurrentObject().put(key, jsonArray);
            _stack.push(jsonArray);

        }


        public void onArrayEnd(String key)
        {
            _stack.pop();

        }



        public void onBoolean(String key, bool value)
        {
            _stack.getCurrentObject().put(key, value);
        }


        public void onNull(String key)
        {
            _stack.getCurrentObject().put(key, (Object)null);
        }


        public void onNumber(String key, double value)
        {
            _stack.getCurrentObject().put(key, value);
        }

        public void onNumber(String key, int value)
        {
            _stack.getCurrentObject().put(key, value);
        }


        public void onNumber(String key, float value)
        {
            _stack.getCurrentObject().put(key, value);
        }


        public void onNumber(String key, long value)
        {
            _stack.getCurrentObject().put(key, value);
        }



        public void onObjectStart(String key)
        {

            JsonObject jsonObject = new JsonObject();
            _stack.getCurrentObject().put(key, jsonObject);
            _stack.push(jsonObject);

        }


        public void onObjectEnd(String key)
        {
            _stack.pop();

        }


        public void onString(String key, String value)
        {
            _stack.getCurrentObject().put(key, value);
        }


    }
}
