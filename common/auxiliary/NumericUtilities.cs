// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.exception;

namespace jsonbroker.library.common.auxiliary
{
    public class NumericUtilities
    {

        public static int parseInteger(String integerValue)
        {
            try
            {
                return Int32.Parse(integerValue);
            } catch( Exception e ) {

                String technicalError = String.Format( "failed to parse '{0}' as an integer", integerValue);

                throw new BaseException(e, typeof(NumericUtilities), technicalError);
            }

        }


        public static long parseLong(String longValue)
        {
            try
            {
                return Int64.Parse(longValue);
            }
            catch (Exception e)
            {
                String technicalError = String.Format("failed to parse '{0}' as an long", longValue);
                throw new BaseException(e, typeof(NumericUtilities), technicalError);
            }

        }


        public static float parseFloat(String floatValue)
        {
            try
            {
                return float.Parse(floatValue);
            }
            catch (Exception e)
            {
                String technicalError = String.Format("failed to parse '{0}' as a float", floatValue);

                throw new BaseException(e, typeof(NumericUtilities), technicalError);
            }

        }

    }
}
