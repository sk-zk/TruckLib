using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    public static class IOExtensions
    {
        private static readonly Encoding StringEncoding = Encoding.UTF8;

        /// <summary>
        /// Reads a value of type T from the current stream and advances the current position
        /// of the stream.
        /// </summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="r">A BinaryReader.</param>
        /// <returns>A value of type T read from the current stream.</returns>
        /// <exception cref="NotImplementedException"></exception>
        internal static object Read<T>(this BinaryReader r)
        {
            // send help.
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean: { return r.ReadBoolean(); }
                case TypeCode.Byte: { return r.ReadByte(); }
                case TypeCode.SByte: { return r.ReadSByte(); }
                case TypeCode.Char: { return r.ReadChar(); }
                case TypeCode.Decimal: { return r.ReadDecimal(); }
                case TypeCode.Double: { return r.ReadDouble(); }
                case TypeCode.Single: { return r.ReadSingle(); }
                case TypeCode.Int16: { return r.ReadInt16(); }
                case TypeCode.Int32: { return r.ReadInt32(); }
                case TypeCode.Int64: { return r.ReadInt64(); }
                case TypeCode.UInt16: { return r.ReadUInt16(); }
                case TypeCode.UInt32: { return r.ReadUInt32(); }
                case TypeCode.UInt64: { return r.ReadUInt64(); }

                default: { throw new NotImplementedException(); }
            }
        }

        /// <summary>
        /// Reads a list of IBinarySerializable objects or various other types from the current stream
        /// and advances the current position of the stream.
        /// </summary>
        /// <param name="r">A BinaryReader.</param>
        /// <param name="count">The number of objects to read.</param>
        /// <typeparam name="T">The type of the list.</typeparam>
        /// <returns>A list of type T read from the current stream.</returns>
        public static List<T> ReadObjectList<T>(this BinaryReader r, uint count, uint? version = null) where T : new()
        {
            var list = new List<T>((int)count);
            if (count == 0)
                return list;

            if (typeof(IBinarySerializable).IsAssignableFrom(typeof(T))) // trucklib objects
            {
                ReadList(r =>
                {
                    var obj = new T() as IBinarySerializable;
                    obj.Deserialize(r, version);
                    return (T)obj;
                });
            }
            else if (typeof(IComparable).IsAssignableFrom(typeof(T))) // int, float etc.
            {
                ReadList(r =>
                {
                    var val = r.Read<T>();
                    return (T)Convert.ChangeType(val, typeof(T));
                });
            }
            else if (typeof(T) == typeof(Vector2))
            {
                ReadList(r =>
                {
                    var vector = r.ReadVector2();
                    return (T)Convert.ChangeType(vector, typeof(T));
                });
            }
            else if (typeof(T) == typeof(Vector3))
            {
                ReadList(r =>
                {
                    var vector = r.ReadVector3();
                    return (T)Convert.ChangeType(vector, typeof(T));
                });
            }
            else
            {
                throw new NotImplementedException($"Don't know what to do with {typeof(T).Name}");
            }

            return list;

            void ReadList(Func<BinaryReader, T> readOneItem)
            {
                for (int i = 0; i < count; i++)
                    list.Add(readOneItem.Invoke(r));
            }
        }

        /// <summary>
        /// Writes a list of IBinarySerializable objects or various other types to the current stream
        /// and advances the stream position.
        /// </summary>
        /// <param name="w">The BinaryWriter.</param>
        /// <param name="list">The list to write.</param>
        /// <typeparam name="T">The type of the list.</typeparam>
        /// <exception cref="NotImplementedException"></exception>
        public static void WriteObjectList<T>(this BinaryWriter w, List<T> list)
        {
            if (list is null || list.Count == 0)
                return;

            if (typeof(IBinarySerializable).IsAssignableFrom(typeof(T))) // trucklib objects
            {
                foreach (var obj in list)
                    (obj as IBinarySerializable).Serialize(w);
            }
            else if (typeof(IComparable).IsAssignableFrom(typeof(T))) // int, float etc.
            {
                foreach (var value in list)
                    WriteListValue(w, value);
            }
            else if (typeof(T) == typeof(Vector2) || typeof(T) == typeof(Vector3))
            {
                foreach (var value in list)
                    WriteListValue(w, value);
            }
            else
            {
                throw new NotImplementedException($"Don't know what to do with {typeof(T).Name}");
            }
        }

        private static void WriteListValue<T>(BinaryWriter w, T value)
        {
            // dont @ me.
            switch (value)
            {
                case bool _bool:
                    w.Write(_bool);
                    break;
                case byte _byte:
                    w.Write(_byte);
                    break;
                case sbyte _sbyte:
                    w.Write(_sbyte);
                    break;
                case char _char:
                    w.Write(_char);
                    break;
                case double _double:
                    w.Write(_double);
                    break;
                case float _float:
                    w.Write(_float);
                    break;
                case short _short:
                    w.Write(_short);
                    break;
                case int _int:
                    w.Write(_int);
                    break;
                case long _long:
                    w.Write(_long);
                    break;
                case ushort _ushort:
                    w.Write(_ushort);
                    break;
                case uint _uint:
                    w.Write(_uint);
                    break;
                case ulong _ulong:
                    w.Write(_ulong);
                    break;
                case Vector2 _vec2:
                    w.Write(_vec2);
                    break;
                case Vector3 _vec3:
                    w.Write(_vec3);
                    break;
                default:
                    throw new NotImplementedException($"Don't know what to do with {typeof(T).Name}");
            }
        }

        /// <summary>
        /// Reads a Token from the current stream and advances the current position of the stream
        /// by eight bytes.
        /// </summary>
        /// <param name="r">A BinaryReader.</param>
        /// <returns>A token read from the current stream.</returns>
        public static Token ReadToken(this BinaryReader r) =>
            new(r.ReadUInt64());

        /// <summary>
        /// Reads a Vector2 from the current stream and advances the current position of the stream
        /// by eight bytes.
        /// </summary>
        /// <param name="r">A BinaryReader.</param>
        /// <returns>A Vector2 read from the current stream.</returns>
        public static Vector2 ReadVector2(this BinaryReader r) =>
            new(r.ReadSingle(), r.ReadSingle());

        /// <summary>
        /// Reads a Vector3 from the current stream and advances the current position of the stream
        /// by 12 bytes.
        /// </summary>
        /// <param name="r">A BinaryReader.</param>
        /// <returns>A Vector3 read from the current stream.</returns>
        public static Vector3 ReadVector3(this BinaryReader r) =>
            new(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

        /// <summary>
        /// Reads a Vector4 from the current stream and advances the current position of the stream
        /// by 16 bytes.
        /// </summary>
        /// <param name="r">A BinaryReader.</param>
        /// <returns>A Vector4 read from the current stream.</returns>
        public static Vector4 ReadVector4(this BinaryReader r) =>
            new(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

        /// <summary>
        /// Reads a Matrix4x4 from the current stream and advances the current position of the stream
        /// by 64 bytes.
        /// </summary>
        /// <param name="r">A BinaryReader.</param>
        /// <returns>A Matrix4x4 read from the current stream.</returns>
        public static Matrix4x4 ReadMatrix4x4(this BinaryReader r) =>
            new(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());

        /// <summary>
        /// Reads a quaternion in WXYZ format from the current stream and advances
        /// the current position of the stream by 16 bytes.
        /// </summary>
        /// <param name="r">A BinaryReader.</param>
        /// <returns>A quaternion read from the current stream.</returns>
        public static Quaternion ReadQuaternion(this BinaryReader r)
        {
            var w = r.ReadSingle();
            var x = r.ReadSingle();
            var y = r.ReadSingle();
            var z = r.ReadSingle();
            return new Quaternion(x, y, z, w);
        }

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
        /// Reads a Color in RGBA format from the current stream and advances
        /// the current position of the stream by four bytes.
        /// </summary>
        /// <param name="r">A BinaryReader.</param>
        /// <returns>A Color read from the current stream.</returns>
        public static Color ReadColor(this BinaryReader r)
        {
            var red = r.ReadByte();
            var green = r.ReadByte();
            var blue = r.ReadByte();
            var alpha = r.ReadByte();
            return Color.FromArgb(alpha, red, green, blue);
        }

        /// <summary>
        /// Writes a token to the current stream and advances the
        /// stream position by eight bytes.
        /// </summary>
        /// <param name="w">The BinaryWriter.</param>
        /// <param name="token">The value to write.</param>
        public static void Write(this BinaryWriter w, Token token)
        {
            w.Write(token.Value);
        }

        /// <summary>
        /// Writes a Vector2 to the current stream and advances the stream position by eight bytes.
        /// </summary>
        /// <param name="w">The BinaryWriter.</param>
        /// <param name="vector">The value to write.</param>
        public static void Write(this BinaryWriter w, Vector2 vector)
        {
            w.Write(vector.X);
            w.Write(vector.Y);
        }

        /// <summary>
        /// Writes a Vector3 to the current stream and advances the stream position by 12 bytes.
        /// </summary>
        /// <param name="w">The BinaryWriter.</param>
        /// <param name="vector">The value to write.</param>
        public static void Write(this BinaryWriter w, Vector3 vector)
        {
            w.Write(vector.X);
            w.Write(vector.Y);
            w.Write(vector.Z);
        }

        /// <summary>
        /// Writes a Vector4 to the current stream and advances the stream position by 16 bytes.
        /// </summary>
        /// <param name="w">The BinaryWriter.</param>
        /// <param name="vector">The value to write.</param>
        public static void Write(this BinaryWriter w, Vector4 vector)
        {
            w.Write(vector.X);
            w.Write(vector.Y);
            w.Write(vector.Z);
            w.Write(vector.W);
        }

        /// <summary>
        /// Writes a Matrix4x4 to the current stream and advances the stream position by 64 bytes.
        /// </summary>
        /// <param name="w">The BinaryWriter.</param>
        /// <param name="n">The value to write.</param>
        public static void Write(this BinaryWriter w, Matrix4x4 m)
        {
            w.Write(m.M11); w.Write(m.M12); w.Write(m.M13); w.Write(m.M14);
            w.Write(m.M21); w.Write(m.M22); w.Write(m.M23); w.Write(m.M24);
            w.Write(m.M31); w.Write(m.M32); w.Write(m.M33); w.Write(m.M34);
            w.Write(m.M41); w.Write(m.M42); w.Write(m.M43); w.Write(m.M44);
        }

        /// <summary>
        /// Writes a quaternion in WXYZ format to the current stream and advances
        /// the stream position by 16 bytes.
        /// </summary>
        /// <param name="w">The BinaryWriter.</param>
        /// <param name="q">The value to write.</param>
        public static void Write(this BinaryWriter w, Quaternion q)
        {
            w.Write(q.W);
            w.Write(q.X);
            w.Write(q.Y);
            w.Write(q.Z);
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

        /// <summary>
        /// Writes a Color in RGBA format to the current stream and advances
        /// the stream position by four bytes.
        /// </summary>
        /// <param name="w">The BinaryWriter.</param>
        /// <param name="color">The value to write.</param>
        public static void Write(this BinaryWriter w, Color color)
        {
            w.Write(color.R);
            w.Write(color.G);
            w.Write(color.B);
            w.Write(color.A);
        }
    }
}
