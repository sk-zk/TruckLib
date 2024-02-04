using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace TruckLib.Models
{
    public class Locator : IBinarySerializable
    {
        public Token Name { get; set; }

        public Vector3 Position { get; set; }

        public float Scale { get; set; }

        public Quaternion Rotation { get; set; }

        public int HookupOffset { get; set; }

        public override string ToString()
        {
            return Name.String;
        }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Name = r.ReadToken();
            Position = r.ReadVector3();
            Scale = r.ReadSingle();
            Rotation = r.ReadQuaternion();
            HookupOffset = r.ReadInt32();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Name);
            w.Write(Position);
            w.Write(Scale);
            w.Write(Rotation);
            w.Write(HookupOffset);
        }
    }
}
