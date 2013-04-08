// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.json
{
    public class DefaultJsonHandler : JsonDocumentHandler
    {

        
        ////////////////////////////////////////////////////////////////////////////        
        // document
        public void onObjectDocumentStart()
        {
        }


        public void onObjectDocumentEnd()
        {
        }



        public void onArrayDocumentStart()
        {
        }


        public void onArrayDocumentEnd()
        {
        }


        ////////////////////////////////////////////////////////////////////////////
        // array

        public void onArrayStart(int index)
        {
        }


        public void onArrayEnd(int index)
        {
        }



        public void onBoolean(int index, bool value)
        {
        }


        public void onNull(int index)
        {
        }

        public void onNumber(int index, double value)
        {
        }


        public void onNumber(int index, int value)
        {
        }


        public void onNumber(int index, float value)
        {
        }


        public void onNumber(int index, long value)
        {
        }



        public void onObjectStart(int index)
        {
        }


        public void onObjectEnd(int index)
        {
        }


        public void onString(int index, String value)
        {
        }


        ////////////////////////////////////////////////////////////////////////////
        // object
        public void onArrayStart(String key)
        {
        }


        public void onArrayEnd(String key)
        {
        }



        public void onBoolean(String key, bool value)
        {
        }


        public void onNull(String key)
        {
        }


        public void onNumber(String key, double value)
        {
        }

        public void onNumber(String key, int value)
        {
        }


        public void onNumber(String key, float value)
        {
        }


        public void onNumber(String key, long value)
        {
        }



        public void onObjectStart(String key)
        {
        }


        public void onObjectEnd(String key)
        {
        }



        public void onString(String key, String value)
        {
        }






    }
}
