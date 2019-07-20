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
    /// Represents a point where a sign or other static models will be added at runtime.
    /// </summary>
    public class Sign : IBinarySerializable
    {
        public Token Name;

        public Vector3 Position;

        public Quaternion Rotation;

        public Token Model;

        public Token Part;

        public void ReadFromStream(BinaryReader r)
        {
            Name = r.ReadToken();
            Position = r.ReadVector3();
            Rotation = r.ReadQuaternion();
            Model = r.ReadToken();
            Part = r.ReadToken();
        }

        public void WriteToStream(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
