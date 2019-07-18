using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader
{
    /// <summary>
    /// Interface for classes that can de/serialize themselves to the binary map format.
    /// </summary>
    public interface IMapSerializable
    {
        /// <summary>
        /// Reads the object from a BinaryReader
        /// whose position is at the start of the object.
        /// </summary>
        void ReadFromStream(BinaryReader r);

        /// <summary>
        /// Writes the object to a BinaryWriter.
        /// </summary>
        void WriteToStream(BinaryWriter w);
    }
}
