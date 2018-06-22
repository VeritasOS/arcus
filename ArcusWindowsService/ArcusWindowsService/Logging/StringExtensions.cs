/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Logging
{
    public static class StringExtensions
    {
        ///<summary>
        ///Returns a bool indicating whether the supplied string represents a numeric value
        ///</summary>
        public static bool IsNumeric(this string str)
        {
            double db = 0;
            return (Double.TryParse(Convert.ToString(str), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out db));
        }
        ///<summary>
        ///Returns a bool indicating whether the string is null, empty or only whitespace
        ///</summary>
        public static bool IsNullEmptyOrBlank(this string str)
        {
            return ((string.IsNullOrEmpty(str)) || (str.TrimEnd().Length == 0));
        }
    }
}
