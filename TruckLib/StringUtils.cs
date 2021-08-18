using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    internal static class StringUtils
    {
        /// <summary>
        /// Checks if a string contains a number in a format used by Sii files.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumerical(string str)
        {
            if (IsHexNotationFloat(str))
                return true;

            return double.TryParse(str, NumberStyles.Float | NumberStyles.AllowExponent, 
                CultureInfo.InvariantCulture, out var _);
        }

        /// <summary>
        /// Checks if a string contains a float written as hex bytes as used in .sii files,
        /// e.g. "&3f800000".
        /// </summary>
        public static bool IsHexNotationFloat(string str)
        {
            const string hexPrefix = "&";
            return str.StartsWith(hexPrefix)
                && str.Length == (hexPrefix.Length + 8)
                && IsHexadecimal(str.Substring(1));
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

                if (!isHex)
                    return false;
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
        public static List<string> CStringBytesToList(byte[] bytes, Encoding encoding = null)
        {
            if (encoding is null)
                encoding = Encoding.ASCII;

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

        public static List<byte[]> ListToCStringByteList(List<string> strings, Encoding encoding = null)
        {
            if (encoding is null)
                encoding = Encoding.ASCII;

            var bytes = new List<byte[]>(strings.Count);
            foreach (var str in strings)
            {
                bytes.Add(encoding.GetBytes(str + '\0'));
            }
            return bytes;
        }
    }
}
