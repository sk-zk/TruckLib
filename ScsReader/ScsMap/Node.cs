using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using System.Collections;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// A map node.
    /// </summary>
    public class Node : IItemReferences, IMapObject
    {
        /// <summary>
        /// The UID of this node.
        /// </summary>
        public ulong Uid { get; set; }

        /// <summary>
        /// The sectors this node is in.
        /// </summary>
        public List<Sector> Sectors = new List<Sector>();

        /// <summary>
        /// Position of the node. Note that this will be converted to fixed length floats.
        /// </summary>
        public Vector3 Position = new Vector3();

        /// <summary>
        /// Rotation of the node.
        /// </summary>
        public Quaternion Rotation = new Quaternion(0f, 0f, 0f, 1f);

        /// <summary>
        /// The backward item belonging to this node. 
        /// </summary>
        public IMapObject BackwardItem;

        /// <summary>
        /// The forward item belonging to this node. 
        /// </summary>
        public IMapObject ForwardItem;

        /// <summary>
        /// If true, the game will use whichever rotation is specified without
        /// reverting to its default rotation when the node is updated.
        /// </summary>
        public bool FreeRotation;

        /// <summary>
        /// Defines if this node is a country border.
        /// </summary>
        public bool IsCountryBorder;

        /// <summary>
        /// The country of the backward item if this node is a country border.
        /// </summary>
        public byte BackwardCountry;

        /// <summary>
        /// The country of the forward item if this node is a country border.
        /// </summary>
        public byte ForwardCountry;

        /// <summary>
        /// Determines if this node is red or green.
        /// </summary>
        public bool IsRed;

        public Node()
        {
            Uid = Utils.GenerateUuid();
        }

        private const float positionFactor = 256f;

        /// <summary>
        /// Reads the node from a BinaryReader.
        /// </summary>
        /// <param name="sector"></param>
        /// <param name="r"></param>
        public void ReadFromStream(Sector sector, BinaryReader r)
        {            
            // Uid
            Uid = r.ReadUInt64();

            // Position
            Position.X = r.ReadInt32() / positionFactor;
            Position.Y = r.ReadInt32() / positionFactor;
            Position.Z = r.ReadInt32() / positionFactor;

            /* TODO: Figure out why I needed this code to begin with,
             * if I needed it at all
            // Sector
            // not all nodes in a sector file are actually in that sector,
            // so we'll check for each of them
            var sectorCoords = Map.GetSectorOfCoordinate(Position);
            if(!sector.Map.Sectors.ContainsKey(sectorCoords)) {
                sector.Map.AddSector(sectorCoords);
            }
            Sector = sector.Map.Sectors[sectorCoords];
            */
            Sectors.Add(sector);

            // rotation as quaternion
            Rotation.W = r.ReadSingle();
            Rotation.X = r.ReadSingle();
            Rotation.Y = r.ReadSingle();
            Rotation.Z = r.ReadSingle();

            // The item attached to the node in backward direction.
            // This reference will be resolved in Map.UpdateItemReferences()
            // once all sectors are loaded
            BackwardItem = new UnresolvedItem(r.ReadUInt64());

            // The item attached to the node in forward direction.
            // This reference will be resolved in Map.UpdateItemReferences()
            // once all sectors are loaded
            ForwardItem = new UnresolvedItem(r.ReadUInt64());

            // 6: free rotation; 
            // 7: country border; 8: is green node
            // rest unknown
            var flags1 = r.ReadByte();
            var bitArr1 = new BitArray(new byte[] { flags1 });
            FreeRotation = bitArr1[8 - 6];
            IsCountryBorder = bitArr1[8 - 7];
            IsRed = bitArr1[8 - 8];

            // country id in forward direction
            ForwardCountry = r.ReadByte();

            // country id in the backward direction
            BackwardCountry = r.ReadByte();

            // unknown
            r.ReadByte();
        }

        /// <summary>
        /// Writes the node.
        /// </summary>
        /// <param name="w"></param>
        public void WriteToStream(BinaryWriter w)
        {
            // UID
            w.Write(Uid);

            // Position
            w.Write((int)(Position.X * positionFactor));
            w.Write((int)(Position.Y * positionFactor));
            w.Write((int)(Position.Z * positionFactor));

            // Rotation
            w.Write(Rotation.W);
            w.Write(Rotation.X);
            w.Write(Rotation.Y);
            w.Write(Rotation.Z);

            // Backward UID
            w.Write(BackwardItem is null ? 0UL : BackwardItem.Uid);

            // Forward UID
            w.Write(ForwardItem is null ? 0UL : ForwardItem.Uid);

            // 6: free rotation; 
            // 7: country border; 8: is red node
            // rest unknown
            byte flags1 = 0;
            flags1 |= (byte)(Convert.ToByte(FreeRotation) << 2);
            flags1 |= (byte)(Convert.ToByte(IsCountryBorder) << 1);
            flags1 |= IsRed.ToByte();
            w.Write(flags1);

            // country ids
            w.Write(ForwardCountry);
            w.Write(BackwardCountry);

            // unknown
            w.Write((byte)0);
        }

        public override string ToString()
        {
            //return $"{Uid:X16} (B: {BackwardUid:X16}; F: {ForwardUid:X16})";
            return $"{Uid:X16} ({Position.X}|{Position.Y}|{Position.Z})";
        }

        /// <summary>
        /// Searches a list of all items for the items referenced by uid in this node
        /// and adds references to them in the node's MapItem fields.
        /// </summary>
        /// <param name="allItems">A list containing all map items.</param>
        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            if (ForwardItem is UnresolvedItem && allItems.ContainsKey(ForwardItem.Uid))
            {
                ForwardItem = allItems[ForwardItem.Uid];
            }
            if (BackwardItem is UnresolvedItem && allItems.ContainsKey(BackwardItem.Uid))
            {
                BackwardItem = allItems[BackwardItem.Uid];
            }
        }

    }
}
