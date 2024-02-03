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
    /// Represents a map node.
    /// </summary>
    public class Node : INode, IItemReferences, IMapObject, IBinarySerializable
    {
        /// <summary>
        /// The UID of this node.
        /// </summary>
        public ulong Uid { get; set; }

        private Vector3 position;
        /// <summary>
        /// Position of the node. Note that this will be serialized as fixed point values.
        /// </summary>
        public Vector3 Position
        {
            get => position;
            set
            {
                var recalc = position != value;
                position = value;
                if (recalc)
                    RecalculateItems();
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
                rotation = value;
            }
        }

        /// <summary>
        /// The backward item belonging to this node. 
        /// </summary>
        public IMapObject BackwardItem { get; set; }

        /// <summary>
        /// The forward item belonging to this node. 
        /// </summary>
        public IMapObject ForwardItem { get; set; }

        protected FlagField Flags;

        /// <summary>
        /// Gets or sets if this node is red or green.
        /// </summary>
        public bool IsRed
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Gets or sets if the game will use whichever rotation is specified without
        /// reverting to its default rotation when the node is updated.
        /// </summary>
        public bool FreeRotation
        {
            get => Flags[2];
            set => Flags[2] = value;
        }

        /// <summary>
        /// Gets or sets if this node is a country border.
        /// </summary>
        public bool IsCountryBorder
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// Gets or sets if this node represents a curve locator.
        /// </summary>
        public bool IsCurveLocator
        {
            get => Flags[3];
            set => Flags[3] = value;
        }

        /// <summary>
        /// Gets or sets if this node can be moved or deleted in the editor.
        /// </summary>
        public bool Locked
        {
            get => Flags[26];
            set => Flags[26] = value;
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
        /// The map, selection or compound which contains this node.
        /// </summary>
        public IItemContainer Parent { get; set; }

        /// <summary>
        /// Instantiates a new node with a random UID.
        /// </summary>
        public Node()
        {
            Init();
        }

        internal Node(bool initFields)
        {
            if (initFields) Init();
        }

        /// <summary>
        /// Sets the node's properties to its default values.
        /// </summary>
        protected virtual void Init()
        {
            Uid = Utils.GenerateUuid();
            rotation = Quaternion.Identity;
            Flags = new FlagField();
        }

        /// <summary>
        /// Returns whether the node has no ForwardItem and no BackwardItem.
        /// </summary>
        /// <returns>Whether the node has no ForwardItem and no BackwardItem.</returns>
        public bool IsOrphaned() => ForwardItem is null && BackwardItem is null;

        /// <summary>
        /// Moves the node to another position.
        /// </summary>
        /// <param name="newPos">The new position of the node.</param>
        public void Move(Vector3 newPos)
        {
            Position = newPos;
        }

        /// <summary>
        /// Translates the node by the given vector.
        /// </summary>
        /// <param name="translation">The translation vector.</param>
        public void Translate(Vector3 translation)
        {
            Move(Position + translation);
        }

        /// <summary>
        /// Merges the node <i>n2</i> into this one, if possible, and then deletes <i>n2</i>.
        /// Note that the flags and rotation of <i>n2</i> will not be preserved.
        /// </summary>
        /// <param name="n2">The node to merge into this one.</param>
        /// <exception cref="InvalidOperationException">Thrown if the nodes can't be merged.</exception>
        public void Merge(INode n2)
        {
            // TODO put some of the prefab stuff in here

            if (BackwardItem is not null && ForwardItem is not null)
            {
                throw new InvalidOperationException("Unable to merge: this node is occupied in both directions.");
            }
            else if (BackwardItem is not null && n2.BackwardItem is not null)
            {
                throw new InvalidOperationException("Unable to merge: both nodes have a backward item");
            }
            else if (ForwardItem is not null && n2.ForwardItem is not null)
            {
                throw new InvalidOperationException("Unable to merge: both nodes have a forward item");
            }
            else if (ForwardItem is null && n2.ForwardItem is PolylineItem)
            {
                var item = n2.ForwardItem as PolylineItem;
                ForwardItem = item;
                item.Node = this;
                item.Recalculate();
                IsRed = !IsCurveLocator;
                n2.ForwardItem = null;
                n2.BackwardItem = null;
                n2.Parent.Delete(n2);
            }
            else if (BackwardItem is null && n2.BackwardItem is PolylineItem)
            {
                var item = n2.BackwardItem as PolylineItem;
                BackwardItem = item;
                item.ForwardNode = this;
                item.Recalculate();
                n2.ForwardItem = null;
                n2.BackwardItem = null;
                n2.Parent.Delete(n2);
            }
            else
            {
                throw new InvalidOperationException("Unable to merge");
            }
        }

        /// <summary>
        /// Recalculates the items attached to this node.
        /// </summary>
        protected virtual void RecalculateItems()
        {
            // TODO Whenever polyline recalculation changes, 
            // check if this needs to be updated.

            if (BackwardItem is IRecalculatable bw)
                bw.Recalculate();
        }

        private const float positionFactor = 256f;
        /// <summary>
        /// Reads the node from a BinaryReader whose position is at the start of the object.
        /// </summary>
        /// <param name="r">A BinaryReader whose position is at the start of a Node.</param>
        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Uid = r.ReadUInt64();

            position = new Vector3(
                r.ReadInt32() / positionFactor,
                r.ReadInt32() / positionFactor,
                r.ReadInt32() / positionFactor
            );

            rotation = r.ReadQuaternion();

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

        /// <summary>
        /// Writes the node to a BinaryWriter.
        /// </summary>
        /// <param name="w">A BinaryWriter.</param>
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

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Uid:X16} ({Position.X}|{Position.Y}|{Position.Z})";
        }

        /// <summary>
        /// Searches a list of all map items for the map items referenced by UID by this node
        /// and updates the respective references.
        /// </summary>
        /// <param name="allItems">A dictionary of all items in the entire map.</param>
        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            if (ForwardItem is UnresolvedItem
                && allItems.TryGetValue(ForwardItem.Uid, out var resolvedFw))
            {
                ForwardItem = resolvedFw;
            }
            if (BackwardItem is UnresolvedItem
                && allItems.TryGetValue(BackwardItem.Uid, out var resolvedBw))
            {
                BackwardItem = resolvedBw;
            }
        }
    }
}
