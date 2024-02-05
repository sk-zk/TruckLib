using TruckLib.ScsMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap.Serialization;

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
        internal static T[] Push<T>(this T[] arr, T item)
        {
            if (arr is null)
                return new[] { item };
            
            Array.Resize(ref arr, arr.Length + 1);
            arr[^1] = item;
            return arr;
        }

        /// <summary>
        /// Clones an IBinarySerializable object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to clone.</param>
        /// <returns>The clone.</returns>
        public static T Clone<T>(this T obj) where T : IBinarySerializable, new()
        {
            T cloned = new();
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            obj.Serialize(writer);
            stream.Position = 0;

            using var reader = new BinaryReader(stream);
            cloned.Deserialize(reader);

            return cloned;
        }

        /// <summary>
        /// Clones a MapItem.
        /// </summary>
        /// <typeparam name="T">The type of the MapItem.</typeparam>
        /// <param name="item">The MapItem to clone.</param>
        /// <returns>The clone.</returns>
        public static T CloneItem<T>(this T item) where T : MapItem
        {
            T cloned;

            var serializer = MapItemSerializerFactory.Get(item.ItemType);

            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            serializer.Serialize(writer, item);
            if (serializer is IDataPayload)
            {
                (serializer as IDataPayload).SerializeDataPayload(writer, item);
            }
            stream.Position = 0;

            using var reader = new BinaryReader(stream);
            cloned = (T)serializer.Deserialize(reader);
            if (serializer is IDataPayload)
            {
                (serializer as IDataPayload).DeserializeDataPayload(reader, cloned);
            }

            return cloned;
        }

        /// <summary>
        /// Converts a quaternion to Euler angles.
        /// </summary>
        /// <param name="q">The quaternion.</param>
        /// <returns>Euler angles in radians.</returns>
        public static Vector3 ToEuler(this Quaternion q)
        {
            // via https://stackoverflow.com/a/56055813

            double x, y, z;

            // if the input quaternion is normalized, this is exactly one. 
            // Otherwise, this acts as a correction factor for the quaternion's not-normalizedness
            float unit = (q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z) + (q.W * q.W);

            // this will have a magnitude of 0.5 or greater if and only if this is a singularity case
            float test = q.X * q.W - q.Y * q.Z;

            if (test > 0.4995f * unit) // singularity at north pole
            {
                x = Math.PI / 2;
                y = 2f * Math.Atan2(q.Y, q.X);
                z = 0;
            }
            else if (test < -0.4995f * unit) // singularity at south pole
            {
                x = -Math.PI / 2;
                y = -2f * Math.Atan2(q.Y, q.X);
                z = 0;
            }
            else // no singularity - this is the majority of cases
            {
                x = Math.Asin(2f * (q.W * q.X - q.Y * q.Z));
                y = Math.Atan2(2f * q.W * q.Y + 2f *  q.Z * q.X, 1 - 2f * (q.X * q.X + q.Y * q.Y));
                z = Math.Atan2(2f * q.W * q.Z + 2f *  q.X * q.Y, 1 - 2f * (q.Z * q.Z + q.X * q.X));
            }
   
            return new Vector3((float)x, (float)y, (float)z);
        }

        /// <summary>
        /// Converts a quaternion to Euler angles in degrees.
        /// </summary>
        /// <param name="q">The quaternion.</param>
        /// <returns>Euler angles in degrees.</returns>
        public static Vector3 ToEulerDeg(this Quaternion q)
        {
            var euler = q.ToEuler();
            euler.X = MathEx.Deg(euler.X);
            euler.Y = MathEx.Deg(euler.Y);
            euler.Z = MathEx.Deg(euler.Z);
            return euler;
        }

        /// <summary>
        /// Converts bool to byte.
        /// </summary>
        /// <param name="b">The bool.</param>
        /// <returns>1 if true, 0 if false.</returns>
        internal static byte ToByte(this bool b) =>
            b ? (byte)1 : (byte)0;
    }
}
