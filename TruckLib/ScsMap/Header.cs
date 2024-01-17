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
        private const int supportedVersion = 900;
        /// <summary>
        /// Version number of the map format.
        /// </summary>
        public uint CoreMapVersion { get; internal set; } = supportedVersion;

        /// <summary>
        /// Game ID token.
        /// <para>The ID "euro2" is used for both ETS2 and ATS.</para>
        /// </summary>
        public Token GameId { get; set; } = "euro2";

        /// <summary>
        /// Game map version. Not sure what this value affects.
        /// </summary>
        public uint GameMapVersion { get; set; } = 3;

        /// <inheritdoc/>
        /// <exception cref="UnsupportedVersionException"></exception>
        public virtual void Deserialize(BinaryReader r)
        {
            CoreMapVersion = r.ReadUInt32();
            if (CoreMapVersion != supportedVersion)
            {
                throw new UnsupportedVersionException($"Map version {CoreMapVersion} is not supported.");
            }
            GameId = r.ReadToken();
            GameMapVersion = r.ReadUInt32();
        }

        /// <inheritdoc/>
        public void Serialize(BinaryWriter w)
        {
            w.Write(CoreMapVersion);
            w.Write(GameId);
            w.Write(GameMapVersion);
        }
    }
}
