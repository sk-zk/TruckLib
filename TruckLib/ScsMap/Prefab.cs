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
    /// Represents a prefab.
    /// </summary>
    public class Prefab : MapItem, IItemReferences
    {
        public override ItemType ItemType => ItemType.Prefab;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        internal override bool HasDataPayload => true;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

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
        /// The prefab variant.
        /// </summary>
        public Token Variant { get; set; }

        /// <summary>
        /// The prefab look.
        /// </summary>
        public Token Look { get; set; }

        /// <summary>
        /// The nodes belonging to this prefab.
        /// </summary>
        public List<INode> Nodes;

        /// <summary>
        /// The index of the origin node.
        /// <para>This defines which of the ppd nodes is the origin node.
        /// It is not an index for the Nodes property as the origin node is
        /// always the 0th node in the list.</para>
        /// </summary>
        public ushort Origin { get; internal set; }

        /// <summary>
        /// Unit names of additional parts used.
        /// </summary>
        public List<Token> AdditionalParts { get; set; }

        public byte DlcGuard
        {
            get => Kdop.Flags.GetByte(1);
            set => Kdop.Flags.SetByte(1, value);
        }

        /// <summary>
        /// Vegetation and terrain data for each prefab corner.
        /// </summary>
        public PrefabCorner[] Corners { get; set; }

        /// <summary>
        /// Semaphore profile for this crossing.
        /// </summary>
        public Token SemaphoreProfile { get; set; }

        public IMapItem FerryLink { get; set; }

        public uint RandomSeed { get; set; }

        public List<IMapItem> SlaveItems { get; set; }

        public List<VegetationPart> VegetationParts { get; set; } 

        public List<VegetationSphere> VegetationSpheres { get; set; } 

        /// <summary>
        /// Determines if the prefab is a tunnel. 
        /// This will make AI vehicles turn on their headlights.
        /// </summary>
        public bool IsTunnel
        {
            get => Kdop.Flags[0];
            set => Kdop.Flags[0] = value;
        }

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is rendered
        /// if the selected terrain material supports it.
        /// </summary>
        public bool DetailVegetation
        {
            get => !Kdop.Flags[23];
            set => Kdop.Flags[23] = !value;
        }

        /// <summary>
        /// Determines if the item uses left hand traffic.
        /// </summary>
        public bool LeftHandTraffic
        {
            get => Kdop.Flags[22];
            set => Kdop.Flags[22] = value;
        }

        /// <summary>
        /// Enables activation points. 
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
        /// Previously used to mark the prefab as a state border.
        /// </summary>
        [Obsolete]
        public bool IsStateBorder
        {
            get => Kdop.Flags[18];
            set => Kdop.Flags[18] = value;
        }

        /// <summary>
        /// Determines if this prefab is displayed in the UI map.
        /// </summary>
        public bool ShowInUiMap
        {
            get => !Kdop.Flags[17];
            set => Kdop.Flags[17] = !value;
        }

        /// <summary>
        /// Determines if this road has invisible walls on its sides.
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
        /// Determines if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation
        {
            get => Kdop.Flags[29];
            set => Kdop.Flags[29] = value;
        }

        public bool NoDetailVegetation
        {
            get => Kdop.Flags[23];
            set => Kdop.Flags[23] = value;
        }

        /// <summary>
        /// Determines if AI traffic can use this road.
        /// <para>If not, AI vehicles will choose a different route.
        /// If there isn't one, they will despawn instead.</para>
        /// </summary>
        public bool AiVehicles
        {
            get => !Kdop.Flags[27];
            set => Kdop.Flags[27] = !value;
        }

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Kdop.Flags[26];
            set => Kdop.Flags[26] = value;
        }

        /// <summary>
        /// This should be set to true on the last prefab before a ferry.
        /// </summary>
        public bool IsFerryEntrance
        {
            get => Kdop.Flags[25];
            set => Kdop.Flags[25] = value;
        }

        /// <summary>
        /// Determines if the item is reflected on water surfaces.
        /// </summary>
        public bool WaterReflection
        {
            get => Kdop.Flags[24];
            set => Kdop.Flags[24] = value;
        }

        public bool Collision
        {
            get => !Kdop.Flags[2];
            set => Kdop.Flags[2] = !value;
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
                Corners = new PrefabCorner[6].Select(h => new PrefabCorner(false)).ToArray();
            }
        }

        protected override void Init()
        {
            base.Init();
            Corners = new PrefabCorner[6].Select(h => new PrefabCorner()).ToArray();
            Nodes = new List<INode>(2);
            AdditionalParts = new List<Token>();
            SlaveItems = new List<IMapItem>();
            VegetationParts = new List<VegetationPart>();
            VegetationSpheres = new List<VegetationSphere>();
        }

        /// <summary>
        /// Creates a prefab item from a ppd and adds it to the map.
        /// </summary>
        /// <param name="map">The map the prefab is added to.</param>
        /// <param name="ppd">The prefab descriptor file defining the prefab.</param>
        /// <param name="unitName">The unit name of the prefab.</param>
        /// <param name="position">The position of control node 0.</param>
        /// <returns></returns>
        public static Prefab Add(IItemContainer map, string unitName, string variant, string look, 
            PrefabDescriptor ppd, Vector3 position, Quaternion? rotation = null)
        {
            return new PrefabCreator().FromPpd(map, unitName, variant, look, ppd, 
                position, rotation.GetValueOrDefault(Quaternion.Identity));
        }

        [Obsolete]
        public static Prefab Add(IItemContainer map, Token model, IList<Vector3> positions, ushort origin = 0)
        {
            var prefab = new Prefab();

            for(int i = 0; i < positions.Count; i++)
            {
                var node = map.AddNode(positions[i], i == 0);
                node.ForwardItem = prefab;
                prefab.Nodes.Add(node);
            }
            
            prefab.Model = model;
            prefab.Origin = origin;

            map.AddItem(prefab);
            return prefab;
        }

        /// <summary>
        /// Appends a new road to a prefab.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="node">The index of the prefab node to attach to.</param>
        /// <param name="forwardPos">The position of the road's forward node.</param>
        /// <param name="type">The road type.</param>
        /// <param name="leftTerrainSize">The left terrain size.</param>
        /// <param name="rightTerrainSize">The right terrain size.</param>
        /// <returns>The new road.</returns>
        public Road AppendRoad(IItemContainer map, ushort node, Vector3 forwardPos,
            Token type, float leftTerrainSize = 0f, float rightTerrainSize = 0f)
        {
            if(node > Nodes.Count)
                throw new IndexOutOfRangeException($"This prefab only has {Nodes.Count} nodes.");

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
        /// Attaches the ForwardNode of a polyline item to the specified node of this prefab.
        /// The polyline's ForwardNode will be deleted.
        /// </summary>
        /// <param name="item">The polyline item to attach.</param>
        /// <param name="prefabNodeIdx">The index of the prefab node to attach to.</param>
        public void Attach(PolylineItem item, INode itemNode, ushort prefabNodeIdx)
        {
            if (prefabNodeIdx > Nodes.Count)
                throw new ArgumentOutOfRangeException($"This prefab only has {Nodes.Count} nodes.");

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

            //item.Recalculate(); apparently we don't need this?
        }

        /// <summary>
        /// Attaches the ForwardNode of a polyline item to the closest node of this prefab.
        /// The polyline's ForwardNode will be deleted.
        /// </summary>
        /// <param name="item"></param>
        public void Attach(PolylineItem item)
        {
            // find closest node
            float shortestDist = float.MaxValue;
            INode closestPrefabNode = null;
            INode closestItemNode = null;
            foreach(var node in Nodes)
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
        /// Attaches another prefab to a node of this prefab, assuming that
        /// at least one of the nodes has the same position as a node of this prefab.
        /// </summary>
        /// <param name="p2"></param>
        public void Attach(Prefab p2)
        {
            // find overlapping node(s). 
            // the Intersect call returns the
            // nodes of this prefab which will replace the corresponding newPf nodes.
            var overlappingNodes = Nodes.Intersect(p2.Nodes, new NodePositionComparer()).ToList();
            if (overlappingNodes.Count() == 0)
                throw new NotImplementedException("No overlapping node found - can't attach prefab");

            for (var i = 0; i < overlappingNodes.Count(); i++)
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
        /// Checks if two prefabs are connected to each other at one or more nodes.
        /// </summary>
        /// <param name="p2">The other prefab.</param>
        /// <returns></returns>
        public bool IsAttachedTo(Prefab p2) =>
            Nodes.Intersect(p2.Nodes).Any();

        public override void Move(Vector3 newPos)
        {
            Move(newPos, 0);
        }

        public void Move(Vector3 newPos, ushort nodeIdx)
        {
            var translation = newPos - Nodes[nodeIdx].Position;
            Translate(translation);
        }

        public override void Translate(Vector3 translation)
        {
            foreach (var node in Nodes)
                node.Move(node.Position + translation);

            foreach (var si in SlaveItems)
                (si as PrefabSlaveItem).Translate(translation);
        }

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

        internal override IEnumerable<INode> GetItemNodes() =>
            new List<INode>(Nodes);

        internal override INode GetMainNode() => Nodes[0];

        internal override void UpdateNodeReferences(Dictionary<ulong, INode> allNodes)
        {
            ResolveNodeReferences(Nodes, allNodes);
        }

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
