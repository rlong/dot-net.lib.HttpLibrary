// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace jsonbroker.library.common.log
{
    public class DoublingLogDelegate : LogDelegate
    {

        private bool _isDebugEnabled;

        LogDelegate _primaryDelegate;
        LogDelegate _secondaryDelegate;


        public DoublingLogDelegate(bool isDebugEnabled, LogDelegate primaryDelegate, LogDelegate secondaryDelegate)
        {
            _isDebugEnabled = isDebugEnabled;
            _primaryDelegate = primaryDelegate;
            _secondaryDelegate = secondaryDelegate;
        }

        public bool isDebugEnabled()
        {
            return _isDebugEnabled;
        }


        public void debug(Log origin, String message)
        {
            if (_isDebugEnabled)
            {
                _primaryDelegate.debug(origin, message);
                _secondaryDelegate.debug(origin, message);
            }
        }


        public void debug(Log origin, bool value, String name)
        {
            if (_isDebugEnabled)
            {
                _primaryDelegate.debug(origin, value, name);
                _secondaryDelegate.debug(origin, value, name);
            }
        }

        public void debug(Log origin, float value, String name)
        {
            if (_isDebugEnabled)
            {
                _primaryDelegate.debug(origin, value, name);
                _secondaryDelegate.debug(origin, value, name);
            }
        }


        public void debug(Log origin, int value, String name)
        {
            if (_isDebugEnabled)
            {
                _primaryDelegate.debug(origin, value, name);
                _secondaryDelegate.debug(origin, value, name);
            }
        }


        public void debug(Log origin, Loggable value, String name)
        {
            if (_isDebugEnabled)
            {
                _primaryDelegate.debug(origin, value, name);
                _secondaryDelegate.debug(origin, value, name);
            }
        }


        public void debug(Log origin, long value, String name)
        {
            if (_isDebugEnabled)
            {
                _primaryDelegate.debug(origin, value, name);
                _secondaryDelegate.debug(origin, value, name);
            }
        }


        public void debug(Log origin, Object value, String name)
        {
            if (_isDebugEnabled)
            {
                _primaryDelegate.debug(origin, value, name);
                _secondaryDelegate.debug(origin, value, name);
            }
        }


        public void debug(Log origin, String value, String name)
        {
            if (_isDebugEnabled)
            {
                _primaryDelegate.debug(origin, value, name);
                _secondaryDelegate.debug(origin, value, name);
            }
        }


        public void enteredMethod(Log origin)
        {
            if (_isDebugEnabled)
            {
                _primaryDelegate.enteredMethod(origin);
                _secondaryDelegate.enteredMethod(origin);
            }
        }



        public void info(Log origin, String message)
        {
            _primaryDelegate.info(origin, message);
            _secondaryDelegate.info(origin, message);
        }



        public void warn(Log origin, String message)
        {
            _primaryDelegate.warn(origin, message);
            _secondaryDelegate.warn(origin, message);

        }


        public void error(Log origin, String message)
        {
            _primaryDelegate.error(origin, message);
            _secondaryDelegate.error(origin, message);
        }

    }
}
