using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// A map file header.
    /// </summary>
    public class Header : IBinarySerializable
    {
        private const int supportedVer = 869;
        /// <summary>
        /// Version number of the map format.
        /// </summary>
        // 1.33 & 1.34 := 0x35A (858)
        // 1.35 := 0x365 (869)
        public uint CoreMapVersion { get; set; } = supportedVer;

        /// <summary>
        /// Game ID token.
        /// <para>The ID "euro2" is used for both ETS2 and ATS.</para>
        /// </summary>
        public Token GameId { get; set; } = "euro2";

        /// <summary>
        /// Game map version. Not sure what this value affects.
        /// </summary>
        public uint GameMapVersion { get; set; } = 3;

        /// <summary>
        /// Reads the header.
        /// </summary>
        /// <param name="r">The reader.</param>
        public virtual void ReadFromStream(BinaryReader r)
        {
            // Map format version
            CoreMapVersion = r.ReadUInt32();

            // Game ID token. "euro2" for ETS2/ATS
            GameId = r.ReadToken();

            // Game map version
            GameMapVersion = r.ReadUInt32();
        }

        /// <summary>
        /// Writes the header to a map file in binary format.
        /// </summary>
        /// <param name="w">The writer.</param>
        public void WriteToStream(BinaryWriter w)
        {
            w.Write(CoreMapVersion);
            w.Write(GameId);
            w.Write(GameMapVersion);
        }

    }
}
