using ScsReader.ScsMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader
{
    public static class MiscExtensions
    {
        /// <summary>
        /// Clones an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone<T>(this T obj) where T : IBinarySerializable, new()
        {
            T cloned = new T();
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                obj.WriteToStream(writer);
                if (obj is IDataPart) (obj as IDataPart).WriteDataPart(writer);
                stream.Position = 0;

                using (var reader = new BinaryReader(stream))
                {
                    cloned.ReadFromStream(reader);
                    if (obj is IDataPart) (obj as IDataPart).ReadDataPart(reader);
                }
            }

            // don't allow duplicate uids
            if(cloned is IMapObject mapObject)
            {
                mapObject.Uid = Utils.GenerateUuid();
            }

            // TODO: also deepclone certain referenced
            // items such as nodes
            // TODO: then add the cloned item to the map
          
            return cloned;
        }

        /// <summary>
        /// Converts a quaternion to Euler angles.
        /// </summary>
        /// <param name="q"></param>
        /// <returns>Euler angles in radians.</returns>
        public static Vector3 ToEuler(this Quaternion q)
        {
            double qw = q.W;
            double qx = q.X;
            double qy = q.Y;
            double qz = q.Z;

            var sqw = qw * qw;
            var sqx = qx * qx;
            var sqy = qy * qy;
            var sqz = qz * qz;

            var eX = Math.Atan2(-2.0 * (qy * qz - qw * qx), sqw - sqx - sqy + sqz);
            var eY = Math.Asin(2.0 * (qx * qz + qw * qy));
            var eZ = Math.Atan2(-2.0 * (qx * qy - qw * qz), sqw + sqx - sqy - sqz);

            return new Vector3((float)eX, (float)eY, (float)eZ);
        }

        /// <summary>
        /// Converts a quaternion to Euler angles in degrees.
        /// </summary>
        /// <param name="q"></param>
        /// <returns>Euler angles in degrees.</returns>
        public static Vector3 ToEulerDeg(this Quaternion q)
        {
            var rad = q.ToEuler();
            rad.X = (float)(rad.X * MathEx.RadToDeg);
            rad.Y = (float)(rad.Y * MathEx.RadToDeg);
            rad.Z = (float)(rad.Z * MathEx.RadToDeg);
            return rad;
        }

        /// <summary>
        /// Converts bool to byte.
        /// </summary>
        /// <param name="b">The bool.</param>
        /// <returns>1 if true, 0 if false.</returns>
        public static byte ToByte(this bool b)
        {
            return b ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// Converts a bool array with length 8 to byte.
        /// </summary>
        /// <param name="bools"></param>
        /// <returns></returns>
        public static byte ToByte(this bool[] bools)
        {
            if (bools.Length < 8)
            {
                throw new InvalidCastException("Can't convert a bool[] of this length to a single byte.");
            }
            var arr = new BitArray(bools);
            var bytes = new byte[1];
            arr.CopyTo(bytes, 0);
            return bytes[0];
        }

    }
}
