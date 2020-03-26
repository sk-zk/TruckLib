using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    internal static class BitArrayExtensions
    {
        /// <summary>
        /// Converts a BitArray to uint.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static uint ToUInt(this BitArray arr)
        {
            // via https://stackoverflow.com/a/51430897
            var len = Math.Min(64, arr.Count);
            ulong n = 0;
            for (int i = 0; i < len; i++)
            {
                if (arr.Get(i))
                    n |= 1UL << i;
            }
            return (uint)n;
        }

        /// <summary>
        /// Retrieves a byte from a BitArray.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="byteIdx"></param>
        /// <returns></returns>
        public static byte GetByte(this BitArray arr, int byteIdx)
        {
            return BitConverter.GetBytes(arr.ToUInt())[byteIdx];
        }

        /// <summary>
        /// Retrieves a byte as bool[] from a BitArray.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="byteIdx"></param>
        /// <returns></returns>
        public static bool[] GetByteAsBools(this BitArray arr, int byteIdx)
        {
            var offset = byteIdx * 8;
            var boolArr = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                boolArr[i] = arr[offset + i];
            }
            return boolArr;
        }

        public static bool[] ToBoolArray(this BitArray arr)
        {
            var boolArr = new bool[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                boolArr[i] = arr[i];
            }
            return boolArr;
        }

        public static void SetByteToBools(this BitArray arr, int byteIdx, bool[] values)
        {
            var offset = byteIdx * 8;
            for (int i = 0; i < 8; i++)
            {
                arr[offset + i] = values[i];
            }
        }

        /// <summary>
        /// Sets a byte of a BitArray.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="byteIdx"></param>
        /// <param name="value"></param>
        public static void SetByte(this BitArray arr, int byteIdx, byte value)
        {
            var offset = byteIdx * 8;
            for (int i = 7; i >= 0; i--)
            {
                // this is terrible code isn't it
                var bit = (value & (1 << i));
                arr[offset + i] = (bit != 0);
            }
        }

        /// <summary>
        /// Sets multiple consecutive bits.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="start">The start index.</param>
        /// <param name="length">The amount of bits to write.</param>
        /// <param name="bitString">The bit string.</param>
        public static void SetBitString(this BitArray arr, int start, int length, uint bitString)
        {
            for (int i = 0; i < length; i++)
            {
                arr[start + i] = (bitString & (uint)(Math.Pow(2, i))) > 0;
            }
        }

        /// <summary>
        /// Returns multiple consecutive bits as uint.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="start">The start index.</param>
        /// <param name="length">The amount of bits to read.</param>
        /// <returns></returns>
        public static uint GetBitString(this BitArray arr, int start, int length)
        {
            uint val = 0;
            for (int i = 0; i < length; i++)
            {
                uint bit = arr[start + i] ? 1U : 0U;
                val += (bit << i);
            }
            return val;
        }
    }
}
