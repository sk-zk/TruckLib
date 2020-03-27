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
        public List<Node> Nodes;

        /// <summary>
        /// The index of the origin node.
        /// </summary>
        public ushort Origin { get; set; } = 0;

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
        /// Vegetation and terrain data for each of the prefab's corners.
        /// </summary>
        public PrefabCorner[] Corners { get; set; }

        /// <summary>
        /// Semaphore profile for this crossing.
        /// </summary>
        public Token SemaphoreProfile { get; set; }

        public MapItem FerryLink { get; set; }

        public uint RandomSeed { get; set; }

        public List<MapItem> SlaveItems { get; set; }

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
        /// Determines if detail vegetation (small clumps of grass etc.) is drawn.
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
        /// Determines if the prefab marks a state border.
        /// </summary>
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

        public bool IsFerryEntrance
        {
            get => Kdop.Flags[25];
            set => Kdop.Flags[25] = value;
        }

        /// <summary>
        /// Determines if the item is reflected in water.
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
            Corners = new PrefabCorner[4]
                .Select(h => new PrefabCorner()).ToArray();
        }

        internal Prefab(bool initFields) : base(initFields)
        {
            if (initFields) Init();
            Corners = new PrefabCorner[4]
                .Select(h => new PrefabCorner()).ToArray();
        }

        protected override void Init()
        {
            base.Init();
            Nodes = new List<Node>(2);
            AdditionalParts = new List<Token>();
            SlaveItems = new List<MapItem>();
            VegetationParts = new List<VegetationPart>();
            VegetationSpheres = new List<VegetationSphere>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="map">The map the prefab is added to.</param>
        /// <param name="ppd">The prefab descriptor file defining the prefab.</param>
        /// <param name="unitName">The unit name of the prefab.</param>
        /// <param name="position">The position of control node 0.</param>
        /// <returns></returns>
        public static Prefab Add(IItemContainer map, string unitName, string variant, string look, PrefabDescriptor ppd, Vector3 position)
        {
            return new PrefabCreator().FromPpd(map, unitName, variant, look, ppd, position);
        }

        [Obsolete]
        public static Prefab Add(IItemContainer map, Token model, Vector3[] positions, ushort origin = 0)
        {
            var prefab = new Prefab();

            for(int i = 0; i < positions.Length; i++)
            {
                var node = map.AddNode(positions[i], i == 0);
                node.ForwardItem = prefab;
                prefab.Nodes.Add(node);
            }
            
            prefab.Model = model;
            prefab.Origin = origin;

            map.AddItem(prefab, prefab.Nodes[0]);
            // TODO: Which node determines the sector? 
            // Nodes[0] or Nodes[origin]?
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
            {
                throw new ArgumentOutOfRangeException($"This prefab only has {Nodes.Count} nodes.");
            }

            Node backwardNode;
            Node forwardNode;

            // if the node to attach to is the origin node,
            // the road has to be *prepended* instead
            var prepend = (node == Origin);
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
            road.InitFromAddOrAppend(backwardNode.Position, forwardNode.Position, type);
            map.AddItem(road, road.Node);

            // nodes of a prefab that have nothing attached to it
            // always have the prefab as ForwardItem, but they will be
            // set to BackwardItem when you attach a road going outward
            if (!prepend) backwardNode.BackwardItem = this;
                
            if (prepend)
            {
                backwardNode.Rotation = MathEx.GetNodeRotation(backwardNode.Position, forwardNode.Position);
            }
            else
            {
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
        public void Attach(PolylineItem item, Node itemNode, ushort prefabNodeIdx)
        {
            if (prefabNodeIdx > Nodes.Count)
            {
                throw new ArgumentOutOfRangeException($"This prefab only has {Nodes.Count} nodes.");
            }

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
                var rot = oldPfNode.Rotation.ToEuler();
                rot.Y -= (float)Math.PI;
                itemNode.Rotation = Quaternion.CreateFromYawPitchRoll(rot.Y, rot.X, rot.Z);
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
            Node closestPrefabNode = null;
            Node closestItemNode = null;
            foreach(var node in Nodes)
            {
                foreach(var itemNode in new Node[] { item.ForwardNode, item.Node })
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
            item.Recalculate();
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
            {
                throw new NotImplementedException("No overlapping node found - can't attach prefab");
            }

            for(var i = 0; i < overlappingNodes.Count(); i++)
            {
                var p1Node = overlappingNodes[i];
                var p1NodeIdx = Nodes.IndexOf(p1Node);
                var p2NodeIdx = p2.Nodes.FindIndex(x => x.Position == p1Node.Position);
                var p2Node = p2.Nodes[p2NodeIdx];

                // if the p2 node is the origin, the nodes have to be merged to the p2 node.
                // otherwise, the p1 node survives
                if (p2NodeIdx == p2.Origin)
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
        /// Checks if two prefabs are connected to each other at one 
        /// or more nodes.
        /// </summary>
        /// <param name="p2">The other prefab.</param>
        /// <returns></returns>
        public bool IsAttachedTo(Prefab p2)
        {
            return Nodes.Intersect(p2.Nodes).Any();
        }

        /// <summary>
        /// Moves the prefab to a different location.
        /// </summary>
        /// <param name="newPos">The new absolute position
        /// of the origin node.</param>
        public void Move(Vector3 newPos)
        {
            var translation = newPos - Nodes[Origin].Position;
            MoveRel(translation);
        }

        /// <summary>
        /// Translates the prefab to a different location.
        /// </summary>
        /// <param name="translation">The translation vector.</param>
        public void MoveRel(Vector3 translation)
        {
            foreach (var node in Nodes)
            {
                node.Move(node.Position + translation);
            }

            foreach (var si in SlaveItems)
            {
                (si as PrefabSlaveItem).MoveRel(translation);
            }
        }

        internal override IEnumerable<Node> GetItemNodes()
        {
            return new List<Node>(Nodes);
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode
                    && allNodes.TryGetValue(Nodes[i].Uid, out var resolvedNode))
                {
                    Nodes[i] = resolvedNode;
                }
            }
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
        internal class NodePositionComparer : IEqualityComparer<Node>
        {
            public bool Equals(Node x, Node y)
            {
                return x.Position == y.Position;
            }

            public int GetHashCode(Node obj)
            {
                // apparently this has to happen for Equals to be called
                return 0;
            }
        }
    }

     
}
