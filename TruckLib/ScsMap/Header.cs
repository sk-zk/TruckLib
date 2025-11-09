using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents the header of .mbd, .base, .aux, .snd, .data, .layer, and .sbd files.
    /// </summary>
    public class Header : IBinarySerializable
    {
        private const int supportedVersion = 905;
        /// <summary>
        /// Version number of the map format.
        /// </summary>
        public uint CoreMapVersion { get; internal set; } = supportedVersion;

        /// <summary>
        /// Game ID token.
        /// <para>The ID <c>euro2</c> is used for both Euro Truck Simulator 2
        /// and American Truck Simulator.</para>
        /// </summary>
        public Token GameId { get; set; } = "euro2";

        /// <summary>
        /// Game map version. Not sure what this value affects.
        /// </summary>
        public uint GameMapVersion { get; set; } = 3;

        /// <inheritdoc/>
        /// <exception cref="UnsupportedVersionException"></exception>
        public virtual void Deserialize(BinaryReader r, uint? version = null)
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
