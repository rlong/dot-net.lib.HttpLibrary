// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.json
{
    public interface JsonDocumentHandler
    {

        ////////////////////////////////////////////////////////////////////////////
        // document
        void onObjectDocumentStart();
        void onObjectDocumentEnd();

        void onArrayDocumentStart();
        void onArrayDocumentEnd();

        ////////////////////////////////////////////////////////////////////////////
        // array

        void onArrayStart(int index);
        void onArrayEnd(int index);

        void onBoolean(int index, bool value);
        void onNull(int index);
        void onNumber(int index, double value);
        void onNumber(int index, int value);
        void onNumber(int index, float value);
        void onNumber(int index, long value);

        void onObjectStart(int index);
        void onObjectEnd(int index);

        void onString(int index, String value);

        ////////////////////////////////////////////////////////////////////////////
        // object
        void onArrayStart(String key);
        void onArrayEnd(String key);

        void onBoolean(String key, bool value);
        void onNull(String key);
        void onNumber(String key, double value);
        void onNumber(String key, int value);
        void onNumber(String key, float value);
        void onNumber(String key, long value);

        void onObjectStart(String key);
        void onObjectEnd(String key);

        void onString(String key, String value);




    }
}
