using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader
{
    internal static class StringUtils
    {
        /// <summary>
        /// Removes the specified pattern and everything after it. 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string RemoveStartingAtPattern(string str, string pattern)
        {
            var patternIdx = str.IndexOf(pattern);
            if (patternIdx < 0) return str;
            str = str.Remove(patternIdx);
            return str;
        }

        /// <summary>
        /// Checks if a string contains a number in a format used by Sii files.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumerical(string str)
        {
            // float written as hex bytes, e.g. "&3f800000"
            const string hexPrefix = "&";
            if (str.StartsWith(hexPrefix))
            {
                return str.Length == (hexPrefix.Length + 8)
                    && IsHexadecimal(str.Substring(1));
            }

            const char positiveSign = '+';
            const char negativeSign = '-';
            const char decimalPoint = '.';
            bool decimalPointFound = false;
            bool containsDigits = false; // to return false on "+" etc.
            for (int i = 0; i < str.Length; i++)
            {
                // signs are only allowed as first character
                if (str[i] == negativeSign || str[i] == positiveSign)
                {
                    if (i > 0) return false;
                    continue;
                }

                // check decimal point - only one per number
                if (str[i] == decimalPoint)
                {
                    if (decimalPointFound) return false;
                    decimalPointFound = true;
                    continue;
                }

                // now check if it's anything other than a digit
                if ((str[i] >= '0' && str[i] <= '9'))
                {
                    containsDigits = true;
                }
                else
                {
                    return false;
                }

            }
            return containsDigits;
        }

        /// <summary>
        /// Checks if a string contains hexadecimal digits only.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsHexadecimal(string str)
        {
            foreach (var c in str)
            {
                var isHex = (c >= '0' && c <= '9') ||
                    (c >= 'a' && c <= 'f') ||
                    (c >= 'A' && c <= 'F');

                if (!isHex) return false;
            }
            return true;
        }

        /// <summary>
        /// Converts a byte array containing null-terminated strings
        /// to a List&lt;string&gt;.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="encoding">Encoding to use. Defaults to ASCII.</param>
        /// <returns></returns>
        public static List<string> CStringBytesToList(byte[] bytes, 
            Encoding encoding = null)
        {
            if (encoding is null) encoding = Encoding.ASCII;

            var strings = new List<string>();
            int lastNull = -1;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0)
                {
                    strings.Add(encoding.GetString(bytes,
                        lastNull + 1,
                        i - lastNull - 1));
                    lastNull = i;
                }
            }
            return strings;
        }
    }
}
