using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using RBush;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// Represents a map node.
    /// </summary>
    public class Node : INode, IItemReferences, ISpatialData
    {
        /// <summary>
        /// The UID of this node.
        /// </summary>
        public ulong Uid { get; set; }

        private Vector3 position;
        /// <inheritdoc/>
        public Vector3 Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    position = value;
                    UpdateEnvelope();
                    RecalculateItems();
                }
            }
        }

        private Quaternion rotation;
        /// <inheritdoc/>
        public Quaternion Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
            }
        }

        /// <inheritdoc/>
        public IMapObject BackwardItem { get; set; }

        /// <inheritdoc/>
        public IMapObject ForwardItem { get; set; }

        private FlagField flags;

        /// <inheritdoc/>
        public bool IsRed
        {
            get => flags[0];
            set => flags[0] = value;
        }

        /// <inheritdoc/>
        public bool FreeRotation
        {
            get => flags[2];
            set => flags[2] = value;
        }

        /// <inheritdoc/>
        public bool IsCountryBorder
        {
            get => flags[1];
            set => flags[1] = value;
        }

        /// <inheritdoc/>
        public bool IsCurveLocator
        {
            get => flags[3];
            set => flags[3] = value;
        }

        /// <inheritdoc/>
        public bool Locked
        {
            get => flags[26];
            set => flags[26] = value;
        }

        /// <inheritdoc/>
        public byte BackwardCountry
        {
            get => flags.GetByte(2);
            set => flags.SetByte(2, value);
        }

        /// <inheritdoc/>
        public byte ForwardCountry
        {
            get => flags.GetByte(1);
            set => flags.SetByte(1, value);
        }

        /// <inheritdoc/>
        public bool PlayerVehicleTypeChange
        {
            get => flags[4];
            set => flags[4] = value;
        }

        /// <inheritdoc/>
        public bool FwdTruck
        {
            get => flags[5];
            set => flags[5] = value;
        }

        /// <inheritdoc/>
        public bool FwdBus
        {
            get => flags[6];
            set => flags[6] = value;
        }

        /// <inheritdoc/>
        public bool FwdCar
        {
            get => flags[7];
            set => flags[7] = value;
        }

        /// <inheritdoc/>
        public bool BwdTruck
        {
            get => flags[28];
            set => flags[28] = value;
        }

        /// <inheritdoc/>
        public bool BwdBus
        {
            get => flags[29];
            set => flags[29] = value;
        }

        /// <inheritdoc/>
        public bool BwdCar
        {
            get => flags[30];
            set => flags[30] = value;
        }

        /// <inheritdoc/>
        public IItemContainer Parent { get; set; }

        private Envelope envelope;

        ref readonly Envelope ISpatialData.Envelope => ref envelope;

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
            flags = new FlagField();
        }

        /// <inheritdoc/>
        public bool IsOrphaned() => ForwardItem is null && BackwardItem is null;

        /// <inheritdoc/>
        public void Move(Vector3 newPos)
        {
            Position = newPos;
        }

        /// <inheritdoc/>
        public void Translate(Vector3 translation)
        {
            Move(Position + translation);
        }

        /// <inheritdoc/>
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
            else if (ForwardItem is not null && ForwardItem is not Prefab && n2.ForwardItem is not null)
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
                if (ForwardItem is not Prefab)
                    item.Recalculate();
                else
                    item.RecalculateLength();
                n2.ForwardItem = null;
                n2.BackwardItem = null;
                n2.Parent.Delete(n2);
            }
            // attach backward node of polyline item to non-origin prefab node
            else if (ForwardItem is Prefab && n2.ForwardItem is PolylineItem)
            {
                var item = n2.ForwardItem as PolylineItem;
                var prefab = ForwardItem as Prefab;
                if (IsRed)
                {
                    throw new InvalidOperationException("Unable to merge: can't connect the backward node " +
                        "of a polyline item with the origin node of a prefab");
                }
                item.Node = this;
                Rotation = Quaternion.Inverse(Rotation);
                BackwardItem = prefab;
                ForwardItem = item;
                IsRed = true;
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

        /// <inheritdoc/>
        public INode Split()
        {
            if (ForwardItem is null || BackwardItem is null)
            {
                return null;
            }

            var newNode = Parent.AddNode(Position);
            if (ForwardItem is PolylineItem fwPoly && BackwardItem is PolylineItem bwPoly)
            {
                newNode.ForwardItem = fwPoly;
                newNode.IsRed = true;
                newNode.Rotation = Rotation;
                IsRed = false;
                ForwardItem = null;
                fwPoly.Node = newNode;
                fwPoly.Recalculate();
                bwPoly.Recalculate();
                return newNode;
            }
            else if (ForwardItem is PolylineItem && BackwardItem is Prefab)
            {
                newNode.ForwardItem = ForwardItem;
                newNode.IsRed = true;
                newNode.Rotation = Rotation;
                IsRed = false;
                (ForwardItem as PolylineItem).Node = newNode;
                (ForwardItem as PolylineItem).Recalculate();
                ForwardItem = BackwardItem;
                BackwardItem = null;
                Rotation = Quaternion.Inverse(Rotation);
                return newNode;
            }
            else if (ForwardItem is Prefab && BackwardItem is PolylineItem)
            {
                newNode.BackwardItem = BackwardItem;
                newNode.Rotation = Rotation;
                (BackwardItem as PolylineItem).ForwardNode = newNode;
                (BackwardItem as PolylineItem).Recalculate();
                BackwardItem = null;
                return newNode;
            }
            else if (ForwardItem is Prefab && BackwardItem is Prefab)
            {
                newNode.ForwardItem = BackwardItem;
                newNode.Rotation = Rotation * Quaternion.CreateFromYawPitchRoll(MathF.PI, 0, 0);
                var idx = (BackwardItem as Prefab).Nodes.IndexOf(this);
                (BackwardItem as Prefab).Nodes[idx] = newNode;
                BackwardItem = null;
                return newNode;
            }
            else
            {
                throw new InvalidOperationException("Unable to split");
            }
        }

        /// <summary>
        /// Recalculates the items attached to this node.
        /// </summary>
        protected virtual void RecalculateItems()
        {
            // TODO Whenever polyline recalculation changes, 
            // check if this needs to be updated.

            if (ForwardItem is PathItem path)
            {
                var nodeIdx = path.Nodes.IndexOf(this);
                path.RecalculateAdjacent(nodeIdx);
            }

            if (BackwardItem is IRecalculatable bw)
            {
                bw.Recalculate();
            }
        }

        private const float fixedPointFactor = 256f;

        /// <summary>
        /// Reads the node from a BinaryReader whose position is at the start of the object.
        /// </summary>
        /// <param name="r">A BinaryReader whose position is at the start of a Node.</param>
        public void Deserialize(BinaryReader r, uint? version = null)
        {
            Uid = r.ReadUInt64();

            position = new Vector3(
                r.ReadInt32() / fixedPointFactor,
                r.ReadInt32() / fixedPointFactor,
                r.ReadInt32() / fixedPointFactor
            );
            UpdateEnvelope();

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

            flags = new FlagField(r.ReadUInt32());
        }

        /// <summary>
        /// Writes the node to a BinaryWriter.
        /// </summary>
        /// <param name="w">A BinaryWriter.</param>
        public void Serialize(BinaryWriter w)
        {
            w.Write(Uid);

            w.Write((int)(Position.X * fixedPointFactor));
            w.Write((int)(Position.Y * fixedPointFactor));
            w.Write((int)(Position.Z * fixedPointFactor));

            w.Write(Rotation);

            w.Write(BackwardItem is null ? 0UL : BackwardItem.Uid);
            w.Write(ForwardItem is null ? 0UL : ForwardItem.Uid);

            w.Write(flags.Bits);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Uid:X16} ({Position.X}|{Position.Y}|{Position.Z})";
        }

        /// <inheritdoc/>
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

        private void UpdateEnvelope()
        {
            envelope = new(position.X, position.Z, position.X, position.Z);
            if (Parent is Map map)
            {
                map.Nodes.Tree.Delete(this);
                map.Nodes.Tree.Insert(this);
            }
        }
    }
}
