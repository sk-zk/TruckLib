using System.IO;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents a terrain quad material.
    /// </summary>
    public struct Material : IBinarySerializable
    {
        public Token Name { get; set; }

        /// <summary>
        /// UV rotation in degrees.
        /// </summary>
        public float Rotation { get; set; }

        public Material(Token name) : this(name, 0) { }

        public Material(Token name, float rotation)
        {
            Name = name;
            Rotation = rotation;
        }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Name = r.ReadToken();
            Rotation = r.ReadUInt16() / 10f;
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Name);
            w.Write((ushort)(Rotation * 10));
        }
    }

}
