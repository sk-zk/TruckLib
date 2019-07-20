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
    /// Represents a semaphore locator.
    /// </summary>
    public class Semaphore : IBinarySerializable
    {
        public Token Profile;

        public SemaphoreType Type;

        public Vector3 Position;

        public Quaternion Rotation;

        public uint SemaphoreId;

        /// <summary>
        /// Intervals/distances for each state of a semaphore. 
        /// Enabled only if Type is not UseProfile.
        /// </summary>
        public Vector4 Intervals;

        /// <summary>
        /// Delay for which each semaphore cycle will be delayed before starting again. 
        /// Enabled only if Type is not UseProfile.
        /// </summary>
        public float CycleDelay;

        private uint Unknown1;

        public void ReadFromStream(BinaryReader r)
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

        public void WriteToStream(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
