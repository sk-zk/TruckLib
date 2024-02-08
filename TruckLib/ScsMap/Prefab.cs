using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using TruckLib.Models.Ppd;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A model with associated game logic which attaches to the road network.
    /// </summary>
    public class Prefab : MapItem, IItemReferences
    {
        /// <inheritdoc/>
        public override ItemType ItemType => ItemType.Prefab;

        /// <inheritdoc/>
        public override ItemFile DefaultItemFile => ItemFile.Base;

        /// <inheritdoc/>
        internal override bool HasDataPayload => true;

        /// <inheritdoc/>
        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        /// <summary>
        /// Gets or sets the view distance of the item in meters.
        /// </summary>
        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The unit name of the prefab.
        /// </summary>
        public Token Model { get; set; }

        /// <summary>
        /// The model variant.
        /// </summary>
        public Token Variant { get; set; }

        /// <summary>
        /// The model look.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// The map nodes of this prefab. The origin node will always be the first entry in this list.
        /// </summary>
        public List<INode> Nodes { get; set; }

        /// <summary>
        /// The index of the origin node.
        /// <para>This defines which of the ppd nodes is the origin node.
        /// It is not an index for the <see cref="Nodes">Nodes</see> property,
        /// as the origin node is always the first node in that list.</para>
        /// </summary>
        public ushort Origin { get; internal set; }

        /// <summary>
        /// Unit names of enabled additional parts.
        /// </summary>
        public List<Token> AdditionalParts { get; set; }

        public byte DlcGuard
        {
            get => Kdop.Flags.GetByte(1);
            set => Kdop.Flags.SetByte(1, value);
        }

        /// <summary>
        /// Vegetation, terrain, and corner model properties for each prefab node.
        /// </summary>
        public PrefabNode[] PrefabNodes { get; set; }

        public Token SemaphoreProfile { get; set; }

        /// <summary>
        /// The <see cref="ScsMap.Ferry"/> item this prefab is connected to, if applicable.
        /// </summary>
        public IMapItem Ferry { get; set; }

        public uint RandomSeed { get; set; }

        /// <summary>
        /// A list of slave items owned by this prefab instance.
        /// </summary>
        public List<IMapItem> SlaveItems { get; set; }

        public List<VegetationPart> VegetationParts { get; set; } 

        /// <summary>
        /// Vegetation spheres of this prefab.
        /// </summary>
        public List<VegetationSphere> VegetationSpheres { get; set; }

        /// <summary>
        /// Gets or sets if the prefab is a tunnel. 
        /// <para>This will make AI vehicles turn on their headlights.</para>
        /// </summary>
        public bool IsTunnel
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        /// <summary>
        /// Gets or sets if detail vegetation (small clumps of grass etc.) is rendered
        /// if the selected terrain material supports it.
        /// </summary>
        public bool DetailVegetation
        {
            get => !Kdop.Flags[23];
            set => Kdop.Flags[23] = !value;
        }

        /// <summary>
        /// Gets or sets if the prefab uses left hand traffic.
        /// </summary>
        public bool LeftHandTraffic
        {
            get => Kdop.Flags[22];
            set => Kdop.Flags[22] = value;
        }

        /// <summary>
        /// Gets or sets if activation points are enabled.
        /// </summary>
        public bool Activation
        {
            get => !Kdop.Flags[21];
            set => Kdop.Flags[21] = !value;
        }

        /// <summary>
        /// Allows customization of selected semaphore profile
        /// by removing unwanted semaphore instances
        /// at complex crossroads consisting of multiple prefabs.
        /// </summary>
        public bool CustomSemaphores
        {
            get => Kdop.Flags[1];
            set => Kdop.Flags[1] = value;
        }

        /// <summary>
        /// Obsolete flag which was used to mark the prefab as a state border.
        /// </summary>
        [Obsolete]
        public bool IsStateBorder
        {
            get => Kdop.Flags[18];
            set => Kdop.Flags[18] = value;
        }

        /// <summary>
        /// Gets or sets if this prefab is displayed in the UI map.
        /// </summary>
        public bool ShowInUiMap
        {
            get => !Kdop.Flags[17];
            set => Kdop.Flags[17] = !value;
        }

        /// <summary>
        /// Gets or sets if this prefab has invisible walls around roads.
        /// </summary>
        public bool Boundary
        {
            get => !Kdop.Flags[16];
            set => Kdop.Flags[16] = !value;
        }

        public bool TerrainShadows
        {
            get => !Kdop.Flags[30];
            set => Kdop.Flags[30] = !value;
        }

        /// <summary>
        /// Gets or sets if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation
        {
            get => Kdop.Flags[29];
            set => Kdop.Flags[29] = value;
        }

        /// <summary>
        /// Gets or sets if AI traffic can spawn on this prefab.
        /// </summary>
        public bool AiVehicles
        {
            get => !Kdop.Flags[27];
            set => Kdop.Flags[27] = !value;
        }

        /// <summary>
        /// Gets or sets if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[26];
            set => Kdop.Flags[26] = value;
        }

        /// <summary>
        /// Gets or sets if this prefab is the last prefab before a <see cref="ScsMap.Ferry"/>.
        /// </summary>
        public bool IsFerryEntrance
        {
            get => Kdop.Flags[25];
            set => Kdop.Flags[25] = value;
        }

        /// <summary>
        /// Gets or sets if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[24];
            set => Kdop.Flags[24] = value;
        }

        /// <summary>
        /// Gets or sets if collision is enabled.
        /// </summary>
        public bool Collision
        {
            get => !Kdop.Flags[2];
            set => Kdop.Flags[2] = !value;
        }

        /// <summary>
        /// Gets or sets if this prefab is only visible in the UI map once discovered.
        /// </summary>
        public bool Secret
        {
            get => Kdop.Flags[5];
            set => Kdop.Flags[5] = value;
        }

        public Prefab() : base() 
        {
            Init();
        }

        internal Prefab(bool initFields) : base(initFields)
        {
            if (initFields)
            {
                Init();
            }
            else
            {
                PrefabNodes = new PrefabNode[6].Select(h => new PrefabNode(false)).ToArray();
            }
        }

        /// <inheritdoc/>
        protected override void Init()
        {
            base.Init();
            PrefabNodes = new PrefabNode[6].Select(h => new PrefabNode()).ToArray();
            Nodes = new List<INode>(2);
            AdditionalParts = new List<Token>();
            SlaveItems = new List<IMapItem>();
            VegetationParts = new List<VegetationPart>();
            VegetationSpheres = new List<VegetationSphere>();
        }

        /// <summary>
        /// Creates a prefab item from a prefab descriptor file and adds it to the map.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="unitName">The unit name of the prefab.</param>
        /// <param name="position">The position of node 0.</param>
        /// <param name="ppd">The prefab descriptor file defining the prefab.</param>
        /// <param name="rotation">The rotation of the prefab.</param>
        /// <returns>The newly created prefab.</returns>
        public static Prefab Add(IItemContainer map, Vector3 position, Token unitName,
            PrefabDescriptor ppd, Quaternion? rotation = null)
        {
            return new PrefabCreator().FromPpd(map, unitName, ppd, position,
                rotation ?? Quaternion.Identity);
        }

        /// <summary>
        /// Appends a new road segment to a node of this prefab.
        /// <remarks>If the node is the origin node, it will be prepended instead.</remarks>
        /// </summary>
        /// <param name="node">The index of the prefab node 
        /// (in <see cref="Nodes"/>, not the .ppd file) to attach to.</param>
        /// <param name="forwardPos">The position of the road's forward node.</param>
        /// <param name="type">The unit name of the road.</param>
        /// <param name="leftTerrainSize">The left terrain size.</param>
        /// <param name="rightTerrainSize">The right terrain size.</param>
        /// <returns>The newly created road.</returns>
        public Road AppendRoad(ushort node, Vector3 forwardPos, Token type,
            float leftTerrainSize = 0f, float rightTerrainSize = 0f)
        {
            if (node > Nodes.Count)
                throw new IndexOutOfRangeException($"This prefab only has {Nodes.Count} nodes.");

            // if the node to attach to is the origin node,
            // the road has to be *prepended* instead
            var prepend = node == 0;

            if (prepend && Nodes[node].BackwardItem is not null)
                throw new InvalidOperationException("Can't prepend to this node: there is already an item attached to it.");
            else if (!prepend && Nodes[node].BackwardItem is not null)
                throw new InvalidOperationException("Can't append to this node: there is already an item attached to it.");

            var theNewNode = Nodes[0].Parent.AddNode(forwardPos);
            var backwardNode = prepend ? theNewNode : Nodes[node];
            var forwardNode = prepend ? Nodes[node] : theNewNode;
         
            var road = new Road
            {
                Node = backwardNode,
                ForwardNode = forwardNode
            };
            backwardNode.ForwardItem = road;
            forwardNode.BackwardItem = road;
            road.InitFromAddOrAppend(backwardNode.Position, forwardNode.Position, type,
                leftTerrainSize, rightTerrainSize);
            backwardNode.IsRed = true;
            Nodes[0].Parent.AddItem(road);
            
            // nodes of a prefab that have nothing attached to it
            // always have the prefab as ForwardItem, but they will be
            // set to BackwardItem when you attach a road going outward
            if (!prepend)
                backwardNode.BackwardItem = this;
                
            if (prepend)
            {
                backwardNode.Rotation = MathEx.GetNodeRotation(backwardNode.Position, forwardNode.Position);
            }
            else
            {
                backwardNode.Rotation *= Quaternion.CreateFromYawPitchRoll((float)Math.PI, 0, 0);
                forwardNode.Rotation = MathEx.GetNodeRotation(backwardNode.Position, forwardNode.Position);
            }

            road.Recalculate();
            return road;
        }

        /// <summary>
        /// Finds the two closest nodes of this prefab and the given polyline item and attaches the
        /// polyline item to the prefab node.
        /// The leftover node of the polyline item will be deleted.
        /// </summary>
        /// <param name="item">The polyline item to attach.</param>
        public void Attach(PolylineItem item)
        {
            float shortestDist = float.MaxValue;
            INode closestPrefabNode = null;
            INode closestItemNode = null;
            foreach (var node in Nodes)
            {
                foreach (var itemNode in new[] { item.ForwardNode, item.Node })
                {
                    var dist = Vector3.DistanceSquared(node.Position, itemNode.Position);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        closestPrefabNode = node;
                        closestItemNode = itemNode;
                    }
                }
            }

            Attach((ushort)Nodes.IndexOf(closestPrefabNode), closestItemNode);
        }

        /// <summary>
        /// Attaches a node of a polyline item to the specified node of this prefab.
        /// </summary>
        /// <param name="prefabNode">The index of the prefab node (in <see cref="Nodes"/>, not the .ppd file)
        /// to attach to.</param>
        /// <param name="itemNode">The node of the polyline item to attach. This node will be deleted.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index exceeds the number of nodes.</exception>
        /// <exception cref="InvalidOperationException">Thrown when attempting to merge the backward node of a road
        /// into the origin node of the prefab, which is not allowed.</exception>
        public void Attach(ushort prefabNode, INode itemNode)
        {
            if (prefabNode > Nodes.Count)
                throw new IndexOutOfRangeException($"This prefab only has {Nodes.Count} nodes.");

            Nodes[prefabNode].Merge(itemNode);
        }

        /// <summary>
        /// Attaches the prefab <c>p2</c> to a node of this prefab. <c>p2</c> will be moved such that the nodes
        /// which will be merged have the same position.
        /// </summary>
        /// <param name="p1NodeIdx">The index of the node of this prefab to which <c>p2</c> will be attached.</param>
        /// <param name="p2">The prefab to attach to this one.</param>
        /// <param name="p2NodeIdx">The index of the node of <c>p2</c> which will be attached to this prefab.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if one of the indices exceeds the number of nodes.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the nodes can't be connected.</exception>
        public void Attach(ushort p1NodeIdx, Prefab p2, ushort p2NodeIdx)
        {
            if (p1NodeIdx > Nodes.Count)
                throw new IndexOutOfRangeException($"This prefab only has {Nodes.Count} nodes.");

            if (p2NodeIdx > p2.Nodes.Count)
                throw new IndexOutOfRangeException($"p2 only has {p2.Nodes.Count} nodes.");

            if (p1NodeIdx == 0 && p2NodeIdx == 0)
                throw new InvalidOperationException("Unable to attach: two origin nodes can't be connected.");

            var p1Node = Nodes[p1NodeIdx];
            var p2Node = p2.Nodes[p2NodeIdx];

            if (!(p1Node.ForwardItem == this && p1Node.BackwardItem is null))
                throw new InvalidOperationException("Unable to attach: p1Node is already occupied.");

            if (!(p2Node.ForwardItem == p2 && p2Node.BackwardItem is null))
                throw new InvalidOperationException("Unable to attach: p2Node is already occupied.");

            p2.Move(p1Node.Position, p2NodeIdx);

            if (p2NodeIdx == 0)
            {
                p2Node.BackwardItem = this;
                Nodes[p1NodeIdx] = p2Node;

                p1Node.ForwardItem = null;
                p1Node.Parent.Delete(p1Node);
            } 
            else
            {
                p1Node.BackwardItem = p2;
                p1Node.ForwardItem = this;
                p2.Nodes[p2NodeIdx] = p1Node;

                p2Node.ForwardItem = null;
                p2Node.Parent.Delete(p2Node);
            }
        }

        /// <summary>
        /// Finds the two closest nodes of this prefab and <c>p2</c>, moves <c>p2</c> such that
        /// the two nodes have the same position, and attaches it to this prefab.
        /// </summary>
        /// <param name="p2">The prefab to attach to this one.</param>
        /// <exception cref="InvalidOperationException">Thrown if the nodes can't be connected.</exception>
        public void Attach(Prefab p2)
        {
            float shortestDist = float.MaxValue;
            ushort closestP1NodeIdx = 999;
            ushort closestP2NodeIdx = 999;
            for (ushort p1NodeIdx = 0; p1NodeIdx < Nodes.Count; p1NodeIdx++)
            {
                for (ushort p2NodeIdx = 0; p2NodeIdx < p2.Nodes.Count; p2NodeIdx++)
                {
                    var dist = Vector3.DistanceSquared(
                        Nodes[p1NodeIdx].Position, p2.Nodes[p2NodeIdx].Position);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        closestP1NodeIdx = p1NodeIdx;
                        closestP2NodeIdx = p2NodeIdx;
                    }
                }
            }

            Attach(closestP1NodeIdx, p2, closestP2NodeIdx);
        }

        /// <summary>
        /// Moves the item to a different location.
        /// </summary>
        /// <param name="newPos">The new position of node 0.</param>
        public override void Move(Vector3 newPos)
        {
            Move(newPos, 0);
        }

        /// <summary>
        /// Moves the item to a different location.
        /// </summary>
        /// <param name="newPos">The new position of the specified node.</param>
        /// <param name="nodeIdx">The index of the node which will assume the given position.</param>
        public void Move(Vector3 newPos, ushort nodeIdx)
        {
            var translation = newPos - Nodes[nodeIdx].Position;
            Translate(translation);
        }

        /// <inheritdoc/>
        public override void Translate(Vector3 translation)
        {
            foreach (var node in Nodes)
                node.Move(node.Position + translation);

            foreach (var si in SlaveItems)
                (si as PrefabSlaveItem).Translate(translation);
        }

        /// <summary>
        /// Changes the origin of the prefab.
        /// </summary>
        /// <param name="newOrigin">The index (in the ppd file, not <see cref="Nodes"/>) of the new origin.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index exceeds the number of nodes.</exception>
        /// <exception cref="InvalidOperationException">Thrown if one or both of the nodes which would be
        /// affected by the opertation already have an item attached to them.</exception>
        public void ChangeOrigin(ushort newOrigin)
        {
            if (newOrigin > Nodes.Count)
                throw new IndexOutOfRangeException();

            var newOriginIdxInList = MathEx.Mod(newOrigin - Origin, Nodes.Count);

            if (Nodes[newOriginIdxInList].BackwardItem is not null || Nodes[0].BackwardItem is not null)
                throw new InvalidOperationException("Unable to change origin: one or both of the " +
                    "affected nodes have an item attached to them");

            Nodes[newOriginIdxInList].IsRed = 
                Nodes[0].IsRed && Nodes[0].BackwardItem == null;

            Nodes = Utils.Rotate(Nodes, newOrigin - Origin);
            Origin = newOrigin;
        }

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes() =>
            new List<INode>(Nodes);

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            ResolveNodeReferences(Nodes, allNodes);
        }

        /// <inheritdoc/>
        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            if (Ferry is UnresolvedItem && 
                allItems.TryGetValue(Ferry.Uid, out var resolvedFerry))
            {
                Ferry = resolvedFerry;
            }

            for (int i = 0; i < SlaveItems.Count; i++)
            {
                if (SlaveItems[i] is UnresolvedItem 
                    && allItems.TryGetValue(SlaveItems[i].Uid, out var resolvedSlaveItem))
                {
                    SlaveItems[i] = resolvedSlaveItem;
                }
            }
        }

        /// <inheritdoc/>
        internal override Vector3 GetCenter()
        {
            var acc = Vector3.Zero;
            foreach (var node in Nodes)
            {
                acc += node.Position;
            }
            return acc / Nodes.Count;
        }
    }
}
