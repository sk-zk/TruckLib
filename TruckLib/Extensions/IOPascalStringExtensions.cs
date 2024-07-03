using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    internal static class IOPascalStringExtensions
    {
        private static readonly Encoding StringEncoding = Encoding.UTF8;

        /// <summary>
        /// Reads a string in the format used in SCS's binary formats from the current stream
        /// and advances the current position of the stream.
        /// </summary>
        /// <param name="r">A BinaryReader.</param>
        /// <returns>A string read from the current stream.</returns>
        public static string ReadPascalString(this BinaryReader r)
        {
            // uint64 for string length - someone's been planning ahead

            var byteLen = r.ReadUInt64();
            var bytes = r.ReadBytes((int)byteLen);
            return StringEncoding.GetString(bytes);
        }

        /// <summary>
        /// Writes a string in the format used in SCS's binary formats to the
        /// current stream and advances the stream position.
        /// </summary>
        /// <param name="w">The BinaryWriter.</param>
        /// <param name="str">The value to write.</param>
        public static void WritePascalString(this BinaryWriter w, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                w.Write(0UL);
            }
            else
            {
                var bytes = StringEncoding.GetBytes(str);
                w.Write((ulong)bytes.Length);
                w.Write(bytes);
            }
        }
    }
}
