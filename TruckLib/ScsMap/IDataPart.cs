using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Used for items that store additional info in .data.
    /// </summary>
    public interface IDataPart
    {
        /// <summary>
        /// Reads the .data part of an item.
        /// </summary>
        /// <param name="r">The reader.</param>
        void ReadDataPart(BinaryReader r);

        /// <summary>
        /// Writes the .data part of an item.
        /// </summary>
        /// <param name="w">The writer.</param>
        void WriteDataPart(BinaryWriter w);
    }
}
