using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    internal static class Utils
    {
        /// <summary>
        /// Generates an 8 byte UUID.
        /// </summary>
        /// <returns>An 8 byte UUID.</returns>
        public static ulong GenerateUuid() =>
            BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0);

        /// <summary>
        /// Checks if a number is within the specified range. 
        /// Returns the number it is; throws ArgumentException if it isn't.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <param name="min">The lower limit.</param>
        /// <param name="max">The upper limit.</param>
        /// <returns>The number if it is within range.</returns>
        public static ushort SetIfInRange(ushort value, ushort min, ushort max)
        {
            if (value < min || value > max)
                throw new ArgumentException($"Value must be between {min} and {max}.");
            return value;
        }

        /// <summary>
        /// Checks if a number is within the specified range. 
        /// Returns the number it is; throws ArgumentException if it isn't.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <param name="min">The lower limit.</param>
        /// <param name="max">The upper limit.</param>
        /// <returns>The number if it is within range.</returns>
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
        /// <returns>The number if it is within range.</returns>
        public static float SetIfInRange(float value, float min, float max)
        {
            if (value < min || value > max)
                throw new ArgumentException($"Value must be between {min} and {max}.");
            return value;
        }

        /// <summary>
        /// Sets array[x,y] to array[y,x] and vice versa.
        /// </summary>
        /// <param name="arr"></param>
        public static void SwitchXY(object[,] arr)
        {
            for (int y = 0; y <= arr.GetUpperBound(0); y++)
            {
                for (int x = 0; x < y; x++)
                {
                    var temp = arr.GetValue(x, y);
                    arr.SetValue(arr.GetValue(y, x), x, y);
                    arr.SetValue(temp, y, x);
                }
            }
        }

        public static Vector3[,] MirrorX(Vector3[,] arr)
        {
            Vector3[,] newArr = Copy2dArray(arr);

            var upperBound0 = newArr.GetUpperBound(1) + 1;
            var halfUpperBound0 = upperBound0 / 2;
            for (int y = 0; y <= newArr.GetUpperBound(0); y++)
            {
                for (int x = 0; x < halfUpperBound0; x++)
                {
                    (newArr[x, y], newArr[upperBound0 - 1 - x, y])
                        = (newArr[upperBound0 - 1 - x, y], newArr[x, y]);
                }
            }

            return newArr;
        }

        /// <summary>
        /// Creates a copy of a 2D array.
        /// </summary>
        /// <param name="arr">The array to copy.</param>
        /// <returns>A copy of the array.</returns>
        public static Vector3[,] Copy2dArray(Vector3[,] arr)
        {
            Vector3[,] newArr = new Vector3[arr.GetUpperBound(0) + 1, arr.GetUpperBound(1) + 1];
            Array.Copy(arr, newArr, (arr.GetUpperBound(0) + 1) * (arr.GetUpperBound(1) + 1));
            return newArr;
        }
    }
}
