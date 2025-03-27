using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents a prefab spawn point belonging to a <see cref="Company"/> item.
    /// </summary>
    public class CompanySpawnPoint
    {
        /// <summary>
        /// The node of the spawn point.
        /// </summary>
        public INode Node { get; set; }

        internal FlagField Flags;

        /// <summary>
        /// The spawn point type.
        /// </summary>
        public CompanySpawnPointType Type 
        {
            get => (CompanySpawnPointType)Flags.GetBitString(0, 4);
            set => Flags.SetBitString(0, 4, (byte)value);
        }

        private const byte UnlimitedLengthValue = 15;
        /// <summary>
        /// Length of the trailer, between 14 and 28 inclusively. 0 means unlimited.
        /// </summary>
        public byte TrailerLength
        {
            get
            {
                var value = Flags.GetBitString(4, 4);
                if (value == UnlimitedLengthValue) 
                    return 0;
                return (byte)(value + 14);
            }
            set
            {
                if (value != 0 && (value < 14 || value > 28))
                {
                    throw new ArgumentOutOfRangeException(nameof(TrailerLength),
                        "Value must be 0 or between 14 and 28 inclusively.");
                }
                value = (value == 0) 
                    ? UnlimitedLengthValue 
                    : (byte)(value - 14);
                Flags.SetBitString(4, 4, value);
            }
        }

        /// <summary>
        /// The trailer type.
        /// </summary>
        public CompanySpawnPointTrailerType TrailerType
        {
            get => (CompanySpawnPointTrailerType)Flags.GetBitString(16, 4);
            set => Flags.SetBitString(16, 4, (byte)value);
        }

        /// <summary>
        /// Instantiates a new CompanySpawnPoint.
        /// </summary>
        /// <param name="node">The node of the spawn point.</param>
        /// <param name="flags">The flag field of the spawn point.</param>
        public CompanySpawnPoint(INode node, uint flags)
        {
            Node = node;
            Flags = new FlagField(flags);
        }

        /// <summary>
        /// Instantiates a new CompanySpawnPoint.
        /// </summary>
        /// <param name="node">The node of the spawn point.</param>
        /// <param name="type">The type of the spawn point.</param>
        public CompanySpawnPoint(INode node, CompanySpawnPointType type)
        {
            Node = node;
            Flags = new FlagField(0);
            Type = type;
        }
    }
}
