using System;

namespace TruckLib
{
    public static class MiscExtensions
    {
        /// <summary>
        /// Increases the size of the given array by one and adds the given item to it.
        /// If the array is null, a new array will be created.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="arr">The array to append to.</param>
        /// <param name="item">The object to append.</param>
        /// <returns>The modified array.</returns>
        public static T[] Push<T>(this T[] arr, T item)
        {
            if (arr is null)
            {
                return [item];
            }        
            Array.Resize(ref arr, arr.Length + 1);
            arr[^1] = item;
            return arr;
        }

        /// <summary>
        /// Converts bool to byte.
        /// </summary>
        /// <param name="b">The bool.</param>
        /// <returns>1 if true, 0 if false.</returns>
        public static byte ToByte(this bool b) =>
            b ? (byte)1 : (byte)0;
    }
}
