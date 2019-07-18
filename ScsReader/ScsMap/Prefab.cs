using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ScsReader.ScsMap
{
    /// <summary>
    /// Represents a prefab.
    /// </summary>
    public class Prefab : MapItem, IDataPart, IItemReferences
    {
        // TODO: Use the new KdopItem system

        public override ItemType ItemType => ItemType.Prefab;

        public override ItemFile DefaultItemFile => ItemFile.Base;

        protected override ushort DefaultViewDistance => KdopItem.ViewDistanceClose;

        public new ushort ViewDistance
        {
            get => base.ViewDistance;
            set => base.ViewDistance = value;
        }

        /// <summary>
        /// The unit name of the prefab.
        /// </summary>
        public Token Model;

        /// <summary>
        /// The prefab variant.
        /// </summary>
        public Token Variant;

        /// <summary>
        /// The prefab look.
        /// </summary>
        public Token Look;

        /// <summary>
        /// The nodes belonging to this prefab.
        /// </summary>
        public List<Node> Nodes = new List<Node>();

        /// <summary>
        /// The index of the origin node.
        /// </summary>
        public ushort Origin;

        /// <summary>
        /// Unit names of additional parts used.
        /// </summary>
        public List<Token> AdditionalParts = new List<Token>();

        public byte DlcGuard
        {
            get => Flags.GetByte(1);
            set => Flags.SetByte(1, value);
        }

        /// <summary>
        /// Vegetation and terrain data for each of the prefab's corners.
        /// </summary>
        public PrefabCorner[] Corners;

        /// <summary>
        /// Semaphore profile for this crossing.
        /// </summary>
        public Token SemaphoreProfile;

        public MapItem FerryLink;

        public uint RandomSeed;

        public List<MapItem> SlaveItems = new List<MapItem>();

        public List<VegetationPart> VegetationParts = new List<VegetationPart>();

        public List<VegetationSphere> VegetationSpheres = new List<VegetationSphere>();

        /// <summary>
        /// Determines if the prefab is a tunnel. 
        /// This will make AI vehicles turn on their headlights.
        /// </summary>
        public bool IsTunnel
        {
            get => Flags[0];
            set => Flags[0] = value;
        }

        /// <summary>
        /// Determines if detail vegetation (small clumps of grass etc.) is drawn.
        /// </summary>
        public bool DetailVegetation
        {
            get => !Flags[23];
            set => Flags[23] = !value;
        }

        /// <summary>
        /// Determines if the item uses left hand traffic.
        /// </summary>
        public bool LeftHandTraffic
        {
            get => Flags[22];
            set => Flags[22] = value;
        }

        public bool Activation
        {
            get => !Flags[21];
            set => Flags[21] = !value;
        }

        /// <summary>
        /// Allows customization of selected semaphore profile
        /// by removing unwanted semaphore instances
        /// at complex crossroads consisting of multiple prefabs.
        /// </summary>
        public bool CustomSemaphores
        {
            get => Flags[1];
            set => Flags[1] = value;
        }

        /// <summary>
        /// Determines if the prefab marks a state border.
        /// </summary>
        public bool IsStateBorder
        {
            get => Flags[18];
            set => Flags[18] = value;
        }

        /// <summary>
        /// Determines if this prefab is displayed in the UI map.
        /// </summary>
        public bool ShowInUiMap
        {
            get => !Flags[17];
            set => Flags[17] = !value;
        }

        public bool Boundary
        {
            get => !Flags[16];
            set => Flags[16] = !value;
        }

        public bool TerrainShadows
        {
            get => !Flags[30];
            set => Flags[30] = !value;
        }

        /// <summary>
        /// Determines if only flat textures are used as vegetation.
        /// </summary>
        public bool LowPolyVegetation
        {
            get => Flags[29];
            set => Flags[29] = value;
        }

        /// <summary>
        /// Determines if AI traffic can use this road.
        /// <para>If not, AI vehicles will choose a different route.
        /// If there isn't one, they will despawn instead.</para>
        /// </summary>
        public bool AiVehicles
        {
            get => !Flags[27];
            set => Flags[27] = !value;
        }

        /// <summary>
        /// Determines if this item will render behind a cut plane.
        /// </summary>
        public bool IgnoreCutPlanes
        {
            get => Flags[26];
            set => Flags[26] = value;
        }

        public bool IsFerryEntrance
        {
            get => Flags[25];
            set => Flags[25] = value;
        }

        /// <summary>
        /// Determines if the item is reflected in water.
        /// </summary>
        public bool WaterReflection
        {
            get => Flags[24];
            set => Flags[24] = value;
        }

        public Prefab()
        {
            const int cornerAmnt = 4;
            Corners = new PrefabCorner[cornerAmnt].Select(h => new PrefabCorner()).ToArray();
        }

        public static Prefab Add(IItemContainer map, Token model, Vector3[] positions, ushort origin)
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

            var r = new Road
            {
                Node = backwardNode,
                ForwardNode = forwardNode
            };
            r.Node.ForwardItem = r;
            r.ForwardNode.BackwardItem = r;
            r.InitFromAddOrAppend(backwardNode.Position, forwardNode.Position, type);
            map.AddItem(r, r.Node);

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
                var eulerRot = backwardNode.Rotation.ToEuler();
                eulerRot.Y -= (float)Math.PI;
                backwardNode.Rotation = Quaternion.CreateFromYawPitchRoll(eulerRot.Y, eulerRot.X, eulerRot.Z);
                forwardNode.Rotation = MathEx.GetNodeRotation(backwardNode.Position, forwardNode.Position);
            }

            backwardNode.IsRed = true;

            return r;
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

        private const float terrainSizeFactor = 10f;
        private const float vegFromToFactor = 10f;

        public override void ReadFromStream(BinaryReader r)
        {
            base.ReadFromStream(r);

            // Model
            Model = r.ReadToken();

            // Variant
            Variant = r.ReadToken();

            // Additional parts, e.g. lines on an intersection
            AdditionalParts = ReadObjectList<Token>(r);

            // Node uids
            Nodes = ReadNodeRefList(r);

            // Slave uids
            // (link to service items)
            SlaveItems = ReadItemRefList(r);

            // Ferry link
            var ferryLinkUid = r.ReadUInt64();
            if (ferryLinkUid != 0)
            {
                FerryLink = new UnresolvedItem(ferryLinkUid);
            }

            // Origin node
            Origin = r.ReadUInt16();

            // Terrain data
            for (int i = 0; i < Nodes.Count; i++)
            {
                Corners[i].Terrain.Profile = r.ReadToken();
                Corners[i].Terrain.Coefficient = r.ReadSingle();
            }

            // semaphore profile
            SemaphoreProfile = r.ReadToken();
        }

        public void ReadDataPart(BinaryReader r)
        {
            // Prefab look
            Look = r.ReadToken();

            // 4 node corners
            foreach (var corner in Corners)
            {
                // Terrain size
                corner.Terrain.Size = r.ReadUInt16() / terrainSizeFactor;

                // Detail vegetation sliders
                corner.DetailVegetationFrom = r.ReadUInt16() / vegFromToFactor;
                corner.DetailVegetationTo = r.ReadUInt16() / vegFromToFactor;

                // Vegetation; 4 nodes with 2 veg. objects
                foreach (var veg in corner.Vegetation)
                {
                    veg.ReadFromStream(r);
                }
            }

            // vegetation parts
            VegetationParts = ReadObjectList<VegetationPart>(r);

            // seed
            RandomSeed = r.ReadUInt32();

            // vegetation spheres
            VegetationSpheres = ReadObjectList<VegetationSphere>(r);

            // Corner models
            foreach (var corner in Corners)
            {
                corner.Model = r.ReadToken();
                corner.Variant = r.ReadToken();
                corner.Look = r.ReadToken();
            }

            // Terrain
            foreach(var corner in Corners)
            {
                corner.Terrain.QuadData.ReadFromStream(r);
            }
        }

        public override void WriteToStream(BinaryWriter w)
        {
            base.WriteToStream(w);

            // Model
            w.Write(Model);

            // Variant
            w.Write(Variant);

            // Additional parts
            WriteObjectList(w, AdditionalParts);

            // Nodes
            WriteNodeRefList(w, Nodes);

            // Slave nodes
            WriteItemRefList(w, SlaveItems);

            // Ferry link
            if (FerryLink is null)
            {
                w.Write(0UL);
            }
            else
            {
                w.Write(FerryLink.Uid);
            }

            // Origin
            w.Write(Origin);

            // Terrain data
            for (int i = 0; i < Nodes.Count; i++)
            {
                w.Write(Corners[i].Terrain.Profile);
                w.Write(Corners[i].Terrain.Coefficient);
            }

            // Semaphore profile
            w.Write(SemaphoreProfile);
        }

        public void WriteDataPart(BinaryWriter w)
        {
            // Prefab look
            w.Write(Look);

            // Corner veg.
            foreach (var corner in Corners)
            {
                // Terrain size
                w.Write((ushort)(corner.Terrain.Size * terrainSizeFactor));

                // Detail vegetation sliders
                w.Write((ushort)(corner.DetailVegetationFrom * vegFromToFactor));
                w.Write((ushort)(corner.DetailVegetationTo * vegFromToFactor));

                // Vegetation
                foreach (var veg in corner.Vegetation)
                {
                    veg.WriteToStream(w);
                }
            }

            // vegetation parts
            WriteObjectList(w, VegetationParts);

            // seed
            w.Write(RandomSeed);

            // veg. spheres
            WriteObjectList(w, VegetationSpheres);

            // corner models
            foreach (var corner in Corners)
            {
                w.Write(corner.Model);
                w.Write(corner.Variant);
                w.Write(corner.Look);
            }

            // Terrain
            foreach (var corner in Corners)
            {
                corner.Terrain.QuadData.WriteToStream(w);
            }
        }

        public override void UpdateNodeReferences(Dictionary<ulong, Node> allNodes)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is UnresolvedNode && allNodes.ContainsKey(Nodes[i].Uid))
                {
                    Nodes[i] = allNodes[Nodes[i].Uid];
                }
            }
        }

        public void UpdateItemReferences(Dictionary<ulong, MapItem> allItems)
        {
            if (FerryLink is UnresolvedItem && allItems.ContainsKey(FerryLink.Uid))
            {
                FerryLink = allItems[FerryLink.Uid];
            }

            for (int i = 0; i < SlaveItems.Count; i++)
            {
                if (SlaveItems[i] is UnresolvedItem && allItems.ContainsKey(SlaveItems[i].Uid))
                {
                    SlaveItems[i] = allItems[SlaveItems[i].Uid];
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
