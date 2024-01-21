using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.ScsMap
{
    /// <summary>
    /// A map for Euro Truck Simulator 2 or American Truck Simulator.
    /// </summary>
    public class Map : IItemContainer
    {
        /// <summary>
        /// The name of the map, which is used for file and directory names.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The map's sectors.
        /// </summary>
        public Dictionary<ValueTuple<int, int>, Sector> Sectors { get; set; } 
            = new Dictionary<ValueTuple<int, int>, Sector>();

        /// <summary>
        /// Contains all nodes in this sector.
        /// </summary>
        // I've pulled the node dictionary into Map instead of having one dict
        // per Sector. This is because certain nodes of items that cross 
        // sector boundaries are written to both sectors, and doing this was
        // the best way I could think of to prevent two instances of 
        // the same node.
        public Dictionary<ulong, INode> Nodes { get; internal set; }
            = new Dictionary<ulong, INode>();

        /// <summary>
        /// Scale and time compression of the game outside cities.
        /// </summary>
        public float NormalScale { get; set; } = 19;

        /// <summary>
        /// Scale and time compression of the game inside cities.
        /// </summary>
        public float CityScale { get; set; } = 3;

        /// <summary>
        /// Editor start position. TODO: Figure out these values
        /// </summary>
        private Vector3 StartPlacementPosition = new(0, 0, 0);

        /// <summary>
        /// Editor start position. TODO: Figure out these values
        /// </summary>
        private uint StartPlacementSectorOrSomething = 0x4B000800;

        /// <summary>
        /// Editor start rotation.
        /// </summary>
        private Quaternion StartPlacementRotation = Quaternion.Identity;

        /// <summary>
        /// <para>Gets or sets if SCS's Europe map UI corrections are enabled.</para>
        /// <para>Nobody seems to know definitively what this does, but it might have
        /// something to do with the scale of the UK in <c>europe.mbd.</c></para>
        /// </summary>
        public bool EuropeMapUiCorrections { get; set; } = false;

        public ulong EditorMapId { get; set; }

        // This value is used in both ETS2 and ATS.
        private uint gameTag = 2998976734; //TODO: What is this?

        /// <summary>
        /// The map's header.
        /// </summary>
        private Header header = new();

        /// <summary>
        /// The size of a sector in engine units (= meters).
        /// </summary>
        public static readonly int SectorSize = 4000;

        /// <summary>
        /// Creates an empty map.
        /// </summary>
        /// <param name="name">The name of the map.</param>
        public Map(string name)
        {
            Name = name;
            EditorMapId = Utils.GenerateUuid();
        }

        /// <summary>
        /// Opens a map.
        /// </summary>
        /// <param name="mbdPath">Path to the .mbd file of the map.</param>
        /// <param name="sectors">If set, only the given sectors will be loaded.
        /// The method expects strings in the same format as the file names of the sectors
        /// (without the extension), e.g. <c>sec+0001+0002</c>.</param>
        public static Map Open(string mbdPath, string[] sectors = null)
        {
            Trace.WriteLine("Loading map " + mbdPath);
            var name = Path.GetFileNameWithoutExtension(mbdPath);
            var mapDirectory = Directory.GetParent(mbdPath).FullName;
            var sectorDirectory = Path.Combine(mapDirectory, name);

            var map = new Map(name);
            map.ReadMbd(mbdPath);
            Trace.WriteLine("Parsing sectors");
            map.ReadSectors(sectorDirectory, sectors);

            Trace.WriteLine("Updating references");
            map.UpdateReferences();

            return map;
        }

        /// <summary>
        /// Creates a new sector.
        /// </summary>
        /// <param name="x">The X coordinate of the sector.</param>
        /// <param name="z">The Z coordinate of the sector.</param>
        /// <returns>The new sector.</returns>
        public Sector AddSector(int x, int z)
        {
            if (Sectors.TryGetValue((x, z), out var existing))
                return existing;
            
             var sector = new Sector(x, z, this);
             Sectors.Add((x, z), sector);
             return sector;
        }

        /// <summary>
        /// Creates a new node and adds it to its corresponding sector. 
        /// If that sector does not yet exist, it will be created automatically.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <returns>The new node.</returns>
        public Node AddNode(Vector3 position)
        {
            return AddNode(position, false);
        }

        /// <summary>
        /// Creates a new node and adds it to its corresponding sector. 
        /// If that sector does not yet exist, it will be created automatically.
        /// </summary>
        /// <param name="position">The position of the node.</param>
        /// <param name="isRed">Whether the node is red.</param>
        /// <returns>The new node.</returns>
        public Node AddNode(Vector3 position, bool isRed)
        {
            var sectorIdx = GetSectorOfCoordinate(position);
            if (!Sectors.ContainsKey(sectorIdx))
            {
                AddSector(sectorIdx.X, sectorIdx.Z);
            }

            var node = new Node();
            node.Sectors = node.Sectors.Push(Sectors[sectorIdx]);
            node.Position = position;
            node.IsRed = isRed;
            Nodes.Add(node.Uid, node);
            return node;
        }

        /// <summary>
        /// Adds an item to the map. This is the final step in the Add() method of an item
        /// and should not be called on its own.
        /// </summary>
        /// <param name="item">The item.</param>
        void IItemContainer.AddItem(MapItem item)
        {
            item.GetMainNode().Sectors[0].MapItems.Add(item.Uid, item);
        }

        /// <summary>
        /// Returns the index of the sector the given coordinate falls into.
        /// </summary>
        /// <param name="c">The coordinate to check.</param>
        /// <returns>The index of the sector the coordinate is in.</returns>
        public static (int X, int Z) GetSectorOfCoordinate(Vector3 c)
        {
            return (
                (int)Math.Floor(c.X / SectorSize), 
                (int)Math.Floor(c.Z / SectorSize)
                );
        }

        /// <summary>
        /// Returns a dictionary containing all nodes in the entire map.
        /// </summary>
        /// <returns>All nodes in the entire map.</returns>
        public Dictionary<ulong, INode> GetAllNodes()
        {       
            return Nodes;
        }

        /// <summary>
        /// Returns a dictionary containing all map items from all sectors.
        /// </summary>
        /// <returns>All map items in the entire map.</returns>
        public Dictionary<ulong, MapItem> GetAllItems()
        {
            var allItems = new Dictionary<ulong, MapItem>();
            foreach (var sectorKvp in Sectors)
            {
                foreach (var itemKvp in sectorKvp.Value.MapItems)
                {
                    allItems.Add(itemKvp.Key, itemKvp.Value);
                }
            }
            return allItems;
        }

        /// <summary>
        /// Returns a dictionary containing all items of type T from all sectors.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <returns>All items of this type in the entire map.</returns>
        public Dictionary<ulong, T> GetAllItems<T>() where T : MapItem
        {
            var allItems = new Dictionary<ulong, T>();
            foreach (var sectorKvp in Sectors)
            {
                foreach (var itemKvp in sectorKvp.Value.MapItems)
                {
                    if (itemKvp.Value is T t)
                    {
                        allItems.Add(itemKvp.Key, t);
                    }
                }
            }
            return allItems;
        }

        /// <summary>
        /// Checks if the map contains an item with the given UID.
        /// </summary>
        /// <param name="uid">The UID to search for.</param>
        /// <returns>Whether an item with this UID exists.</returns>
        public bool HasItem(ulong uid)
        {
            foreach (var sectorKvp in Sectors)
            {
                if (sectorKvp.Value.MapItems.ContainsKey(uid))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the item with the given UID.
        /// </summary>
        /// <param name="uid">The UID to search for.</param>
        /// <returns>Returns the item, or null if it doesn't exist.</returns>
        public MapItem GetItem(ulong uid)
        {
            foreach (var sectorKvp in Sectors)
            {
                if (sectorKvp.Value.MapItems.TryGetValue(uid, out var item))
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Deletes an item. Nodes that are only used by this item 
        /// will also be deleted.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void Delete(MapItem item)
        {
            // delete item from all sectors
            foreach (var sectorKvp in Sectors)
            {
                sectorKvp.Value.MapItems.Remove(item.Uid);
            }
           
            // remove item from its nodes, 
            // and delete them if they're orphaned now
            foreach (var node in item.GetItemNodes())
            {
                if (node.ForwardItem == item)
                {
                    node.ForwardItem = null;
                    node.IsRed = false;
                }
                if (node.BackwardItem == item)
                {
                    node.BackwardItem = null;
                }
                if (node.IsOrphaned())
                {
                    Delete(node);
                }
            }

            // delete dependent items
            if (item is Prefab pf)
            {
                foreach (var slaveItem in pf.SlaveItems)
                {
                    if (slaveItem is MapItem mapItem)
                        Delete(mapItem);
                    // if Unresolved, just ignore it
                }
            }
        }

        /// <summary>
        /// Deletes a node and the items attached to it.
        /// </summary>
        /// <param name="node">The node to delete.</param>
        public void Delete(INode node)
        {
            Nodes.Remove(node.Uid);
            
            if (node.ForwardItem is MapItem fw)
            {
                node.ForwardItem = null;
                Delete(fw);
            }

            if (node.BackwardItem is MapItem bw)
            {
                node.BackwardItem = null;
                Delete(bw);
            }
        }

        /// <summary>
        /// Imports the contents of a Selection (.sbd) file into this map.
        /// </summary>
        /// <param name="selection">The Selection to import.</param>
        /// <param name="position">The point relative to which the items will be inserted.</param>
        public void Import(Selection selection, Vector3 position)
        {
            // deep cloning everything the lazy way

            var clonedItems = selection.Items.Select(x => x.CloneItem())
                .ToDictionary(k => k.Uid, v => v);
            var clonedNodes = selection.Nodes.Select(x => (x as Node).Clone())
                .ToDictionary(k => k.Uid, v => (INode)v);

            foreach (var nodeKvp in clonedNodes)
            {
                var node = nodeKvp.Value;

                node.Position += position - selection.Origin;
                node.UpdateItemReferences(clonedItems);
                var sectorIdx = GetSectorOfCoordinate(node.Position);
                AddSector(sectorIdx.X, sectorIdx.Z);
                node.Sectors = new[] { Sectors[sectorIdx] };
                Nodes.Add(node.Uid, node);
            }

            foreach (var itemKvp in clonedItems)
            {
                var item = itemKvp.Value;

                item.UpdateNodeReferences(clonedNodes);
                if (item is IItemReferences itemRefs)
                    itemRefs.UpdateItemReferences(clonedItems);
                (this as IItemContainer).AddItem(item);
            }
        }

        /// <summary>
        /// Reads the .mbd file of a map.
        /// </summary>
        /// <param name="mbdPath">The path to the .mbd file.</param>
        private void ReadMbd(string mbdPath)
        {
            using var r = new BinaryReader(new MemoryStream(File.ReadAllBytes(mbdPath)));

            var header = new Header();
            header.Deserialize(r);

            EditorMapId = r.ReadUInt64();

            StartPlacementPosition = r.ReadVector3();
            StartPlacementSectorOrSomething = r.ReadUInt32();

            StartPlacementRotation = r.ReadQuaternion();

            gameTag = r.ReadUInt32();

            NormalScale = r.ReadSingle();
            CityScale = r.ReadSingle();

            EuropeMapUiCorrections = (r.ReadByte() == 1);
        }

        /// <summary>
        /// Reads the sectors of this map.
        /// </summary>
        /// <param name="mapDirectory">The main map directory.</param>
        /// <param name="sectors">If set, only the given sectors will be loaded.</param>
        private void ReadSectors(string mapDirectory, string[] sectors = null)
        {
            var baseFiles = Directory.GetFiles(mapDirectory, "*.base");

            // create itemless instances for all the sectors first;
            // this is so we have references to the sectors
            // before we read them to deal with sectors containing
            // nodes from other sectors
            foreach (var baseFile in baseFiles)
            {
                if (!SectorShouldBeLoaded(baseFile))
                    continue;

                var sector = new Sector(baseFile, this);
                Sectors.Add((sector.X, sector.Z), sector);
            }

            var sectorList = Sectors.ToList();
            foreach (var sectorKvp in sectorList)
            {
                Trace.WriteLine($"Reading sector {sectorKvp.Value}");
                sectorKvp.Value.Read();
            }

            bool SectorShouldBeLoaded(string baseFile) =>
                sectors == null || sectors.Contains(Path.GetFileNameWithoutExtension(baseFile));          
        }

        /// <summary>
        /// Fills in the Node and Forward/BackwardItem fields in map item and node objects
        /// by searching the Node or Item list for the Uid references.
        /// </summary>
        private void UpdateReferences()
        {
            var allNodes = GetAllNodes();
            var allItems = GetAllItems();

            // first of all, find map items referenced in nodes
            Trace.WriteLine("Updating item references in nodes");
            foreach (var kvp in allNodes)
            {
                kvp.Value.UpdateItemReferences(allItems);
            }

            // then find nodes referenced in map items
            // and map items referenced in map items
            Trace.WriteLine("Updating node & item references in items");
            foreach (var item in allItems)
            {
                item.Value.UpdateNodeReferences(allNodes);
                if (item.Value is IItemReferences hasItemRef)
                {
                    hasItemRef.UpdateItemReferences(allItems);
                }
            }
        }

        /// <summary>
        /// Saves the map in binary format.
        /// </summary>
        /// <param name="mapDirectory">The directory to save the map to.</param>
        /// <param name="cleanDir">If true, the sectors folder will be emptied before saving the map.</param>
        public void Save(string mapDirectory, bool cleanDir = true)
        {
            var sectorDirectory = Path.Combine(mapDirectory, Name);
            Directory.CreateDirectory(sectorDirectory);
            if (cleanDir)
            {
                new DirectoryInfo(sectorDirectory).GetFiles().ToList()
                    .ForEach(f => f.Delete());
            }

            var sectorNodes = GetSectorNodes();
            var visAreaShowObjectsChildren = GetVisAreaShowObjectsChildUids();

            foreach (var sectorKvp in Sectors)
            {
                Trace.WriteLine($"Writing sector {sectorKvp.Value}");
                sectorKvp.Value.Save(sectorDirectory, sectorNodes[sectorKvp.Value], visAreaShowObjectsChildren);
            }

            var mbdPath = Path.Combine(mapDirectory, $"{Name}.mbd");
            SaveMbd(mbdPath);

            Dictionary<Sector, List<INode>> GetSectorNodes()
            {
                var sectorNodes = new Dictionary<Sector, List<INode>>();
                foreach (var sectorKvp in Sectors)
                {
                    sectorNodes.Add(sectorKvp.Value, new List<INode>());
                }
                foreach (var nodeKvp in Nodes)
                {
                    foreach (var sector in nodeKvp.Value.Sectors)
                    {
                        sectorNodes[sector].Add(nodeKvp.Value);
                    }
                }

                return sectorNodes;
            }

            HashSet<ulong> GetVisAreaShowObjectsChildUids()
            {
                var children = new HashSet<ulong>();
                foreach (var visArea in GetAllItems<VisibilityArea>()
                    .Where(x => x.Value.Behavior == VisibilityAreaBehavior.ShowObjects))
                {
                    foreach (IMapItem child in visArea.Value.Children)
                    {
                        children.Add(child.Uid);
                    }
                }
                return children;
            }
        }

        /// <summary>
        /// Writes the mbd of this map.
        /// </summary>
        /// <param name="mbdPath">The path of the .mbd file.</param>
        private void SaveMbd(string mbdPath)
        {
            var stream = new FileStream(mbdPath, FileMode.Create);
            using var w = new BinaryWriter(stream);

            header.Serialize(w);

            w.Write(EditorMapId);

            w.Write(StartPlacementPosition);
            w.Write(StartPlacementSectorOrSomething);

            w.Write(StartPlacementRotation);

            w.Write(gameTag);

            w.Write(NormalScale);
            w.Write(CityScale);

            w.Write(EuropeMapUiCorrections.ToByte());
        }
    }
}
