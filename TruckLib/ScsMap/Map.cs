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
        private string name;
        /// <summary>
        /// The name of the map, which is used for file and directory names.
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(Name),
                        "The map name must not be null or just whitespace.");
                }
                if (value.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    throw new ArgumentException("The map name must not contain characters which are " +
                        "not allowed in filenames", nameof(Name));
                }
                name = value;
            }
        }

        /// <summary>
        /// The map's sectors.
        /// </summary>
        public Dictionary<(int X, int Z), Sector> Sectors { get; set; } 
            = new Dictionary<(int X, int Z), Sector>();

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
        /// <param name="sectors">If set, only the given sectors will be loaded.</param>
        /// <returns>A Map object.</returns>
        public static Map Open(string mbdPath, (int,int)[] sectors = null)
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
            foreach (var (_, sector) in Sectors)
            {
                foreach (var (uid, item) in sector.MapItems)
                {
                    allItems.Add(uid, item);
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
            foreach (var (_, sector) in Sectors)
            {
                foreach (var (uid, item) in sector.MapItems)
                {
                    if (item is T t)
                    {
                        allItems.Add(uid, t);
                    }
                }
            }
            return allItems;
        }

        /// <summary>
        /// Checks if the map contains an item with the given UID.
        /// </summary>
        /// <param name="uid">The UID.</param>
        /// <returns>Whether an item with this UID exists.</returns>
        public bool HasItem(ulong uid)
        {
            foreach (var (_, sector) in Sectors)
            {
                if (sector.MapItems.ContainsKey(uid))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the item with the given UID.
        /// </summary>
        /// <param name="uid">The UID.</param>
        /// <returns>The item, or null if it doesn't exist.</returns>
        public MapItem GetItem(ulong uid)
        {
            foreach (var (_, sector) in Sectors)
            {
                if (sector.MapItems.TryGetValue(uid, out var item))
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Returns the item with the given UID if it exists.
        /// </summary>
        /// <param name="uid">The UID.</param>
        /// <param name="item">Contains the item when the method returns, if it was found;
        /// otherwise, it is set to <c>null</c>.</param>
        /// <returns>Whether the item exists in the map.</returns>
        public bool TryGetItem(ulong uid, out MapItem item)
        {
            foreach (var (_, sector) in Sectors)
            {
                if (sector.MapItems.TryGetValue(uid, out var _item))
                {
                    item = _item;
                    return true;
                }
            }
            item = null;
            return false;
        }

        /// <summary>
        /// Deletes an item. Nodes that are only used by this item 
        /// will also be deleted.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        public void Delete(MapItem item)
        {
            // delete item from all sectors
            foreach (var (_, sector) in Sectors)
            {
                sector.MapItems.Remove(item.Uid);
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

            foreach (var (_, node) in clonedNodes)
            {
                node.Position += position - selection.Origin;
                node.UpdateItemReferences(clonedItems);
                var sectorIdx = GetSectorOfCoordinate(node.Position);
                AddSector(sectorIdx.X, sectorIdx.Z);
                node.Sectors = new[] { Sectors[sectorIdx] };
                Nodes.Add(node.Uid, node);
            }

            foreach (var (_, item) in clonedItems)
            {
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
        private void ReadSectors(string mapDirectory, (int X, int Z)[] sectors = null)
        {
            var baseFiles = Directory.GetFiles(mapDirectory, "*.base");

            // create itemless instances for all the sectors first;
            // this is so we have references to the sectors
            // before we read them to deal with sectors containing
            // nodes from other sectors
            foreach (var baseFile in baseFiles)
            {
                var coords = Sector.GetSectorCoordsFromSectorFilePath(baseFile);
                if (sectors != null && !sectors.Contains(coords))
                    continue;

                var sector = new Sector(coords.X, coords.Z, this);
                sector.BasePath = baseFile;
                Sectors.Add(coords, sector);
            }

            // now read in the sectors
            foreach (var (_, sector) in Sectors)
            {
                Trace.WriteLine($"Reading sector {sector}");
                sector.Read();
            }
        }

        /// <summary>
        /// Fills in the Node and Forward/BackwardItem fields in map item and node objects
        /// by searching the Node or Item list for the UID references.
        /// </summary>
        private void UpdateReferences()
        {
            var allNodes = GetAllNodes();
            var allItems = GetAllItems();

            // first of all, find map items referenced in nodes
            Trace.WriteLine("Updating item references in nodes");
            foreach (var (_, node) in allNodes)
            {
                node.UpdateItemReferences(allItems);
            }

            // then find nodes referenced in map items
            // and map items referenced in map items
            Trace.WriteLine("Updating node & item references in items");
            foreach (var (_, item) in allItems)
            {
                item.UpdateNodeReferences(allNodes);
                if (item is IItemReferences hasItemRef)
                {
                    hasItemRef.UpdateItemReferences(allItems);
                }
            }
        }

        /// <summary>
        /// Saves the map in binary format. If the sector directory does not yet exist, it will be created.
        /// </summary>
        /// <param name="mapDirectory">The path of the directory to save the map into.</param>
        /// <param name="cleanSectorDirectory">If true, the sector directory will be emptied
        /// before saving the map.</param>
        public void Save(string mapDirectory, bool cleanSectorDirectory = true)
        {
            var sectorDirectory = Path.Combine(mapDirectory, Name);
            Directory.CreateDirectory(sectorDirectory);
            if (cleanSectorDirectory)
            {
                new DirectoryInfo(sectorDirectory).GetFiles().ToList()
                    .ForEach(f => f.Delete());
            }

            var sectorNodes = GetSectorNodes();
            var visAreaShowObjectsChildren = GetVisAreaShowObjectsChildUids();

            foreach (var (_, sector) in Sectors)
            {
                Trace.WriteLine($"Writing sector {sector}");
                sector.Save(sectorDirectory, sectorNodes[sector], visAreaShowObjectsChildren);
            }

            var mbdPath = Path.Combine(mapDirectory, $"{Name}.mbd");
            SaveMbd(mbdPath);

            Dictionary<Sector, List<INode>> GetSectorNodes()
            {
                var sectorNodes = new Dictionary<Sector, List<INode>>();
                foreach (var (_, sector) in Sectors)
                {
                    sectorNodes.Add(sector, new List<INode>());
                }
                foreach (var (_, node) in Nodes)
                {
                    foreach (var sector in node.Sectors)
                    {
                        sectorNodes[sector].Add(node);
                    }
                }

                return sectorNodes;
            }

            HashSet<ulong> GetVisAreaShowObjectsChildUids()
            {
                var children = new HashSet<ulong>();
                foreach (var (_, visArea) in GetAllItems<VisibilityArea>()
                    .Where(x => x.Value.Behavior == VisibilityAreaBehavior.ShowObjects))
                {
                    foreach (IMapItem child in visArea.Children)
                    {
                        children.Add(child.Uid);
                    }
                }
                return children;
            }
        }

        /// <summary>
        /// Writes the .mbd file of this map.
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
