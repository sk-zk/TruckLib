using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.Model.Ppd
{
    /// <summary>
    /// Represents a locator for various activators and spawn points prefabs use.
    /// </summary>
    public class SpawnPoint : IBinarySerializable
    {
        public Vector3 Position;

        public Quaternion Rotation;

        public SpawnPointType Type;

        public void ReadFromStream(BinaryReader r)
        {
            Position = r.ReadVector3();
            Rotation = r.ReadQuaternion();
            Type = (SpawnPointType)r.ReadUInt32();
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Position);
            w.Write(Rotation);
            w.Write((uint)Type);
        }
    }
}
