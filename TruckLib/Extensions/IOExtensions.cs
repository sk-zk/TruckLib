using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TruckLib.ScsMap;

namespace TruckLib
{
    internal static class IOExtensions
    {
        private static readonly Encoding StringEncoding = Encoding.UTF8;

        /// <summary>
        /// Reads a Vector2.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Vector2 ReadVector2(this BinaryReader r)
        {
            return new Vector2(r.ReadSingle(), r.ReadSingle());
        }

        /// <summary>
        /// Reads a Vector3.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Vector3 ReadVector3(this BinaryReader r)
        {
            return new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
        }

        /// <summary>
        /// Reads a Vector4.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Vector4 ReadVector4(this BinaryReader r)
        {
            return new Vector4(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
        }

        public static Matrix4x4 ReadMatrix4x4(this BinaryReader r)
        {
            return new Matrix4x4(
                    r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                    r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                    r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle(),
                    r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle()
                );
        }

        /// <summary>
        /// Reads a token.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Token ReadToken(this BinaryReader r)
        {
            return new Token(r.ReadUInt64());
        }

        /// <summary>
        /// Reads a string in the format used by SCS.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static string ReadPascalString(this BinaryReader r)
        {
            // uint64 for string length - someone's been planning ahead

            var byteLen = r.ReadUInt64();
            var str = StringEncoding.GetString(
                r.ReadBytes((int)byteLen)
                );
            return str;
        }

        /// <summary>
        /// Reads a color in RGBA format.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Color ReadColor(this BinaryReader r)
        {
            var red = r.ReadByte();
            var green = r.ReadByte();
            var blue = r.ReadByte();
            var alpha = r.ReadByte();
            return Color.FromArgb(alpha, red, green, blue);
        }

        /// <summary>
        /// Reads a quaternion in WXYZ format.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Quaternion ReadQuaternion(this BinaryReader r)
        {
            var w = r.ReadSingle();
            var x = r.ReadSingle();
            var y = r.ReadSingle();
            var z = r.ReadSingle();
            return new Quaternion(x, y, z, w);
        }

        public static object Read<T>(this BinaryReader r)
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
        /// Reads a list of IBinarySerializable objects or various other types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<T> ReadObjectList<T>(this BinaryReader r, uint count) where T : new()
        {
            var list = new List<T>((int)count);
            if (count == 0) return list;

            if (typeof(IBinarySerializable).IsAssignableFrom(typeof(T))) // trucklib objects
            {
                for (int i = 0; i < count; i++)
                {
                    var obj = new T() as IBinarySerializable;
                    obj.Deserialize(r);
                    list.Add((T)obj);
                }
            }
            else if (typeof(IComparable).IsAssignableFrom(typeof(T))) // int, float etc.
            {
                for (int i = 0; i < count; i++)
                {
                    var val = r.Read<T>();
                    // copy-pasting methods suddenly doesn't seem so bad anymore
                    var Tval = (T)Convert.ChangeType(val, typeof(T));
                    list.Add(Tval);
                }
            }
            else if (typeof(T) == typeof(Vector2))
            {
                for (int i = 0; i < count; i++)
                {
                    var vector = r.ReadVector2();
                    var Tvector = (T)Convert.ChangeType(vector, typeof(T));
                    list.Add(Tvector);
                }
            }
            else if (typeof(T) == typeof(Vector3))
            {
                for (int i = 0; i < count; i++)
                {
                    var vector = r.ReadVector3();
                    var Tvector = (T)Convert.ChangeType(vector, typeof(T));
                    list.Add(Tvector);
                }
            }
            else if(typeof(T) == typeof(UnresolvedItem))
            {
                for (int i = 0; i < count; i++)
                {
                    var item = new UnresolvedItem(r.ReadUInt64());
                    var Titem = (T)Convert.ChangeType(item, typeof(T));
                    list.Add(Titem);
                }
            }
            else
            {
                throw new NotImplementedException($"Don't know what to do with {typeof(T).Name}");
            }

            return list;
        }

        /// <summary>
        /// Writes a Vector2.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="vector"></param>
        public static void Write(this BinaryWriter w, Vector2 vector)
        {
            w.Write(vector.X);
            w.Write(vector.Y);
        }

        /// <summary>
        /// Writes a Vector3.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="vector"></param>
        public static void Write(this BinaryWriter w, Vector3 vector)
        {
            w.Write(vector.X);
            w.Write(vector.Y);
            w.Write(vector.Z);
        }

        /// <summary>
        /// Writes a Vector4.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="vector"></param>
        public static void Write(this BinaryWriter w, Vector4 vector)
        {
            w.Write(vector.X);
            w.Write(vector.Y);
            w.Write(vector.Z);
            w.Write(vector.W);
        }

        public static void Write(this BinaryWriter w, Matrix4x4 m)
        {
            w.Write(m.M11); w.Write(m.M12); w.Write(m.M13); w.Write(m.M14);
            w.Write(m.M21); w.Write(m.M22); w.Write(m.M23); w.Write(m.M24);
            w.Write(m.M31); w.Write(m.M32); w.Write(m.M33); w.Write(m.M34);
            w.Write(m.M41); w.Write(m.M42); w.Write(m.M43); w.Write(m.M44);
        }

        /// <summary>
        /// Writes a token to the stream.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="token"></param>
        public static void Write(this BinaryWriter w, Token token)
        {
            w.Write(token.Value);
        }

        /// <summary>
        /// Writes a color in RGBA format.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="color"></param>
        public static void Write(this BinaryWriter w, Color color)
        {
            w.Write(color.R);
            w.Write(color.G);
            w.Write(color.B);
            w.Write(color.A);
        }

        /// <summary>
        /// Writes a string.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="str"></param>
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
        /// Writes a quaternion in WXYZ format.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="q"></param>
        public static void Write(this BinaryWriter w, Quaternion q)
        {
            w.Write(q.W);
            w.Write(q.X);
            w.Write(q.Y);
            w.Write(q.Z);

        }

        /// <summary>
        /// Writes a list of IBinarySerializable objects or various other types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="w"></param>
        /// <param name="list"></param>
        public static void WriteObjectList<T>(this BinaryWriter w, List<T> list)
        {
            if (list is null || list.Count == 0) return;

            if (typeof(IBinarySerializable).IsAssignableFrom(typeof(T))) // trucklib objects
            {
                foreach (var obj in list)
                {
                    (obj as IBinarySerializable).Serialize(w);
                }
            }
            else if (typeof(IComparable).IsAssignableFrom(typeof(T))) // int, float etc.
            {
                foreach (var value in list)
                {
                    WriteListValue(w, value);
                }
            }
            else if(typeof(T) == typeof(Vector2) || typeof(T) == typeof(Vector3))
            {
                foreach (var value in list)
                {
                    WriteListValue(w, value);
                }
            }
            else
            {
                throw new NotImplementedException($"Don't know what to do with {typeof(T).Name}");
            }
        }

        private static void WriteListValue<T>(BinaryWriter w, T value)
        {
            // dont @ me.
            if (value is bool _bool)
            {
                w.Write(_bool);
            }
            else if (value is byte _byte)
            {
                w.Write(_byte);
            }
            else if (value is sbyte _sbyte)
            {
                w.Write(_sbyte);
            }
            else if (value is char _char)
            {
                w.Write(_char);
            }
            else if (value is double _double)
            {
                w.Write(_double);
            }
            else if (value is float _float)
            {
                w.Write(_float);
            }
            else if (value is short _short)
            {
                w.Write(_short);
            }
            else if (value is int _int)
            {
                w.Write(_int);
            }
            else if (value is long _long)
            {
                w.Write(_long);
            }
            else if (value is ushort _ushort)
            {
                w.Write(_ushort);
            }
            else if (value is uint _uint)
            {
                w.Write(_uint);
            }
            else if (value is ulong _ulong)
            {
                w.Write(_ulong);
            }
            else if (value is Vector2 _vec2)
            {
                w.Write(_vec2);
            }
            else if(value is Vector3 _vec3)
            {
                w.Write(_vec3);
            }
            else
            {
                throw new NotImplementedException($"Don't know what to do with {typeof(T).Name}");
            }
        }

    }
}
