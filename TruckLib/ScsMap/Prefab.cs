using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using TruckLib.Model.Ppd;

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
        /// The map nodes of this prefab.
        /// </summary>
        public List<INode> Nodes { get; set; }

        /// <summary>
        /// The index of the origin node.
        /// <para>This defines which of the ppd nodes is the origin node.
        /// It is not an index for the <see cref="Prefab.Nodes">Nodes</see> property,
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
        /// The <see cref="Ferry"/> item this prefab is connected to, if applicable.
        /// </summary>
        public IMapItem FerryLink { get; set; }

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
        /// Gets or sets if this prefab is the last prefab before a <see cref="Ferry"/>.
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
        /// <returns>The newly created prefab.</returns>
        public static Prefab Add(IItemContainer map, string unitName, PrefabDescriptor ppd,
            Vector3 position, Quaternion? rotation = null)
        {
            return new PrefabCreator().FromPpd(map, unitName, ppd, position,
                rotation ?? Quaternion.Identity);
        }

        /// <summary>
        /// Appends a new road segment to a prefab.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="node">The index of the prefab node to attach to.</param>
        /// <param name="forwardPos">The position of the road's forward node.</param>
        /// <param name="type">The road type.</param>
        /// <param name="leftTerrainSize">The left terrain size.</param>
        /// <param name="rightTerrainSize">The right terrain size.</param>
        /// <returns>The newly created road.</returns>
        public Road AppendRoad(IItemContainer map, ushort node, Vector3 forwardPos,
            Token type, float leftTerrainSize = 0f, float rightTerrainSize = 0f)
        {
            if (node > Nodes.Count)
                throw new ArgumentOutOfRangeException(nameof(node),
                    $"This prefab only has {Nodes.Count} nodes.");

            INode backwardNode;
            INode forwardNode;

            // if the node to attach to is the origin node,
            // the road has to be *prepended* instead
            var prepend = (node == 0);
            if (prepend)
            {
                backwardNode = map.AddNode(forwardPos);
                forwardNode = Nodes[node];
            }
            else
            {
                backwardNode = Nodes[node];
                forwardNode = map.AddNode(forwardPos);
            }
         
            var road = new Road
            {
                Node = backwardNode,
                ForwardNode = forwardNode
            };
            road.Node.ForwardItem = road;
            road.ForwardNode.BackwardItem = road;
            road.InitFromAddOrAppend(backwardNode.Position, forwardNode.Position, type,
                leftTerrainSize, rightTerrainSize);
            map.AddItem(road);
            
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
                backwardNode.Rotation = (backwardNode.Rotation.IsIdentity)
                    ? Quaternion.CreateFromAxisAngle(Vector3.UnitY, -3.14159265f)
                    : Quaternion.Inverse(backwardNode.Rotation);
                forwardNode.Rotation = MathEx.GetNodeRotation(backwardNode.Position, forwardNode.Position);
            }          

            road.Recalculate();
            
            backwardNode.IsRed = true;
            
            return road;
        }

        /// <summary>
        /// Attaches the ForwardNode of the given polyline item to the specified node of this prefab.
        /// The polyline's ForwardNode will be deleted.
        /// </summary>
        /// <param name="item">The polyline item to attach.</param>
        /// <param name="prefabNodeIdx">The index of the prefab node to attach to.</param>
        public void Attach(PolylineItem item, INode itemNode, ushort prefabNodeIdx)
        {
            if (prefabNodeIdx > Nodes.Count)
                throw new ArgumentOutOfRangeException(nameof(prefabNodeIdx), $"This prefab only has {Nodes.Count} nodes.");

            if (itemNode.BackwardItem is null)
            {
                // deal with prepend roads coming from the wrong direction
                var oldPfNode = Nodes[prefabNodeIdx];
                Nodes[prefabNodeIdx] = itemNode;
                itemNode.ForwardItem = item;
                itemNode.BackwardItem = this;
                itemNode.Position = oldPfNode.Position;
                // set rotation to the inverse angle
                // now that something is attached to it
                itemNode.Rotation = Quaternion.Inverse(oldPfNode.Rotation);
                itemNode.IsRed = true;
                oldPfNode.Sectors[0].Map.Nodes.Remove(oldPfNode.Uid);
            }
            else
            {
                item.ForwardNode = Nodes[prefabNodeIdx];
                item.ForwardNode.ForwardItem = this;
                item.ForwardNode.BackwardItem = item;
                item.ForwardNode.IsRed = true;
                itemNode.Sectors[0].Map.Nodes.Remove(itemNode.Uid);
            }

            item.RecalculateLength();
        }

        /// <summary>
        /// Attaches the ForwardNode of the given polyline item to the closest node of this prefab.
        /// The polyline's ForwardNode will be deleted.
        /// </summary>
        /// <param name="item">The polyline item to attach.</param>
        public void Attach(PolylineItem item)
        {
            // find closest node
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

            Attach(item, closestItemNode, (ushort)Nodes.IndexOf(closestPrefabNode));
        }

        /// <summary>
        /// Attaches the given prefab to a node of this prefab, assuming that
        /// at least one of the nodes has the same position as a node of this prefab.
        /// </summary>
        /// <param name="p2">The prefab to attach.</param>
        public void Attach(Prefab p2)
        {
            // find overlapping node(s). 
            // the Intersect call returns the
            // nodes of this prefab which will replace the corresponding newPf nodes.
            var overlappingNodes = Nodes.Intersect(p2.Nodes, new NodePositionComparer()).ToList();
            if (overlappingNodes.Count == 0)
                throw new NotImplementedException("No overlapping node found - can't attach prefab");

            for (var i = 0; i < overlappingNodes.Count; i++)
            {
                var p1Node = overlappingNodes[i];
                var p1NodeIdx = Nodes.IndexOf(p1Node);
                var p2NodeIdx = p2.Nodes.FindIndex(x => x.Position == p1Node.Position);
                var p2Node = p2.Nodes[p2NodeIdx];

                // if the p2 node is the origin, the nodes have to be merged to the p2 node.
                // otherwise, the p1 node survives
                if (p2NodeIdx == 0)
                {
                    p2Node.BackwardItem = this;
                    Nodes[p1NodeIdx] = p2Node;

                    p1Node.Sectors[0].Map.Nodes.Remove(p1Node.Uid);
                }
                else
                {
                    p1Node.BackwardItem = p2;
                    p1Node.IsRed = p1Node.IsRed || p2Node.IsRed;
                    p2.Nodes[p2NodeIdx] = p1Node;

                    p2Node.ForwardItem = null;
                    p2Node.Sectors[0].Map.Nodes.Remove(p2Node.Uid);
                }
            }
        }

        /// <summary>
        /// Checks if this and the given prefab are connected to each other at one or more nodes.
        /// </summary>
        /// <param name="p2">The other prefab.</param>
        /// <returns>Whether this and the given prefab are connected to each other at 
        /// one or more nodes.</returns>
        public bool IsAttachedTo(Prefab p2) =>
            Nodes.Intersect(p2.Nodes).Any();

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
        /// <param name="newOrigin">The index of the new origin.</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void ChangeOrigin(ushort newOrigin)
        {
            if (newOrigin > Nodes.Count)
                throw new IndexOutOfRangeException();

            Nodes[newOrigin].IsRed = 
                Nodes[Origin].IsRed && Nodes[Origin].BackwardItem == null;

            Nodes = Nodes.Skip(newOrigin)
                .Concat(Nodes.Take(newOrigin))
                .ToList();
            Origin = newOrigin;
        }

        /// <inheritdoc/>
        internal override IEnumerable<INode> GetItemNodes() =>
            new List<INode>(Nodes);

        /// <inheritdoc/>
        internal override INode GetMainNode() => Nodes[0];

        /// <inheritdoc/>
        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            ResolveNodeReferences(Nodes, allNodes);
        }

        /// <inheritdoc/>
        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            if (FerryLink is UnresolvedItem && 
                allItems.TryGetValue(FerryLink.Uid, out var resolvedFerry))
            {
                FerryLink = resolvedFerry;
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

        /// <summary>
        /// Used for Attach(Prefab). Compares the position of two nodes.
        /// </summary>
        internal class NodePositionComparer : IEqualityComparer<INode>
        {
            public bool Equals(INode x, INode y) => x.Position == y.Position;
            public int GetHashCode(INode obj) => 0; // apparently this has to happen for Equals to be called
        }
    }
}
