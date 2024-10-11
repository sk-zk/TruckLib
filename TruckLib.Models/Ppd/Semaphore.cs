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
    /// Represents a semaphore locator.
    /// </summary>
    public class Semaphore : IBinarySerializable
    {
        public Token Profile { get; set; }

        public SemaphoreType Type { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public uint SemaphoreId { get; set; }

        /// <summary>
        /// Intervals/distances for each state of a semaphore. 
        /// Enabled only if Type is not UseProfile.
        /// </summary>
        public Vector4 Intervals { get; set; }

        /// <summary>
        /// Delay for which each semaphore cycle will be delayed before starting again. 
        /// Enabled only if Type is not UseProfile.
        /// </summary>
        public float CycleDelay { get; set; }

        private uint Unknown1;

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Position = r.ReadVector3();
            Rotation = r.ReadQuaternion();
            Type = (SemaphoreType)r.ReadUInt32();
            SemaphoreId = r.ReadUInt32();
            Intervals = r.ReadVector4();
            CycleDelay = r.ReadSingle();
            Profile = r.ReadToken();
            Unknown1 = r.ReadUInt32();
        }

        public void Serialize(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
