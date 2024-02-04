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
    /// Represents a point where a sign or other static models will be added at runtime.
    /// </summary>
    public class Sign : IBinarySerializable
    {
        public Token Name { get; set; }

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Token Model { get; set; }

        public Token Part { get; set; }

        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Name = r.ReadToken();
            Position = r.ReadVector3();
            Rotation = r.ReadQuaternion();
            Model = r.ReadToken();
            Part = r.ReadToken();
        }

        public void Serialize(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
