using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("TruckLibTests")]
namespace TruckLib.Models
{
    internal static class StringUtils
    {
        /// <summary>
        /// Converts a byte array containing null-terminated strings
        /// to a List&lt;string&gt;.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="encoding">Encoding to use. Defaults to ASCII.</param>
        /// <returns></returns>
        public static List<string> CStringBytesToList(byte[] bytes, Encoding encoding = null)
        {
            encoding ??= Encoding.ASCII;

            var strings = new List<string>();
            int lastNull = -1;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0)
                {
                    strings.Add(encoding.GetString(bytes, lastNull + 1, i - lastNull - 1));
                    lastNull = i;
                }
            }
            return strings;
        }

        public static List<byte[]> ListToCStringByteList(List<string> strings, Encoding encoding = null)
        {
            encoding ??= Encoding.ASCII;

            var bytes = new List<byte[]>(strings.Count);
            foreach (var str in strings)
            {
                bytes.Add(encoding.GetBytes(str + '\0'));
            }
            return bytes;
        }
    }
}
