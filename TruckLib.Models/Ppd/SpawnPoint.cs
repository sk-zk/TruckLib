using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.Models.Ppd
{
    /// <summary>
    /// Represents a locator for various activators and spawn points prefabs use.
    /// </summary>
    public class SpawnPoint : IBinarySerializable
    {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public SpawnPointType Type { get; set; }

        public uint Unknown { get; set; }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            switch (version)
            {
                case 0x15:
                case 0x16:
                case 0x17:
                    Deserialize15to17(r);
                    break;
                case 0x18:
                    Deserialize18(r);
                    break;
                default:
                    throw new NotSupportedException($"Version {version} is not supported.");
            }
        }

        private void Deserialize15to17(BinaryReader r)
        {
            Position = r.ReadVector3();
            Rotation = r.ReadQuaternion();
            Type = (SpawnPointType)r.ReadUInt32();
        }

        private void Deserialize18(BinaryReader r)
        {
            Position = r.ReadVector3();
            Rotation = r.ReadQuaternion();
            Type = (SpawnPointType)r.ReadUInt32();
            Unknown = r.ReadUInt32();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Position);
            w.Write(Rotation);
            w.Write((uint)Type);
            w.Write(Unknown);
        }
    }
}
