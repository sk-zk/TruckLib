using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader
{
    public static class IOExtensions
    {
        private static readonly Encoding StringEncoding = Encoding.UTF8;

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
        /// Writes Vector3.X; Vector3.Y; Vector3.Z to the stream.
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
        /// Writes a string in the format used by SCS.
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

    }
}
