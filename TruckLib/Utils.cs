using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("TruckLib.Tests")]
namespace TruckLib
{
    internal static class Utils
    {
        /// <summary>
        /// Generates an 8-byte UUID.
        /// </summary>
        /// <returns>An 8-byte UUID.</returns>
        public static ulong GenerateUuid() =>
            BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0);

        // TODO Change these to use generic math at some point

        /// <summary>
        /// Checks if a number is within the specified range. 
        /// Returns the number it is; throws ArgumentException if it isn't.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <param name="min">The lower limit.</param>
        /// <param name="max">The upper limit.</param>
        /// <returns>The number, if it is within range.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static ushort SetIfInRange(ushort value, ushort min, ushort max)
        {
            if (value < min || value > max)
                throw new ArgumentException($"Value must be between {min} and {max}.");
            return value;
        }

        /// <summary>
        /// Checks if a number is within the specified range. 
        /// Returns the number if it is; throws ArgumentException if it isn't.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <param name="min">The lower limit.</param>
        /// <param name="max">The upper limit.</param>
        /// <returns>The number, if it is within range.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static int SetIfInRange(int value, int min, int max)
        {
            if (value < min || value > max)
                throw new ArgumentException($"Value must be between {min} and {max}.");
            return value;
        }

        /// <summary>
        /// Checks if a number is within the specified range. 
        /// Returns the number if it is; throws ArgumentException if it isn't.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <param name="min">The lower limit.</param>
        /// <param name="max">The upper limit.</param>
        /// <returns>The number, if it is within range.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static float SetIfInRange(float value, float min, float max)
        {
            if (value < min || value > max)
                throw new ArgumentException($"Value must be between {min} and {max}.");
            return value;
        }

        /// <summary>
        /// Checks if a number is within the specified range. 
        /// Returns the number if it is; throws ArgumentException if it isn't.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <param name="min">The lower limit.</param>
        /// <param name="max">The upper limit.</param>
        /// <returns>The number, if it is within range.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte SetIfInRange(byte value, byte min, byte max)
        {
            if (value < min || value > max)
                throw new ArgumentException($"Value must be between {min} and {max}.");
            return value;
        }

        /// <summary>
        /// Sets array[x,y] to array[y,x] and vice versa.
        /// </summary>
        /// <param name="input">The array.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        public static T[,] SwitchXY<T>(T[,] input)
        {
            T[,] arr = Copy2dArray(input);

            for (int y = 0; y <= arr.GetLength(0) - 1; y++)
            {
                for (int x = 0; x < y; x++)
                {
                    var temp = arr.GetValue(x, y);
                    arr.SetValue(arr.GetValue(y, x), x, y);
                    arr.SetValue(temp, y, x);
                }
            }

            return arr;
        }

        public static T[,] MirrorX<T>(T[,] input)
        {
            T[,] arr = Copy2dArray(input);

            var length1 = arr.GetLength(1);
            var halfLength1 = length1 / 2;
            for (int y = 0; y <= arr.GetLength(0) - 1; y++)
            {
                for (int x = 0; x < halfLength1; x++)
                {
                    (arr[x, y], arr[length1 - 1 - x, y])
                        = (arr[length1 - 1 - x, y], arr[x, y]);
                }
            }

            return arr;
        }

        /// <summary>
        /// Creates a copy of a 2D array.
        /// </summary>
        /// <param name="src">The array to copy.</param>
        /// <returns>A copy of the array.</returns>
        /// <typeparam name="T">The type of the array.</typeparam>
        public static T[,] Copy2dArray<T>(T[,] src)
        {
            T[,] copy = new T[src.GetLength(0), src.GetLength(1)];
            Array.Copy(src, copy, (src.GetLength(0)) * (src.GetLength(1)));
            return copy;
        }

        internal static List<T> Rotate<T>(List<T> list, int shift)
        {
            shift = MathEx.Mod(shift, list.Count);

            var newList = new List<T>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                var idx = ((i + shift) % list.Count);
                newList.Add(list[idx]);
            }

            return newList;
        }
    }
}
