using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using System.Collections;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A map node.
    /// </summary>
    public class Node : INode, IItemReferences, IMapObject, IBinarySerializable
    {
        /// <summary>
        /// The UID of this node.
        /// </summary>
        public ulong Uid { get; set; }

        /// <summary>
        /// The sectors this node is in.
        /// </summary>
        // This is an array instead of a list to greatly reduce
        // memory overhead
        public Sector[] Sectors { get; set; }

        private Vector3 position = Vector3.Zero;
        /// <summary>
        /// Position of the node. Note that this will be converted to fixed length floats.
        /// </summary>
        public Vector3 Position
        {
            get => position;
            set
            {
                var recalc = position != value;
                position = value;
                if (recalc) RecalculateItems();
            }
        }

        private Quaternion rotation;
        /// <summary>
        /// Rotation of the node.
        /// </summary>
        public Quaternion Rotation
        {
            get => rotation;
            set
            {
                var recalc = rotation != value;
                rotation = value;
                if (recalc) RecalculateItems();
            }
        }

        private IMapObject backwardItem;

        /// <summary>
        /// The backward item belonging to this node. 
        /// </summary>
        public IMapObject BackwardItem
        {
            get => backwardItem;
            set
            {
                var recalc = backwardItem != value;
                backwardItem = value;
                if (recalc) RecalculateItems();
            }
        }

        private IMapObject forwardItem;
        /// <summary>
        /// The forward item belonging to this node. 
        /// </summary>
        public IMapObject ForwardItem
        {
            get => forwardItem;
            set
            {
                var recalc = forwardItem != value;
                forwardItem = value;
                if (recalc) RecalculateItems();
            }
        }

        protected FlagField Flags;

        /// <summary>
        /// Determines if this node is red or green.
        /// </summary>
        public bool IsRed
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// If true, the game will use whichever rotation is specified without
        /// reverting to its default rotation when the node is updated.
        /// </summary>
        public bool FreeRotation
        {
            get => Flags[2];
            set => Flags[2] = value;
        }

        /// <summary>
        /// Defines if this node is a country border.
        /// </summary>
        public bool IsCountryBorder
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// The country of the backward item if this node is a country border.
        /// </summary>
        public byte BackwardCountry
        {
            get => Flags.GetByte(2);
            set => Flags.SetByte(2, value);
        }

        /// <summary>
        /// The country of the forward item if this node is a country border.
        /// </summary>
        public byte ForwardCountry
        {
            get => Flags.GetByte(1);
            set => Flags.SetByte(1, value);
        }

        /// <summary>
        /// Creates a new node with a random UID.
        /// </summary>
        public Node()
        {
            Init();
        }

        internal Node(bool initFields)
        {
            if (initFields) Init();
        }

        protected virtual void Init()
        {
            Uid = Utils.GenerateUuid();
            rotation = new Quaternion(0f, 0f, 0f, 1f);
            Flags = new FlagField();
        }

        public bool IsOrphaned() => ForwardItem is null && BackwardItem is null;

        public void Move(Vector3 newPos)
        {
            // if the node isn't attached to a sector,
            // just move it
            if (Sectors is null || Sectors.Length == 0)
            {
                Position = newPos;
            }
            else
            {
                var map = Sectors[0].Map;
                // check if the new position is still inside 
                // one of the node's sectors.
                // if not, set Sectors to the new sector.
                var newSector = Map.GetSectorOfCoordinate(newPos);
                map.AddSector(newSector.X, newSector.Z); // just in case
                Sectors = new Sector[] { map.Sectors[newSector] };
                Position = newPos;
            }

            // if one or both of the items are polyline items,
            // their length has to be recalculated
            // and terrain has to be adjusted
            RecalculateItems();
        }

        protected virtual void RecalculateItems()
        {
            if (BackwardItem is IRecalculatable bw) bw.Recalculate();
            if (ForwardItem is IRecalculatable fw) fw.Recalculate();
        }

        private const float positionFactor = 256f;
        /// <summary>
        /// Reads the node from a BinaryReader.
        /// </summary>
        /// <param name="sector">The sector the node was in.</param>
        /// <param name="r"></param>
        public void Deserialize(BinaryReader r)
        {
            Uid = r.ReadUInt64();

            Position = new Vector3(
                r.ReadInt32() / positionFactor,
                r.ReadInt32() / positionFactor,
                r.ReadInt32() / positionFactor
            );

            Rotation = r.ReadQuaternion();

            // The item attached to the node in backward direction.
            // This reference will be resolved in Map.UpdateItemReferences()
            // once all sectors are loaded
            var bwItemUid = r.ReadUInt64();
            BackwardItem = bwItemUid == 0 ? null : (IMapObject)new UnresolvedItem(bwItemUid);

            // The item attached to the node in forward direction.
            // This reference will be resolved in Map.UpdateItemReferences()
            // once all sectors are loaded
            var fwItemUid = r.ReadUInt64();
            ForwardItem = fwItemUid == 0 ? null : (IMapObject)new UnresolvedItem(fwItemUid);

            Flags = new FlagField(r.ReadUInt32());
        }

        public void Serialize(BinaryWriter w)
        {
            w.Write(Uid);

            w.Write((int)(Position.X * positionFactor));
            w.Write((int)(Position.Y * positionFactor));
            w.Write((int)(Position.Z * positionFactor));

            w.Write(Rotation);

            w.Write(BackwardItem is null ? 0UL : BackwardItem.Uid);
            w.Write(ForwardItem is null ? 0UL : ForwardItem.Uid);

            w.Write(Flags.Bits);
        }

        public override string ToString()
        {
            return $"{Uid:X16} ({Position.X}|{Position.Y}|{Position.Z})";
        }

        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            // uses the field instead of the property to not trigger recalc,
            // which we don't want while loading an existing map
            if (forwardItem is UnresolvedItem
                && allItems.TryGetValue(forwardItem.Uid, out var resolvedFw))
            {
                forwardItem = resolvedFw;
            }
            if (backwardItem is UnresolvedItem
                && allItems.TryGetValue(backwardItem.Uid, out var resolvedBw))
            {
                backwardItem = resolvedBw;
            }
        }

    }
}
