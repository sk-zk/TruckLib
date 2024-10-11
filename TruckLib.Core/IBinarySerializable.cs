using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    /// <summary>
    /// Interface for classes which can de/serialize themselves to a binary format.
    /// </summary>
    public interface IBinarySerializable
    {
        /// <summary>
        /// Reads the object from a BinaryReader whose position is at the start of the object.
        /// </summary>
        /// <param name="r">A BinaryReader whose position is at the start of the object.</param>
        /// <param name="version">The version number of the file format.</param>
        void Deserialize(BinaryReader r, uint? version = null);

        /// <summary>
        /// Writes the object to a BinaryWriter.
        /// </summary>
        /// <param name="w">A BinaryWriter.</param>
        void Serialize(BinaryWriter w);
    }
}
