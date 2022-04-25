using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A map file header.
    /// </summary>
    public class Header : IBinarySerializable
    {
        private const int supportedVer = 893;
        /// <summary>
        /// Version number of the map format.
        /// </summary>
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

        public virtual void Deserialize(BinaryReader r)
        {
            CoreMapVersion = r.ReadUInt32();
            GameId = r.ReadToken();
            GameMapVersion = r.ReadUInt32();
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(CoreMapVersion);
            w.Write(GameId);
            w.Write(GameMapVersion);
        }

    }
}
