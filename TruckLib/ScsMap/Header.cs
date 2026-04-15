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
        private const int supportedVersion = 907;
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

        /// <summary>
        /// Whether <see cref="Deserialize"/> should throw an exception.
        /// </summary>
        /// <remarks>Allows for reading .snd or .mbd files that haven't been updated yet.</remarks>
        internal EnforceVersionBehavior EnforceVersion { get; set; } = EnforceVersionBehavior.Strict;

        internal enum EnforceVersionBehavior
        {
            /// <summary>
            /// An exception is thrown if the version number is not an exact match.
            /// </summary>
            Strict = 0,

            /// <summary>
            /// An exception is thrown if the version number is higher than the supported version.
            /// Lower versions are allowed.
            /// </summary>
            AllowLower = 1,

            /// <summary>
            /// All version numbers are allowed. No exception is thrown for any reason.
            /// </summary>
            None = 2,
        }

        /// <inheritdoc/>
        /// <exception cref="UnsupportedVersionException"></exception>
        public virtual void Deserialize(BinaryReader r, uint? version = null)
        {
            CoreMapVersion = r.ReadUInt32();
            if (EnforceVersion == EnforceVersionBehavior.Strict && CoreMapVersion != supportedVersion)
            {
                throw new UnsupportedVersionException($"Map version {CoreMapVersion} is not supported.");
            }
            else if (EnforceVersion == EnforceVersionBehavior.AllowLower && CoreMapVersion > supportedVersion)
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
