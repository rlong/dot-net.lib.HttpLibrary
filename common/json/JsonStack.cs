// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.json
{
    public class JsonStack
    {
        Stack<Object> _stack;

        ////////////////////////////////////////////////////////////////////////////
        //
        JsonObject _currentObject;

        public JsonObject getCurrentObject()
        {
            return _currentObject;
        }

        ////////////////////////////////////////////////////////////////////////////
        //
        JsonArray _currentArray;

        public JsonArray getCurrentArray()
        {
            return _currentArray;
        }


        ////////////////////////////////////////////////////////////////////////////
        //
        public JsonStack()
        {
            _stack = new Stack<Object>();
            _currentObject = null;
            _currentArray = null;
        }


        public Object pop()
        {
            Object popped = _stack.Pop();

            _currentObject = null;
            _currentArray = null;

            if (0 != _stack.Count)
            {

                Object current = _stack.Peek();
                if (current is JsonObject)
                {
                    _currentObject = (JsonObject)current;
                }
                else
                {
                    _currentArray = (JsonArray)current;
                }
            }

            return popped;

        }

        public void push(JsonObject top)
        {

            _stack.Push(top);
            _currentObject = top;
            _currentArray = null;
        }


        public void push(JsonArray top)
        {

            _stack.Push(top);
            _currentObject = null;
            _currentArray = top;
        }

    }
}
